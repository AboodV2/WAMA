using System;

namespace WAMA.Core.Models.DTOs
{
    /// <summary>
    /// Represents Waiver
    /// </summary>
    /// <seealso cref="WAMA.Core.Models.DTOs.TableRow" />
    public class Waiver : TableRow
    {
        /// <summary>
        /// Gets or sets the member identifier of this Waiver.
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// Gets or sets the signed on of this Waiver.
        /// </summary>
        public DateTime SignedOn { get; set; }

        /// <summary>
        /// Gets or sets the member of this Waiver.
        /// </summary>
        public virtual UserAccount Member { get; set; }

        /// <summary>
        /// Gets or sets the signature of this Waiver.
        /// </summary>
        public string Signature { get; set; }
    }
}