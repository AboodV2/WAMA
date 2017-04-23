using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WAMA.Core.ViewModel;
using WAMA.Core.ViewModel.User;

namespace WAMA.Core.Models.Service
{
    /// <summary>
    /// API Contract for interacting with <see cref="DTOs.CheckInActivity"/> in the Database
    /// </summary>
    public interface ICheckInService
    {
        /// <summary>
        /// Creates the log in credential asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task CreateLogInCredentialAsync(UserAccountViewModel user);

        /// <summary>
        /// Gets the log in credential asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        Task<LogInCredentialViewModel> GetLogInCredentialAsync(string memberId);

        /// <summary>
        /// Performs the check in asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        Task<CheckInActivityViewModel> PerformCheckInAsync(string memberId);

        /// <summary>
        /// Performs the check in asynchronous.
        /// </summary>
        /// <param name="memberId">The login credential.</param>
        /// <returns></returns>
        Task<CheckInActivityViewModel> PerformCheckInAsync(LogInCredentialViewModel loginCredential);

        /// <summary>
        /// Gets the check in activities for member asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<CheckInActivityViewModel>> GetCheckInActivitiesAsync(string memberId);

        /// <summary>
        /// Gets the check in activities for period asynchronous.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        Task<IEnumerable<CheckInActivityViewModel>> GetCheckInActivitiesForPeriodAsync(DateTimeOffset start);

        /// <summary>
        /// Gets the check in activities for period asynchronous.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        Task<IEnumerable<CheckInActivityViewModel>> GetCheckInActivitiesForPeriodAsync(DateTimeOffset start, DateTimeOffset end);

        /// <summary>
        /// Gets the check in activity aggregates asynchronous.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="granularity">The granularity.</param>
        /// <returns></returns>
        Task<IEnumerable<CheckInActivityAggregateViewModel>> GetCheckInActivityAggregatesAsync(
           DateTimeOffset start,
           DateTimeOffset end,
           ReportGranularity granularity);


        /// <summary>
        /// Sets the password for log in credential asynchronous.
        /// </summary>
        /// <param name="loginCredential">The login credential.</param>
        /// <returns></returns>
        Task SetPasswordForLogInCredentialAsync(LogInCredentialViewModel loginCredential);
    }
}