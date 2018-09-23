using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OutageDataLayer.QueryModels;
using OutageDataLayer.QueryResultModels;
using OutageDataLayer.DataLayer;
using Microsoft.Extensions.Configuration;

namespace OutageWebApp.Controllers
{
    public class OutageController : Controller
    {
        private IConfiguration Configuration { get; }
        public OutageController(IConfiguration configuration)
        {
           Configuration = configuration;
        }

        public IActionResult Index(OutageQuery outageQuery)
        {
            ViewData["Message"] = "Your application outage page.";
            if (!ModelState.IsValid)
            {
                return View(new List<OutageQueryResult>());
            }
            string connStr = Configuration["ConnectionStrings:DefaultConnection"];
            OutageFetcher outageFetcher = new OutageFetcher();
                       
            List<OutageQueryResult> queryResults = outageFetcher.FetchOutages(outageQuery, connStr);

            return View(queryResults);
        }

    }
}
