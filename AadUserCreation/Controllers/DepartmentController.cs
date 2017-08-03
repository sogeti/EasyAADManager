using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AadUserCreation.Models;
using AadUserCreation.Business.Interfaces;
using AadUserCreation.Data.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace AadUserCreation.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly IDepartmentContext _departmentContext;
        private readonly IAadCollector _aadCollector;
        private readonly IGroupContext _groupContext;
        private readonly ILogger _logger;

        public DepartmentController(IOptions<AppSettings> appSettings, IDepartmentContext departmentContext, IAadCollector aadCollector, IGroupContext groupContext, ILogger<DepartmentController> logger)
        {
            _appSettings = appSettings.Value;
            _departmentContext = departmentContext;
            _aadCollector = aadCollector;
            _groupContext = groupContext;
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var GroupCollection = await _aadCollector.LoadGroupMembershipsForGroupsFromBlob();
                var user = new User();
                ViewBag.DepartmentList = GroupCollection.DepartmentList;
                ViewBag.ApplicationName = _appSettings.ApplicationName;

                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "DepartmentController");
                TempData["message"] = e.ToString();
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DepartmentList department)
        {
            var GroupCollection = await _aadCollector.LoadGroupMembershipsForGroupsFromBlob();
            try
            {
                var succes = await _departmentContext.AddDepartmentToTable(department);
                if (!succes)
                {
                    throw (new Exception("Error adding Department"));
                }

                DepartmentList dep = new DepartmentList();
                dep.Name = department.Name;
                GroupCollection.DepartmentList.Add(dep);
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "DepartmentController");
                return View(department);
            }

            ViewBag.DepartmentList = GroupCollection.DepartmentList;
            ModelState.Clear();
            return View();
        }


        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var succes = await _groupContext.RemoveGroupFromTable(id);
                if (!succes)
                {
                    TempData["message"] = "Error on delete group operation.";
                    return RedirectToAction("Error");
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "DepartmentController");

                if (e.Message.Contains("Insufficient privileges"))
                {
                    TempData["message"] = "Insufficient privileges to complete the operation.";
                    return RedirectToAction("Error");
                }
                TempData["message"] = e.Message;
                return RedirectToAction("Error");
            }
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            ViewBag.ApplicationName = _appSettings.ApplicationName;
            ViewBag.ErrorMessage = TempData["message"];
            return View();
        }
    }
}