using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SurveyJSAsFormLibrary.Models;
using SurveyJSAsFormLibrary.DomainModels;

namespace SurveyJSAsFormLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new FormListModel());
        }
        public IActionResult FormResponse(string formName, string id)
        {
            return View(new FormResponseModel() { FormName = formName, Id = id });
        }
        public IActionResult EditForm(string formName, bool isAdmin = false)
        {
            string formTitle = DomainModelList.GetTitleByFormName(formName);
            return View(new EditFormModel() { FormTitle = formTitle, IsAdmin = isAdmin });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
