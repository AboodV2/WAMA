using WAMA.Core.Models.DTOs;
using WAMA.Core.ViewModel;

namespace WAMA.Core.Extensions
{
    /// <summary>
    /// Represents WaiverExtensions
    /// </summary>
    public static class WaiverExtensions
    {
        /// <summary>
        /// Converts the ViewModel to DTO.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        public static Waiver ToDTO(this WaiverViewModel viewModel)
        {
            return new Waiver
            {
                MemberId = viewModel.MemberId,
                SignedOn = viewModel.SignedOn.Date,
                Signature = viewModel.Signature
            };
        }

        /// <summary>
        /// Converts the DTO to ViewModel.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        public static WaiverViewModel ToViewModel(this Waiver dto)
        {
            return new WaiverViewModel
            {
                MemberId = dto.MemberId,
                SignedOn = dto.SignedOn.Date,
                Signature = dto.Signature
            };
        }
    }
}