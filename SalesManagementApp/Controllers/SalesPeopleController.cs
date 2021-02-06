using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesManagementApp.Data;
using SalesManagementApp.Models;

namespace SalesManagementApp.Controllers
{
    public class SalesPeopleController : Controller
    {
        private readonly SalesManagementContext _context;

        public SalesPeopleController(SalesManagementContext context)
        {
            _context = context;
        }

        // GET: SalesPeople
        public async Task<IActionResult> Index(string sortOrder)
        {
            ComputeCommissions();
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CommissionSortParm"] = sortOrder == "Com" ? "com_desc" : "Com";
            var salesPeople = from sp in _context.SalesPeople select sp;
            switch (sortOrder)
            {
               case "name_desc":
                    salesPeople = salesPeople.OrderByDescending(sp => sp.FullName);
                    break;
                case "Com":
                    salesPeople = salesPeople.OrderBy(sp => sp.Commission);
                    break;
                case "com_desc":
                    salesPeople = salesPeople.OrderByDescending(sp => sp.Commission);
                    break;
                default:
                    salesPeople = salesPeople.OrderBy(sp => sp.FullName);
                    break;
            }
            return View(await salesPeople.AsNoTracking().ToListAsync());
        }

        // GET: SalesPeople/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesPerson = await _context.SalesPeople
                .Include(sp => sp.Sales)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesPerson == null)
            {
                return NotFound();
            }
            ComputeMonthlyRecap(salesPerson);
            return View(salesPerson);
        }

        // GET: SalesPeople/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SalesPeople/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName")] SalesPerson salesPerson)
        {
            if (ModelState.IsValid)
            {
                salesPerson.Sales = new List<Sale>();
                salesPerson.Commission = 0;
                _context.Add(salesPerson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salesPerson);
        }

        // GET: SalesPeople/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesPerson = await _context.SalesPeople.FindAsync(id);
            if (salesPerson == null)
            {
                return NotFound();
            }
            return View(salesPerson);
        }

        // POST: SalesPeople/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,")] SalesPerson salesPerson)
        {
            if (id != salesPerson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesPerson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesPersonExists(salesPerson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(salesPerson);
        }

        // GET: SalesPeople/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesPerson = await _context.SalesPeople
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesPerson == null)
            {
                return NotFound();
            }

            return View(salesPerson);
        }

        // POST: SalesPeople/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salesPerson = await _context.SalesPeople.FindAsync(id);
            _context.SalesPeople.Remove(salesPerson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesPersonExists(int id)
        {
            return _context.SalesPeople.Any(e => e.Id == id);
        }

        private void ComputeCommissions()
        {
            List<SalesPerson> employees = _context.SalesPeople.Include(sp => sp.Sales).ToList();

            foreach (SalesPerson employee in employees){
                employee.Commission = 0;
                foreach (Sale sale in employee.Sales)
                {
                    employee.Commission += sale.Amount * (decimal)0.1;
                }
            }
            _context.SaveChanges();
        }

        private void ComputeMonthlyRecap(SalesPerson employee)
        {
            Dictionary<string, List<decimal>> recap = new Dictionary<string, List<decimal>>();
            List<decimal> totalSales = new List<decimal>(new decimal[12]), totalCommission = new List<decimal>(new decimal[12]);
            List<string> months = new List<string>() { "January", "February", "March", "April", "May", "June", 
                                                       "July", "August", "September", "October", "November", "December" };
            foreach (Sale sale in employee.Sales)
            {
              if(sale.Date.Year == DateTime.Today.Year)
                {
                    totalSales[sale.Date.Month-1] += sale.Amount;
                    totalCommission[sale.Date.Month-1] += sale.Amount * (decimal)0.1;
                }
            }
            for(int i = 0; i < 12; i++)
            {
                List<decimal> sums = new List<decimal>() { totalSales[i], totalCommission[i] } ;
                recap.Add(months[i], sums);
            }

            ViewBag.RecapDict = recap;
        }
    }
}
