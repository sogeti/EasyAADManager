using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AadUserCreation.Data.Interfaces;
using AadUserCreation.Models;
using Microsoft.Extensions.Options;
using AadUserCreation.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace AadUserCreation.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly IGroupContext _groupContext;
        private readonly IAadCollector _aadCollector;
        private readonly ILogger _logger;

        public GroupController(IOptions<AppSettings> appSettings,IGroupContext groupContext, IAadCollector aadCollector, ILogger<GroupController> logger)
        {
            _appSettings = appSettings.Value;
            _groupContext = groupContext;
            _aadCollector = aadCollector;
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var GroupCollection = await _aadCollector.LoadGroupMembershipsForGroupsFromBlob();
                var user = new User();
                ViewBag.GroupList = GroupCollection.GroupList;
                ViewBag.ApplicationName = _appSettings.ApplicationName;

                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "GroupController");
                TempData["message"] = e.ToString();
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(GroupList group)
        {
            var GroupCollection = await _aadCollector.LoadGroupMembershipsForGroupsFromBlob();
            try
            {
                var succes = await _groupContext.AddGroupToTable(group);
                if (!succes)
                {
                    throw (new Exception("Error adding Group"));
                }
                
                GroupList grp = new GroupList();
                grp.Name = group.Name;
                GroupCollection.GroupList.Add(grp);

               
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "GroupController");
                return View(group);
            }

            ViewBag.GroupList = GroupCollection.GroupList;

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
                _logger.LogError(8, e, "GroupController");

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