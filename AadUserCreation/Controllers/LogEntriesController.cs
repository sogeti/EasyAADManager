using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AadUserCreation.Models;
using AadUserCreation.Data.Interfaces;
using AadUserCreation.Business.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace AadUserCreation.Controllers
{
    [Authorize]
    public class LogEntriesController : Controller
    {
        private readonly ILogTable _logTable;
        private readonly AppSettings _appSettings;
        private readonly IBloblogReader _bloblogReader;
        private readonly ILogger _logger;

        public LogEntriesController(ILogTable logTable, IOptions<AppSettings> appSettings, IBloblogReader bloblogReader, ILogger<LogEntriesController> logger)
        {
            _logTable = logTable;
            _appSettings = appSettings.Value;
            _bloblogReader = bloblogReader;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.LogList = await _logTable.GetAllLogs();
                ViewBag.ApplicationName = _appSettings.ApplicationName;
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "LogEntriesController");
                TempData["message"] = e.ToString();
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                ViewBag.ApplicationName = _appSettings.ApplicationName;

                var logList = await _logTable.GetAllLogs();
                var log = logList.Where(a => a.RowKey == id).FirstOrDefault();

                var logTekst = await _bloblogReader.GetLog(id);

                log.LogText = logTekst;
                if (log == null)
                {
                    return View();
                }
                return View(log);
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "LogEntriesController");
                TempData["message"] = e.ToString();
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
