using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WAMA.Core.Extensions;
using WAMA.Core.Models.DTOs;
using WAMA.Core.Models.Service;
using WAMA.Core.ViewModel;
using WAMA.Core.ViewModel.User;
using WAMA.Web.Model;

namespace WAMA.Web.Controllers
{
    public class UserToolController : AdminConsoleBaseController
    {
        private static Dictionary<UserAccountType, string> AccountTypeToolsMapping = new Dictionary<UserAccountType, string>
        {
            { UserAccountType.Patron, Constants.ADMIN_CONSOLE_USERS_PATRONS },
            { UserAccountType.Employee, Constants.ADMIN_CONSOLE_USERS_EMPLOYEES },
            { UserAccountType.Manager, Constants.ADMIN_CONSOLE_USERS_MANAGERS },
            { UserAccountType.Administrator, Constants.ADMIN_CONSOLE_USERS_ADMINISTRATORS}
        };

        private IUserAccountService _UserAccountService;
        private ICheckInService _CheckInService;
        private ICertificationService _CertificationService;
        private IWaiverService _WaiverService;

        public UserToolController(IUserAccountService userAccountService,
            ICheckInService checkinService,
            ICertificationService certificationService,
            IWaiverService waiverService)
        {
            _UserAccountService = userAccountService;
            _CheckInService = checkinService;
            _CertificationService = certificationService;
            _WaiverService = waiverService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["SuspendedPatron"] = await _UserAccountService.GetSuspendedUserAccountsAsync(UserAccountType.Patron);
            ViewData["PendingPatron"] = await _UserAccountService.GetPendingUserAccountsAsync(UserAccountType.Patron);
            SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS);
            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/Index.cshtml");
        }

        public async Task<IActionResult> Administrators()
        {
            var accounts = await _UserAccountService.GetUserAccountsAsync(new UserSearchFilterViewModel
            {
                AccountTypes = new UserAccountType[] { UserAccountType.Administrator }
            });

            SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_ADMINISTRATORS);
            ViewData[Constants.USER_ACCOUNT_TYPE] = AppString.AdministratorLabel;

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/Administrators.cshtml", accounts);
        }

        public async Task<IActionResult> Employees()
        {
            var accounts = await _UserAccountService.GetUserAccountsAsync(new UserSearchFilterViewModel
            {
                AccountTypes = new UserAccountType[] { UserAccountType.Employee }
            });

            SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_EMPLOYEES);
            ViewData[Constants.USER_ACCOUNT_TYPE] = AppString.EmployeeLabel;

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/Employees.cshtml", accounts);
        }

        public async Task<IActionResult> Managers()
        {
            var accounts = await _UserAccountService.GetUserAccountsAsync(new UserSearchFilterViewModel
            {
                AccountTypes = new UserAccountType[] { UserAccountType.Manager }
            });

            SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_MANAGERS);
            ViewData[Constants.USER_ACCOUNT_TYPE] = AppString.ManagerLabel;

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/Managers.cshtml", accounts);
        }

        public async Task<IActionResult> Patrons()
        {
            var accounts = await _UserAccountService.GetUserAccountsAsync(new UserSearchFilterViewModel
            {
                AccountTypes = new UserAccountType[] { UserAccountType.Patron }
            });

            SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_PATRONS);
            ViewData[Constants.USER_ACCOUNT_TYPE] = AppString.PatronLabel;

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/Patrons.cshtml", accounts);
        }

        public async Task<IActionResult> ViewAccount(string memberId)
        {
            var account = await _UserAccountService.GetUserAccountAsync(memberId);
            ViewBag.MemberId = memberId;
            if (!Equals(account, null) && AccountTypeToolsMapping.ContainsKey(account.AccountType))
            {
                SetActiveConsoleTool(AccountTypeToolsMapping[account.AccountType]);
            }

            account.RequestToken = HashString(account.MemberId);

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/ViewAccount.cshtml", account);
        }

        public IActionResult CreateNewUserAccount(UserAccountType type)
        {
            switch (type)
            {
                case UserAccountType.Patron:
                    SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_PATRONS);
                    break;

                case UserAccountType.Employee:
                    SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_EMPLOYEES);
                    break;

                case UserAccountType.Manager:
                    SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_MANAGERS);
                    break;

                case UserAccountType.Administrator:
                    SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_ADMINISTRATORS);
                    break;

                default:
                    return RedirectToAction(nameof(AdminConsoleController.Index).StripController());
            }

            ViewBag.AccountType = type;

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/CreateNewUserAccount.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUserAccount(UserAccountViewModel user)
        {
            if (!ModelState.IsValid)
            {
                this.SetErrorMessages(ModelState.Values
                    .Where(val => val.ValidationState == ModelValidationState.Invalid)
                    .Select(val => val.Errors.FirstOrDefault().ErrorMessage));
            }
            else
            {
                var userAccount = await _UserAccountService.GetUserAccountAsync(user.MemberId);

                if (userAccount != null)
                {
                    // TODO: User account already exists
                    SetErrorMessages(string.Format(AppString.AccountWithSameMemberIdExist, user.MemberId));
                }
                else
                {
                    try
                    {
                        await _UserAccountService.CreateUserAsync(user);
                        await _CheckInService.CreateLogInCredentialAsync(user);

                        return RedirectToAction(
                            actionName: $"{user.AccountType}s",
                            controllerName: nameof(UserToolController).StripController());
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                    when (ex.InnerException is System.Data.SqlClient.SqlException)
                    {
                        if (ex.InnerException.Message.Contains("Cannot insert duplicate key"))
                        {
                            // TODO: Revert DB actions on error
                        }
                        else
                        {
                        }
                    }
                    catch (Exception ex)
                    {
                        var errMessages = new List<string>();

                        while (ex != null)
                        {
                            errMessages.Add(ex.Message);
                            ex = ex.InnerException;
                        }

                        SetErrorMessages(errMessages);
                    }
                }
            }

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/CreateNewUserAccount.cshtml", user);
        }

        public IActionResult AddCertification(string memberId)
        {
            ViewBag.MemberId = memberId;
            SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_PATRONS);
            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/AddCertification.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> AddCertification(CertificationViewModel Certification)
        {
            if (!ModelState.IsValid)
            {
                this.SetErrorMessages(ModelState.Values
                    .Where(val => val.ValidationState == ModelValidationState.Invalid)
                    .Select(val => val.Errors.FirstOrDefault().ErrorMessage));
            }
            else
            {
                //if certification type already exist then if not expired error if expired update
                try
                {
                    await _CertificationService.AddCertificationAsync(Certification);
                    return RedirectToAction(
                        actionName: nameof(ViewCertifications),
                        routeValues: new
                        {
                            MemberId = Certification.MemberId
                        });
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                when (ex.InnerException is System.Data.SqlClient.SqlException)
                {
                    if (ex.InnerException.Message.Contains("Cannot insert duplicate key"))
                    {
                    }
                    else
                    {
                    }
                }
                catch (Exception ex)
                {
                    var errMessages = new List<string>();
                    while (ex != null)
                    {
                        errMessages.Add(ex.Message);
                        ex = ex.InnerException;
                    }

                    SetErrorMessages(errMessages);
                }
            }

            ViewBag.MemberId = Certification.MemberId;
            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/AddCertification.cshtml", Certification);
        }

        public async Task<IActionResult> ViewCertifications(string memberId)
        {
            SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_PATRONS);
            var Certifications = await _CertificationService.GetCertificationsAsync(memberId);
            ViewBag.UserAccount = await _UserAccountService.GetUserAccountAsync(memberId);
            ViewBag.MemberId = memberId;
            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/ViewCertifications.cshtml", Certifications);
        }

        public async Task<IActionResult> ViewCertification(string memberId) //need to straight idea first
        {
            SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS_PATRONS);
            var Certification = await _CertificationService.GetCertificationAsync(memberId);
            ViewBag.MemberId = memberId;
            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/ViewCertification.cshtml", Certification);
        }

        public IActionResult DeleteCertification(string memberId)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> EditAccount(string memberId)
        {
            var account = await GetUserAccountAsync(memberId);

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/EditAccount.cshtml", account);
        }

        [HttpPost]
        public async Task<IActionResult> EditAccount(UserAccountViewModel user)
        {
            if (UserMeetsBasicRequirements(user, false))
            {
                if (string.IsNullOrWhiteSpace(user.UpdatedMemberId) == false)
                {
                    user.MemberId = user.UpdatedMemberId.Trim();
                }

                await _UserAccountService.UpdateUserAccountAsync(user);
                return RedirectToAction(nameof(ViewAccount), new { MemberId = user.MemberId });
            }

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/EditAccount.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveAccount(UserAccountViewModel user)
        {
            if (UserMeetsBasicRequirements(user))
            {
                await _UserAccountService.ApproveAccountAsync(user.MemberId);
            }

            return RedirectToAction(nameof(ViewAccount), new { MemberId = user?.MemberId });
        }

        [HttpPost]
        public async Task<IActionResult> SuspendAccount(UserAccountViewModel user)
        {
            if (UserMeetsBasicRequirements(user))
            {
                await _UserAccountService.SuspendUserAccountAsync(user.MemberId);
            }

            return RedirectToAction(nameof(ViewAccount), new { MemberId = user?.MemberId });
        }

        [HttpPost]
        public async Task<IActionResult> ReactivateAccount(UserAccountViewModel user)
        {
            if (UserMeetsBasicRequirements(user))
            {
                await _UserAccountService.ReactivateUserAccountAsync(user.MemberId);
            }

            return RedirectToAction(nameof(ViewAccount), new { MemberId = user?.MemberId });
        }

        [HttpPost]
        public IActionResult DeleteAccount(UserAccountViewModel user)
        {
            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/Index.cshtml");
        }

        public async Task<IActionResult> SetPassword(string memberID)
        {
            if (string.IsNullOrWhiteSpace(memberID))
            {
                return RedirectToAction(nameof(Index));
            }

            // TODO: Better security, check that account even exists
            var loginCredential = await _CheckInService.GetLogInCredentialAsync(memberID);

            loginCredential = loginCredential ?? new LogInCredentialViewModel
            {
                MemberId = memberID
            };
            loginCredential.PasswordSetRequestVerificationToken = HashString(memberID);

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/SetPassword.cshtml", loginCredential);
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(LogInCredentialViewModel loginCredential)
        {
            try
            {
                if (LoginGredentialMeetsBasicRequirement(loginCredential))
                {
                    await _CheckInService.SetPasswordForLogInCredentialAsync(loginCredential);
                    return RedirectToAction(nameof(ViewAccount), new { MemberId = loginCredential?.MemberId });
                }
            }
            catch (Exception)
            {
                SetErrorMessages(AppString.GenericSettingPasswordError);
            }

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/SetPassword.cshtml", loginCredential);
        }

        public async Task<IActionResult> ViewCheckins(string memberId)
        {
            var checkins = await _CheckInService.GetCheckInActivitiesAsync(memberId);
            ViewBag.UserAccount = await _UserAccountService.GetUserAccountAsync(memberId);

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/ViewCheckins.cshtml", checkins);
        }

        public async Task<IActionResult> ViewWaivers(string memberId)
        {
            var checkins = await _WaiverService.GetWaiversAsync(memberId);
            ViewBag.UserAccount = await _UserAccountService.GetUserAccountAsync(memberId);

            return View($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/ViewWaivers.cshtml", checkins);
        }

        private async Task<UserAccountViewModel> GetUserAccountAsync(string memberId)
        {
            if (!string.IsNullOrWhiteSpace(memberId))
            {
                var account = await _UserAccountService.GetUserAccountAsync(memberId);

                if (Equals(account, null))
                {
                    SetErrorMessages(string.Format(AppString.AccountWithIdDoesntExist, memberId));
                }
                else
                {
                    account.RequestToken = HashString(account.MemberId);

                    if (AccountTypeToolsMapping.ContainsKey(account.AccountType))
                    {
                        SetActiveConsoleTool(AccountTypeToolsMapping[account.AccountType]);
                    }
                    else
                    {
                        SetActiveConsoleTool(Constants.ADMIN_CONSOLE_USERS);
                    }
                }

                return account;
            }

            return null;
        }

        private bool UserMeetsBasicRequirements(UserAccountViewModel user, bool ignoreModelState = true)
        {
            var allIsGood = ObjectIsNotNull(user) && HashIsGood(user.MemberId, user.RequestToken);

            if (!ignoreModelState)
            {
                allIsGood &= ValidateModelState();
            }

            return allIsGood;
        }

        private bool LoginGredentialMeetsBasicRequirement(LogInCredentialViewModel loginCredential)
        {
            var allIsGood = ObjectIsNotNull(loginCredential) &&
                HashIsGood(loginCredential.MemberId, loginCredential.PasswordSetRequestVerificationToken) &&
                ValidateModelState(nameof(loginCredential.CurrentPassword));

            if (allIsGood && !loginCredential.PassWordsMatch)
            {
                this.SetErrorMessages(AppString.PasswordMisMatch);
                allIsGood = false;
            }

            return allIsGood;
        }

        private bool ValidateModelState(params string[] exclude)
        {
            var modelIsGood = ModelState.IsValid;

            if (!modelIsGood)
            {
                var invalidModelStateErrors = ModelState
                    .Where(entry => exclude.Contains(entry.Key) == false && entry.Value.ValidationState == ModelValidationState.Invalid)
                    .Select(entry => entry.Value.Errors.FirstOrDefault().ErrorMessage);

                if (invalidModelStateErrors.Any())
                {
                    this.SetErrorMessages(invalidModelStateErrors);
                }

                modelIsGood = !invalidModelStateErrors.Any();
            }

            return modelIsGood;
        }

        private bool ObjectIsNotNull(object obj)
        {
            var objectIsNull = Equals(obj, null);

            if (objectIsNull)
            {
                this.SetErrorMessages(AppString.GenericErrorMessage);
            }

            return !objectIsNull;
        }

        private bool HashIsGood(string value, string hash)
        {
            var hashIsGood = string.IsNullOrWhiteSpace(hash) == false &&
                string.IsNullOrWhiteSpace(value) == false &&
                hash.Equals(HashString(value));

            if (hashIsGood == false)
            {
                this.SetErrorMessages(AppString.GenericErrorMessage);
            }

            return hashIsGood;
        }
    }
}