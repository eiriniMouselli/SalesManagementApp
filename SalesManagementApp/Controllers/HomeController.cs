using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesManagementApp.Data;
using SalesManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SalesManagementApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SalesManagementContext _context;

        public HomeController(ILogger<HomeController> logger, SalesManagementContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ComputeSalesAndEmployees();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void ComputeSalesAndEmployees()
        {
            List<SalesPerson> employees = _context.SalesPeople.Include(sp => sp.Sales).ToList();
            int totalNumEmployees = employees.Count();
            decimal totalSales = 0;
            foreach(SalesPerson employee in employees)
            {
                foreach(Sale sale in employee.Sales)
                    totalSales += sale.Amount;  
            }
           
            ViewBag.TotalSales = totalSales;
            ViewBag.TotalEmployees = totalNumEmployees;
        }
    }
}
