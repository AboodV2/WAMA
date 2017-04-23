using System.Collections.Generic;
using System.Threading.Tasks;
using WAMA.Core.ViewModel;

namespace WAMA.Core.Models.Service
{
    /// <summary>
    /// API Contract for interacting with <see cref="DTOs.Waiver"/> in the Database
    /// </summary>
    public interface IWaiverService
    {
        /// <summary>
        /// Adds the waiver asynchronous.
        /// </summary>
        /// <param name="waiver">The waiver.</param>
        /// <returns></returns>
        Task AddWaiverAsync(WaiverViewModel waiver);

        /// <summary>
        /// Gets the waiver asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        Task<WaiverViewModel> GetWaiverAsync(string memberId);

        /// <summary>
        /// Gets the waivers asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<WaiverViewModel>> GetWaiversAsync(string memberId);
    }
}