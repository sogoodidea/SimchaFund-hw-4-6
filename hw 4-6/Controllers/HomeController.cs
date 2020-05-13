using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using hw_4_6.Models;
using hw_4_6.data;

namespace hw_4_6.Controllers
{
    public class HomeController : Controller
    {
        private SimchaFundMgr mgr = new SimchaFundMgr();
        public IActionResult Index()
        {
            var vm = new SimchaViewModel
            {
                Simchos = mgr.GetSimchas(),
                TotalContributors = mgr.GetTotalContributors(),
                Message = (string)TempData["message"]
            };

            return View(vm);
        }

        public IActionResult Contributors()
        {
            var vm = new ContributorViewModel
            {
                Contributors = mgr.GetContributors(),
                Message = (string)TempData["message"]
            };
            return View(vm);
        }
        public IActionResult Contributions(int SimchaId)
        {
            ContributionViewModel vm = new ContributionViewModel
            {
                Simcha = mgr.GetSimchaById(SimchaId),
                Contributors = mgr.GetContributors(),
                IdsContributed = mgr.GetIdsThatContributed(SimchaId)
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddSimcha(Simcha simcha)
        {
            mgr.AddSimcha(simcha);
            TempData["message"] = "Simcha added successfully!";
            return Redirect("/home/index");
        }

        [HttpPost]
        public IActionResult MakeContribution(List<Contributor> contrs, int simchaId)
        {
            contrs = contrs.Where(con => con.WantsToContribute).ToList();
            mgr.AddContributions(contrs, simchaId);
            TempData["message"] = "Contributions updated successfully!";
            return Redirect("/home/index");
        }
        public IActionResult ShowHistory(int contrId)
        {
            return View(mgr.GetHistoryById(contrId));
        }

        [HttpPost]
        public IActionResult AddDeposit(Contributor contr)
        {
            mgr.AddDeposit(contr);
            TempData["message"] = "Deposit added successfully!";
            return Redirect("/home/contributors");
        }

        [HttpPost]
        public IActionResult NewContributor(Contributor contr)
        {
            mgr.AddContributor(contr);
            TempData["message"] = "Contributor added successfully!";
            return Redirect("/home/contributors");
        }

        [HttpPost]
        public IActionResult UpdateContributor(Contributor contributor)
        {
            mgr.UpdateContributor(contributor);
            TempData["message"] = "Contributor updated successfully!";
            return Redirect("/home/contributors");
        }
    }
}
