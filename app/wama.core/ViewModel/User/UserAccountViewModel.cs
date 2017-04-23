using System.ComponentModel.DataAnnotations;
using WAMA.Core.Models.DTOs;

namespace WAMA.Core.ViewModel.User
{
    /// <summary>
    /// Represents UserAccountViewModel
    /// </summary>
    public abstract class UserAccountViewModel
    {
        /// <summary>
        /// Gets the type of the account.
        /// </summary>
        [Required]
        [Display(Name = "Account Type")]
        public virtual UserAccountType AccountType { get; }

        /// <summary>
        /// Gets or sets the member identifier of this UserAccountViewModel.
        /// </summary>
        [Required]
        [Display(Name = "ID (6 or 7 digit SDSU ID)")]
        [MaxLength(7, ErrorMessage = "Member ID length should be either 6 or 7 digits long.")]
        [MinLength(6, ErrorMessage = "Member ID length should be either 6 or 7 digits long.")]
        public string MemberId { get; set; }

        /// <summary>
        /// Gets or sets the updated member identifier of this UserAccountViewModel.
        /// </summary>
        [Display(Name = "Updated ID (7 digit SDSU ID)")]
        public string UpdatedMemberId { get; set; }

        /// <summary>
        /// Gets or sets the first name of this UserAccountViewModel.
        /// </summary>
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of this UserAccountViewModel.
        /// </summary>
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the name of the middle.
        /// </summary>
        /// <value>
        /// The name of the middle.
        /// </value>
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the email of this UserAccountViewModel.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the gender of this UserAccountViewModel.
        /// </summary>
        [Required]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets the institution affiliation of this UserAccountViewModel.
        /// </summary>
        [Required]
        [Display(Name = "Affiliation with the school")]
        public InstitutionAffiliation InstitutionAffiliation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has been approved.
        /// </summary>
        [Display(Name = "Account has been approved")]
        public bool HasBeenApproved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is suspended.
        /// </summary>
        [Display(Name = "Account is suspended")]
        public bool IsSuspended { get; set; }

        /// <summary>
        /// Gets or sets the request token of this UserAccountViewModel.
        /// </summary>
        public string RequestToken { get; set; }

        /// <summary>
        /// Gets the combination of First name, Last name (delimited by space).
        /// </summary>
        public string FirstNameLastName => $"{FirstName} {LastName}";
    }
}