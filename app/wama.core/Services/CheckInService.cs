using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WAMA.Core.Extensions;
using WAMA.Core.Models.DTOs;
using WAMA.Core.Models.Provider;
using WAMA.Core.Models.Service;
using WAMA.Core.ViewModel;
using WAMA.Core.ViewModel.User;

namespace WAMA.Core.Services
{
    /// <summary>
    /// Represents CheckInService
    /// </summary>
    /// <seealso cref="WAMA.Core.Models.Service.ICheckInService" />
    public class CheckInService : ICheckInService
    {
        /// <summary>
        /// The database context provider
        /// </summary>
        private IDbContextProvider _DbCtxProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckInService" /> class.
        /// </summary>
        /// <param name="dbCtx">The database CTX.</param>
        public CheckInService(IDbContextProvider dbCtx)
        {
            _DbCtxProvider = dbCtx;
        }

        /// <summary>
        /// Creates the log in credential asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public async Task CreateLogInCredentialAsync(UserAccountViewModel user)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                dbCtx.LogInCredentials.Add(new LogInCredential
                {
                    MemberId = user.MemberId,
                    RequiresPassword = user.AccountType != UserAccountType.Patron
                });
                await dbCtx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets the log in credential asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        public async Task<LogInCredentialViewModel> GetLogInCredentialAsync(string memberId)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                var loginCredential = await dbCtx.LogInCredentials
                    .FirstOrDefaultAsync(lc => lc.MemberId == memberId);

                return loginCredential?.ToViewModel();
            }
        }

        /// <summary>
        /// Performs the check in asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        public async Task<CheckInActivityViewModel> PerformCheckInAsync(string memberId)
        {
            var checkinActivity = new CheckInActivity
            {
                MemberId = memberId,
                CheckInDateTime = DateTimeOffset.UtcNow,
                IsCheckedIn = true,
            };

            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                dbCtx.CheckInActivities.Add(checkinActivity);
                await dbCtx.SaveChangesAsync();
            }

            return checkinActivity?.ToViewModel();
        }

        /// <summary>
        /// Performs the check in asynchronous.
        /// </summary>
        /// <param name="loginCredential"></param>
        /// <returns></returns>
        public async Task<CheckInActivityViewModel> PerformCheckInAsync(LogInCredentialViewModel loginCredential)
        {
            var checkinActivity = new CheckInActivity
            {
                MemberId = loginCredential.MemberId,
                CheckInDateTime = DateTimeOffset.UtcNow,
                IsCheckedIn = false,
            };

            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                checkinActivity.IsCheckedIn = await dbCtx.LogInCredentials.AnyAsync(_loginCredntial =>
                    _loginCredntial.MemberId == loginCredential.MemberId &&
                     _loginCredntial.HashedPassword == loginCredential.Password
                );

                if (checkinActivity.IsCheckedIn)
                {
                    await dbCtx.CheckInActivities.AddAsync(checkinActivity);
                    await dbCtx.SaveChangesAsync();
                }
            }

            return checkinActivity?.ToViewModel();
        }

        /// <summary>
        /// Gets the check in activities for member asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<CheckInActivityViewModel>> GetCheckInActivitiesAsync(string memberId)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                return await dbCtx.CheckInActivities
                    .Where(checkInActivity => checkInActivity.MemberId == memberId)
                    .Select(checkInActivity => checkInActivity.ToViewModel())
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets the check in activities for period asynchronous.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public async Task<IEnumerable<CheckInActivityViewModel>> GetCheckInActivitiesForPeriodAsync(DateTimeOffset start)
        {
            return await GetCheckInActivitiesForPeriodAsync(start, DateTimeOffset.MaxValue);
        }

        /// <summary>
        /// Gets the check in activities for period asynchronous.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public async Task<IEnumerable<CheckInActivityViewModel>> GetCheckInActivitiesForPeriodAsync(DateTimeOffset start, DateTimeOffset end)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                return await dbCtx.CheckInActivities
                    .Where(checkInActivity => checkInActivity.CheckInDateTime >= start && checkInActivity.CheckInDateTime <= end)
                    .OrderBy(checkInActivity => checkInActivity.CheckInDateTime)
                    .Select(checkInActivity => checkInActivity.ToViewModel())
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets the check in activity aggregates asynchronous.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="granularity">The granularity.</param>
        /// <returns></returns>
        public async Task<IEnumerable<CheckInActivityAggregateViewModel>> GetCheckInActivityAggregatesAsync(
            DateTimeOffset start,
            DateTimeOffset end,
            ReportGranularity granularity)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                using (var connection = dbCtx.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetCheckInAggregate";

                        command.Parameters.Add(new SqlParameter("@granularity", granularity));
                        command.Parameters.Add(new SqlParameter("@startDate", start));
                        command.Parameters.Add(new SqlParameter("@endDate", end));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var aggregates = new List<CheckInActivityAggregateViewModel>();

                            while (reader.Read())
                            {
                                aggregates.Add(new CheckInActivityAggregateViewModel
                                {
                                    Time = Convert.ToDateTime(reader["time"]),
                                    Count = Convert.ToInt32(reader["count"]),
                                    ReportGranularity = granularity
                                });
                            }

                            return aggregates;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the password for log in credential asynchronous.
        /// </summary>
        /// <param name="loginCredential">The login credential.</param>
        /// <returns></returns>
        public async Task SetPasswordForLogInCredentialAsync(LogInCredentialViewModel loginCredential)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                var old = dbCtx.LogInCredentials.SingleOrDefault(_loginCredential => _loginCredential.MemberId == loginCredential.MemberId);

                if (old == null || string.Equals(loginCredential.CurrentPassword, old.HashedPassword) == false)
                {
                    throw new InvalidOperationException("No login credential updated");
                }

                old.HashedPassword = loginCredential.Password;

                dbCtx.Entry(old).CurrentValues.SetValues(old);
                await dbCtx.SaveChangesAsync();
            }
        }
    }
}