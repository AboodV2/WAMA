using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WAMA.Core.Models.DTOs;
using WAMA.Core.Models.Service;
using WAMA.Core.ViewModel.User;
using WAMA.Web;
using WAMA.Web.Controllers;
using WAMA.Web.Model;
using Xunit;

namespace WAMAcut.Integration.Tests
{
    public class UserToolControllerTest
    {
        private static UserAccountViewModel _PatronUserAccountViewModel = new PatronUserAccountViewModel
        {
            MemberId = string.Empty
        };

        [Fact]
        public async Task Index()
        {
            var userToolController = MockUserToolController();
            var result = await userToolController.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var suspendedPatron = Assert.IsAssignableFrom<IEnumerable<UserAccountViewModel>>(viewResult.ViewData["SuspendedPatron"]);
            var pendingPatron = Assert.IsAssignableFrom<IEnumerable<UserAccountViewModel>>(viewResult.ViewData["PendingPatron"]);
        }

        [Fact]
        public async Task Administrators()
        {
            var userToolController = MockUserToolController();
            var result = await userToolController.Administrators();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(Constants.ADMIN_CONSOLE_USERS_ADMINISTRATORS,
                viewResult.ViewData[Constants.ADMIN_CONSOLE_ACTIVE_TOOL]);

            Assert.Equal(AppString.AdministratorLabel,
                viewResult.ViewData[Constants.USER_ACCOUNT_TYPE]);

            Assert.Equal($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/Administrators.cshtml",
                viewResult.ViewName);
        }

        [Fact]
        public async Task Employees()
        {
            var userToolController = MockUserToolController();
            var result = await userToolController.Employees();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(Constants.ADMIN_CONSOLE_USERS_EMPLOYEES,
                viewResult.ViewData[Constants.ADMIN_CONSOLE_ACTIVE_TOOL]);

            Assert.Equal(AppString.EmployeeLabel,
                viewResult.ViewData[Constants.USER_ACCOUNT_TYPE]);

            Assert.Equal($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/Employees.cshtml",
                viewResult.ViewName);
        }

        [Fact]
        public async Task Managers()
        {
            var userToolController = MockUserToolController();
            var result = await userToolController.Managers();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(Constants.ADMIN_CONSOLE_USERS_MANAGERS,
                viewResult.ViewData[Constants.ADMIN_CONSOLE_ACTIVE_TOOL]);

            Assert.Equal(AppString.ManagerLabel,
                viewResult.ViewData[Constants.USER_ACCOUNT_TYPE]);

            Assert.Equal($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/Managers.cshtml",
                viewResult.ViewName);
        }

        [Fact]
        public async Task Patrons()
        {
            var userToolController = MockUserToolController();
            var result = await userToolController.Patrons();

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(Constants.ADMIN_CONSOLE_USERS_PATRONS,
                viewResult.ViewData[Constants.ADMIN_CONSOLE_ACTIVE_TOOL]);

            Assert.Equal(AppString.PatronLabel,
                viewResult.ViewData[Constants.USER_ACCOUNT_TYPE]);

            Assert.Equal($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/Patrons.cshtml",
                viewResult.ViewName);
        }

        [Fact]
        public async Task ViewAccount()
        {
            var userToolController = MockUserToolController();
            var result = await userToolController.ViewAccount(string.Empty);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal(Constants.ADMIN_CONSOLE_USERS_PATRONS,
                viewResult.ViewData[Constants.ADMIN_CONSOLE_ACTIVE_TOOL]);

            Assert.Equal($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/ViewAccount.cshtml",
                viewResult.ViewName);
        }

        [Fact]
        public void CreateNewUserAccount()
        {
            var userToolController = MockUserToolController();
            var result = userToolController.CreateNewUserAccount(UserAccountType.Unknown);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/CreateNewUserAccount.cshtml",
                viewResult.ViewName);
        }

        [Fact]
        public async Task CreateNewUserAccount_POST()
        {
            // TODO: Test existing memberid
            // TODO: Test new memberId

            var errMessage = "err message 1";
            var userToolController = MockUserToolController();
            userToolController.ModelState.AddModelError("err1", errMessage);
            var result = await userToolController.CreateNewUserAccount(_PatronUserAccountViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);

            var errors = Assert.IsAssignableFrom<IEnumerable<string>>(viewResult.ViewData[Constants.ERROR_MESSAGES]);

            Assert.Equal(1, errors.Count());
            Assert.Equal(errMessage, errors.First());
            Assert.Equal($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/CreateNewUserAccount.cshtml",
                viewResult.ViewName);
        }

        [Fact]
        public async Task EditAccount()
        {
            var userToolController = MockUserToolController();
            var result = await userToolController.EditAccount(string.Empty);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/EditAccount.cshtml",
                viewResult.ViewName);
        }

        [Fact]
        public async Task EditAccount_POST()
        {
            // TODO: switch to theory for testing proper post
            var userToolController = MockUserToolController();
            var result = await userToolController.EditAccount(_PatronUserAccountViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal($"{Constants.ADMIN_CONSOLE_USER_TOOL_DIRECTORY}/EditAccount.cshtml",
                viewResult.ViewName);
        }

        [Fact]
        public async Task ApproveAccount_POST()
        {
            // TODO: switch to theory for testing proper post
            var userToolController = MockUserToolController();
            var result = await userToolController.ApproveAccount(_PatronUserAccountViewModel);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(UserToolController.ViewAccount), viewResult.ActionName);
            Assert.Equal(_PatronUserAccountViewModel.MemberId, viewResult.RouteValues["MemberId"]);
        }

        [Fact]
        public async Task SuspendAccount()
        {
            // TODO: switch to theory for testing proper post
            var userToolController = MockUserToolController();
            var result = await userToolController.SuspendAccount(_PatronUserAccountViewModel);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(UserToolController.ViewAccount), viewResult.ActionName);
            Assert.Equal(_PatronUserAccountViewModel.MemberId, viewResult.RouteValues["MemberId"]);
        }

        [Fact]
        public async Task ReactivateAccount()
        {
            // TODO: switch to theory for testing proper post
            var userToolController = MockUserToolController();
            var result = await userToolController.ReactivateAccount(_PatronUserAccountViewModel);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(UserToolController.ViewAccount), viewResult.ActionName);
            Assert.Equal(_PatronUserAccountViewModel.MemberId, viewResult.RouteValues["MemberId"]);
        }

        [Fact]
        public void DeleteAccount()
        {
        }

        private static UserToolController MockUserToolController()
        {
            var mockCheckinService = new Mock<ICheckInService>();
            var mockUserAccountService = MockUserAccountService();
            var mockCertificationService = new Mock<ICertificationService>();
            var mockWaiverService = new Mock<IWaiverService>();

            var userToolController = new UserToolController(mockUserAccountService, 
                mockCheckinService.Object, 
                mockCertificationService.Object, 
                mockWaiverService.Object);
            return userToolController;
        }

        private static IUserAccountService MockUserAccountService()
        {
            var mockUserAccountService = new Mock<IUserAccountService>();

            //mockUserAccountService.Setup(cs => cs.GetSuspendedUserAccountsAsync(UserAccountType.Administrator))
            //    .ReturnsAsync(Enumerable.Empty<AdministratorUserAccountViewModel>());
            //mockUserAccountService.Setup(cs => cs.GetSuspendedUserAccountsAsync(UserAccountType.Manager))
            //    .ReturnsAsync(Enumerable.Empty<ManagerUserAccountViewModel>());
            //mockUserAccountService.Setup(cs => cs.GetSuspendedUserAccountsAsync(UserAccountType.Employee))
            //    .ReturnsAsync(Enumerable.Empty<EmployeeUserAccountViewModel>());
            //mockUserAccountService.Setup(cs => cs.GetSuspendedUserAccountsAsync(UserAccountType.Patron))
            //    .ReturnsAsync(Enumerable.Empty<PatronUserAccountViewModel>());
            //mockUserAccountService.Setup(cs => cs.GetPendingUserAccountsAsync(UserAccountType.Administrator))
            //    .ReturnsAsync(Enumerable.Empty<AdministratorUserAccountViewModel>());
            //mockUserAccountService.Setup(cs => cs.GetPendingUserAccountsAsync(UserAccountType.Manager))
            //    .ReturnsAsync(Enumerable.Empty<ManagerUserAccountViewModel>());
            //mockUserAccountService.Setup(cs => cs.GetPendingUserAccountsAsync(UserAccountType.Employee))
            //    .ReturnsAsync(Enumerable.Empty<EmployeeUserAccountViewModel>());
            //mockUserAccountService.Setup(cs => cs.GetPendingUserAccountsAsync(UserAccountType.Patron))
            //    .ReturnsAsync(Enumerable.Empty<PatronUserAccountViewModel>());

            mockUserAccountService.SetReturnsDefault(
                Enumerable.Empty<UserAccountViewModel>());

            mockUserAccountService.Setup(uas => uas.GetUserAccountAsync(string.Empty))
                .ReturnsAsync(_PatronUserAccountViewModel);

            return mockUserAccountService.Object;
        }
    }
}