using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AadUserCreation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Options;
using AadUserCreation.Business;

namespace AadUserCreation.Controllers
{

    /// <summary>
    /// WIP to create users based on CSV upload 
    /// 
    /// </summary>
    /// 

    [Authorize]
    public class CsvUploadController : Controller
    {
        private readonly AppSettings _appSettings;
        private IHostingEnvironment _environment;

        public CsvUploadController(IHostingEnvironment environment, IOptions<AppSettings> appSettings)
        {
            _environment = environment;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index(ICollection<IFormFile> files)
        {

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");

            var webUrl = _appSettings.WebHookUrlCreateUsersRunbook;

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        //var createUser = new UserCreationSerivice();
                        //createUser.ProcessCsvData();
                    }
                }
            }
            return View();
        }

    }
}