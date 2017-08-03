using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AadUserCreation.Models;
using Microsoft.Extensions.Options;
using AadUserCreation.Business.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace AadUserCreation.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
       
        private readonly AppSettings _appSettings;
        private readonly IAadUserService _aadUserCreationService;
        private readonly IAadCollector _aadCollector;
        private readonly ILogger _logger;


        public HomeController(IOptions<AppSettings> appSettings, IAadUserService aadUserCreationService, IAadCollector aadCollector, ILogger<HomeController> logger)
        {
            _appSettings = appSettings.Value;
            _aadUserCreationService = aadUserCreationService;
            _aadCollector = aadCollector;
            _logger = logger;
        }

     

        public async Task<IActionResult> Index()
        {
            try
            {
                var groupData = await _aadCollector.GetGroupMembershipsForGroups();
                var departmentList = new List<SelectListItem>();
                foreach (var item in groupData.DepartmentList)
                {
                    departmentList.Add(new SelectListItem
                    {
                        Text = item.Name.ToString(),
                        Value = item.Id
                    });
                }

                ViewBag.DepartmentList = departmentList;
                ViewBag.MailAccountDomain = _appSettings.MailAccountDomain;
                ViewBag.ApplicationName = _appSettings.ApplicationName;

                var user = new User();
                return View(user);
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "HomeController");
                TempData["message"] = e.ToString();
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(User userToAdd)
        {
            try
            {
                var requestByUser = User.Identity.Name;
                string jobId = await _aadUserCreationService.AddAadUser(userToAdd, requestByUser, "Add");
                ModelState.Clear();
                return Json(jobId);
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "HomeController");
                return Json("Error, please notify administrator.");
            }
        }

        public IActionResult Delete()
        {
            try
            {
                ViewBag.ApplicationName = _appSettings.ApplicationName;
                ViewBag.MailAccountDomain = _appSettings.MailAccountDomain;
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "HomeController");
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult>  DeleteUser(string jsonData)
        {
            try
            {
                string jobId = await _aadUserCreationService.RemoveAadUser(jsonData, User.Identity.Name, "Delete");
                ModelState.Clear();
                return RedirectToAction("Delete", "Home");
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "HomeController");
                return Json("Error, please notify administrator.");
            }
        }


        public async Task<IActionResult> RefreshCollectionBlob()
        {
            try
            {
                ViewBag.BlobCollectionState = await _aadCollector.CollectionBlobState();
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "HomeController");
                return View();
            }
        }

        [HttpPost]
          public async Task<IActionResult> PostRefreshCollectionBlob()
        {
            try
            {
                var t= await _aadCollector.LoadGroupMembershipsForGroupsFromAad();
                return Json("Loaded");
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "HomeController");
                return Json("Error, please notify administrator.");
            }
        }

        

        public async Task<IActionResult> Edit(string upn)
        {
            try
            {

                var user = GetGroupsAndDepartmentsForUPN(upn, await _aadCollector.GetGroupMembershipsForGroups());

                ViewBag.Departments = user.Departments;
                ViewBag.Groups = user.Groups;
                ViewBag.ApplicationName = _appSettings.ApplicationName;
                ViewBag.MailAccountDomain = _appSettings.MailAccountDomain;

                return View(user);
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "HomeController");
                TempData["message"] = e.ToString();
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// TODO: move methode
        /// </summary>
        /// <param name="usermail"></param>
        /// <param name="groupData"></param>
        /// <returns></returns>
        private GroupUser GetGroupsAndDepartmentsForUPN(string usermail, GroupListDetails groupData)
        {
            try
            {
                var _filledGroupList = new GroupListDetails();

                var user = new GroupUser();
                user.UPN = usermail.ToLower();

                var usergroups = new List<GroupList>();
                foreach (var groupItem in groupData.GroupList)
                {
                    GroupList grp = new GroupList();
                    grp.Name = groupItem.Name.ToString();
                    if (groupItem.Members.Contains(user.UPN))
                    {
                        grp.Selected = true;
                    }
                    usergroups.Add(grp);
                }
                user.Groups = usergroups;

                var userdepartments = new List<DepartmentList>();
                foreach (var departnmentItem in groupData.DepartmentList)
                {
                    DepartmentList dep = new DepartmentList();
                    dep.Name= departnmentItem.Name.ToString();
                    if (departnmentItem.Members.Contains(user.UPN))
                    {
                        dep.Selected = true;
                    }
                    userdepartments.Add(dep);
                }
                user.Departments = userdepartments;
               

                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "HomeController");
                return null;
            }
        }

       
        public IActionResult Edit2()
        {
            try
            {
                var user = new GroupUser();
                return View(user);
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "HomeController");
                TempData["message"] = e.ToString();
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(GroupUser userToEdit)
        {
            try
            {
                var requestByUser = User.Identity.Name;
                string jobId = await _aadUserCreationService.ProcessUserGroupsEdits(userToEdit, requestByUser);
                ModelState.Clear();
                return Json(jobId);
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "HomeController");
                return Json("Error, please notify administrator.");
            }
        }



        [AllowAnonymous]
        public IActionResult Error()
        {
            ViewBag.ApplicationName = _appSettings.ApplicationName;
            ViewBag.ErrorMessage = "";
            return View();
        }
    }
}
