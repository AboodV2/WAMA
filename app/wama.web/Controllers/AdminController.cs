using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WAMA.Core.Extensions;
using WAMA.Core.Models.Service;
using WAMA.Core.ViewModel;

namespace WAMA.Web.Controllers
{
    public class AdminController : WamaBaseController
    {
        private static ICheckInService _CheckInService;
        private IUserAccountService _UserAccountService;

        public AdminController(ICheckInService checkInService, IUserAccountService userAccountService)
        {
            _UserAccountService = userAccountService;
            _CheckInService = checkInService;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(AdminAccessLogin));
        }

        [HttpGet]
        public IActionResult AdminAccessLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminAccessLogin(LogInCredentialViewModel loginCredential)
        {
            try
            {
                var result = await _CheckInService.PerformCheckInAsync(loginCredential);

                if (result.IsCheckedIn)
                {
                    return RedirectToAction(actionName: nameof(AdminConsoleController.Index),
                        controllerName: nameof(AdminConsoleController).StripController());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }

            return RedirectToAction(nameof(AccessDenied));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}