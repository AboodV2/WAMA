using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WAMA.Core.Extensions;
using WAMA.Core.Models.Provider;
using WAMA.Core.Models.Service;
using WAMA.Core.ViewModel;

namespace WAMA.Core.Services
{
    /// <summary>
    /// Represents WaiverService
    /// </summary>
    /// <seealso cref="WAMA.Core.Models.Service.IWaiverService" />
    public class WaiverService : IWaiverService
    {
        /// <summary>
        /// The database context provider
        /// </summary>
        private IDbContextProvider _DbCtxProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaiverService"/> class.
        /// </summary>
        /// <param name="dbCtx">The database CTX.</param>
        public WaiverService(IDbContextProvider dbCtx)
        {
            _DbCtxProvider = dbCtx;
        }

        /// <summary>
        /// Adds the waiver asynchronous.
        /// </summary>
        /// <param name="waiver">The waiver.</param>
        /// <returns></returns>
        public async Task AddWaiverAsync(WaiverViewModel waiver)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                await dbCtx.Waivers.AddAsync(waiver.ToDTO());
                await dbCtx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets the waiver asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        public async Task<WaiverViewModel> GetWaiverAsync(string memberId)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                var waiver = await dbCtx.Waivers
                    .FirstOrDefaultAsync(wv => wv.MemberId == memberId);

                return waiver?.ToViewModel();
            }
        }

        /// <summary>
        /// Gets the waivers asynchronous.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<WaiverViewModel>> GetWaiversAsync(string memberId)
        {
            using (var dbCtx = _DbCtxProvider.GetWamaDbContext())
            {
                return await dbCtx.Waivers
                    .Where(waiver => waiver.MemberId == memberId)
                    .Select(waiver => waiver.ToViewModel())
                    .ToListAsync();
            }
        }
    }
}