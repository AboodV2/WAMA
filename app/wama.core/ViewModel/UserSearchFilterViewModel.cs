using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WAMA.Core.Models.DTOs;

namespace WAMA.Core.ViewModel
{
    /// <summary>
    /// Represents UserSearchFilterViewModel. <para />
    /// This class is used to specify filters used to search for <see cref="UserAccount"/>>
    /// </summary>
    public class UserSearchFilterViewModel
    {
        /// <summary>
        /// Gets or sets the name of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the member i ds of this UserSearchFilterViewModel.
        /// </summary>
        public IEnumerable<string> MemberIDs
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CommaSeparatedMemberIDs))
                {
                    return Enumerable.Empty<string>();
                }

                return CommaSeparatedMemberIDs.Split(',').Select(id => id.Trim());
            }
        }

        /// <summary>
        /// Gets or sets the member i ds of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Member ID(s)")]
        public string CommaSeparatedMemberIDs { get; set; }

        /// <summary>
        /// Gets or sets the account types of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Account types")]
        public IEnumerable<UserAccountType> AccountTypes { get; set; }

        /// <summary>
        /// Gets or sets the account is suspended of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Account is suspended")]
        public bool AccountIsSuspended { get; set; }

        /// <summary>
        /// Gets or sets the account is approved of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Account is approved")]
        public bool AccountIsApproved { get; set; }

        /// <summary>
        /// Gets or sets the certified on of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Certified on")]
        [DataType(DataType.Date)]
        public DateTime? CertifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the certified after of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Certified after")]
        [DataType(DataType.Date)]
        public DateTime? CertifiedAfter { get; set; }

        /// <summary>
        /// Gets or sets the certified before of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Certified before")]
        [DataType(DataType.Date)]
        public DateTime? CertifiedBefore { get; set; }

        /// <summary>
        /// Gets or sets the signed waiver on of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Signed waiver on")]
        [DataType(DataType.Date)]
        public DateTime? SignedWaiverOn { get; set; }

        /// <summary>
        /// Gets or sets the signed waiver after of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Signed waiver after")]
        [DataType(DataType.Date)]
        public DateTime? SignedWaiverAfter { get; set; }

        /// <summary>
        /// Gets or sets the signed waiver before of this UserSearchFilterViewModel.
        /// </summary>
        [Display(Name = "Signed waiver before")]
        [DataType(DataType.Date)]
        public DateTime? SignedWaiverBefore { get; set; }
    }
}