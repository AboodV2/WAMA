using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Represents UserAccountService
    /// </summary>
    /// <seealso cref="WAMA.Core.Models.Service.IUserAccountService" />
    public class UserAccountService : IUserAccountService
    {
        /// <summary>
        /// The database context provider
        /// </summary>
        private IDbContextProvider _DbCtxProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccountService" /> class.
        /// </summary>
        /// <param name="dbCtx">The database CTX.</param>
        public UserAccountService(IDbContextProvider dbCtx)
        {
            _DbCtxProvider = dbCtx;
        }

        /// <summary>
        /// Creates the user asynchronous.
        /// </summary>
        /// <param name="userAccount">The user account.</param>
        /// <returns></returns>
        public async Task CreateUserAsync(UserAccountViewModel userAccount)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                dbCtx.UserAccounts.Add(userAccount.ToDTO());
                await dbCtx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets the listserv data asynchronous.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public async Task<IEnumerable<ListservViewModel>> GetListservDataAsync(UserAccountType type)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                return await dbCtx.UserAccounts
                    .Where(user => user.AccountType == type)
                    .Select(user => new ListservViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        MiddleName = user.MiddleName,
                        Email = user.Email
                    })
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets the user account asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        public async Task<UserAccountViewModel> GetUserAccountAsync(string memberId)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                var userAccount = await dbCtx.UserAccounts
                    .FirstOrDefaultAsync(user => user.MemberId == memberId);

                return userAccount?.ToViewModel();
            }
        }

        /// <summary>
        /// Gets the user accounts asynchronous.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserAccountViewModel>> GetUserAccountsAsync(UserSearchFilterViewModel filter)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                var users = await dbCtx.UserAccounts.AppendFilterQuery(filter).ToListAsync();

                return users?.Select(user => user.ToViewModel());
            }
        }

        /// <summary>
        /// Gets the suspended user accounts asynchronous.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserAccountViewModel>> GetSuspendedUserAccountsAsync(UserAccountType type)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                return await dbCtx.UserAccounts
                    .Where(user => user.AccountType == type && user.IsSuspended == true)
                    .Select(user => user.ToViewModel())
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets the pending user accounts asynchronous.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserAccountViewModel>> GetPendingUserAccountsAsync(UserAccountType type)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                return await dbCtx.UserAccounts
                    .Where(user => user.AccountType == type && user.HasBeenApproved == false)
                    .Select(user => user.ToViewModel())
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Updates the user account asynchronous.
        /// </summary>
        /// <param name="updated">The updated.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">No user account updated</exception>
        public async Task UpdateUserAccountAsync(UserAccountViewModel updated)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                var old = dbCtx.UserAccounts.SingleOrDefault(user => user.MemberId.Equals(updated.MemberId));

                if (old == null)
                {
                    throw new InvalidOperationException("No user account updated");
                }

                dbCtx.Entry(old).CurrentValues.SetValues(updated.UpdateDTO(old));
                await dbCtx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Suspends the user account asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">No user account updated</exception>
        public async Task SuspendUserAccountAsync(string memberId)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                var old = dbCtx.UserAccounts.SingleOrDefault(user => user.MemberId.Equals(memberId));

                if (old == null)
                {
                    throw new InvalidOperationException("No user account updated");
                }

                old.IsSuspended = true;
                await dbCtx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Reactivates the user account asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">No user account updated</exception>
        public async Task ReactivateUserAccountAsync(string memberId)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                var old = dbCtx.UserAccounts.SingleOrDefault(user => user.MemberId.Equals(memberId));

                if (old == null)
                {
                    throw new InvalidOperationException("No user account updated");
                }

                old.IsSuspended = false;
                await dbCtx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Approves the account asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">No user account updated</exception>
        public async Task ApproveAccountAsync(string memberId)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                var old = dbCtx.UserAccounts.SingleOrDefault(user => user.MemberId.Equals(memberId));

                if (old == null)
                {
                    throw new InvalidOperationException("No user account updated");
                }

                old.HasBeenApproved = true;
                await dbCtx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets the overall activity aggregates asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<AdminConsoleHomeViewModel> GetOverallActivityAggregatesAsync()
        {
            var aggregate = new AdminConsoleHomeViewModel();

            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                using (var connection = dbCtx.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetGeneralAggregate";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                aggregate.NumberOfExpiredCertifications = Convert.ToInt32(reader["expiredcerts"]);
                                aggregate.NumberOfCertificationsExpiringInTheNextWeek = Convert.ToInt32(reader["expiringcerts"]);
                                aggregate.NumberOfNonPatronCheckinsToday = Convert.ToInt32(reader["nonpatroncheckins"]);
                                aggregate.NumberOfPatronCheckinsToday = Convert.ToInt32(reader["patroncheckins"]);
                                aggregate.NumberOfPatronsPendingApprobal = Convert.ToInt32(reader["pendingapproval"]);
                                aggregate.NumberOfSuspendedPatrons = Convert.ToInt32(reader["suspended"]);
                            }
                        }
                    }
                }
            }

            return aggregate;
        }
    }
}