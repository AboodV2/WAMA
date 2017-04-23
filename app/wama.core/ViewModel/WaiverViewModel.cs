using System;
using System.ComponentModel.DataAnnotations;
using WAMA.Core.Models.DTOs;

namespace WAMA.Core.ViewModel
{
    /// <summary>
    /// Represents WaiverViewModel
    /// </summary>
    public class WaiverViewModel
    {
        /// <summary>
        /// Gets or sets the member identifier of this WaiverViewModel.
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// Gets or sets the signed on of this WaiverViewModel.
        /// </summary>
        public DateTimeOffset SignedOn { get; set; }

        /// <summary>
        /// Gets the signing signature.
        /// </summary>
        [Display(Name = "Signing signature")]
        [Required]
        public string Signature { get; set; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        [Display(Name = "Date of signing")]
        public string Date => $"{SignedOn:D}";

        /// <summary>
        /// Gets the time.
        /// </summary>
        [Display(Name = "Time of signing")]
        public string Time => $"{SignedOn:T}";

        /// <summary>
        /// Gets or sets the member that owns this WaiverViewModel.
        /// </summary>
        public virtual UserAccount Member { get; set; }
    }
}