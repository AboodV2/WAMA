﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WAMA.Core.Extensions;
using WAMA.Core.Models.Service;
using WAMA.Core.ViewModel.User;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WAMA.Web.Controllers
{
    public class PatronController : WamaBaseController
    {
        // Used to throttle account creation rate
        private const int ACCOUNT_CREATION_THROTTLE_RATE = 10;

        private IUserAccountService _UserAccountService;
        private ICheckInService _CheckInService;

        public PatronController(IUserAccountService userAccountService, ICheckInService checkinService)
        {
            _UserAccountService = userAccountService;
            _CheckInService = checkinService;
        }

        [HttpGet]
        public IActionResult Create(string memberId)
        {
            ViewBag.MemberId = memberId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatronUserAccountViewModel patron)
        {
            if (!ModelState.IsValid)
            {
                this.SetErrorMessages(ModelState.Values
                    .Where(val => val.ValidationState == ModelValidationState.Invalid)
                    .Select(val => val.Errors.FirstOrDefault().ErrorMessage));
            }
            else
            {
                var userAccount = await _UserAccountService.GetUserAccountAsync(patron.MemberId);

                if (userAccount != null)
                {
                    // TODO: User account already exists
                    SetErrorMessages(string.Format(AppString.AccountWithSameMemberIdExist, patron.MemberId));
                }
                else
                {
                    try
                    {
                        await _UserAccountService.CreateUserAsync(patron);

                        return RedirectToAction(
                            actionName: nameof(CheckInController.Index),
                            controllerName: nameof(CheckInController).StripController());
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

            return View(patron);
        }
    }
}