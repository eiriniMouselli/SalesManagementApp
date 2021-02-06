using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SalesManagementApp.Data;
using SalesManagementApp.Models;

namespace SalesManagementApp.Controllers
{
    public class SalesController : Controller
    {
        private readonly SalesManagementContext _context;

        public SalesController(SalesManagementContext context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index(string sortOrder)
        {       
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["AmountSortParm"] = sortOrder == "Amount" ? "amount_desc" : "Amount";
            var sales = from s in _context.Sales select s;

            switch (sortOrder)
            {
                case "Amount":
                    sales = sales.OrderBy(s => s.Amount);
                    break;
                case "amount_desc":
                    sales = sales.OrderByDescending(s => s.Amount);
                    break;
                case "name_desc":
                    sales = sales.OrderByDescending(s => s.SalesPersonFullName);
                    break;
                case "Date":
                    sales = sales.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    sales = sales.OrderByDescending(s => s.Date);
                    break;
                default:
                    sales = sales.OrderBy(s => s.SalesEmployee);
                    break;
            }

            return View(await sales.AsNoTracking().ToListAsync());
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.SalesEmployee)
                .FirstOrDefaultAsync(m => m.SaleId == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            ViewData["SalesPersonFullName"] = new SelectList(_context.SalesPeople, "FullName", "FullName");
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SaleId,SalesPersonFullName,Amount,Date")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                SalesPerson employee = await _context.SalesPeople.Include(sp => sp.Sales)
                    .FirstOrDefaultAsync(sp => sp.FullName == sale.SalesPersonFullName);
                sale.SalesEmployee = employee;
                _context.Add(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SalesPersonFullName"] = new SelectList(_context.SalesPeople, "FullName", "FullName", sale.SalesPersonFullName);
            return View(sale);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            ViewData["SalesPersonFullName"] = new SelectList(_context.SalesPeople, "FullName", "FullName", sale.SalesPersonFullName);
            return View(sale);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SaleId,SalesPersonFullName,Amount,Date")] Sale sale)
        {
            if (id != sale.SaleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                     SalesPerson employee = await _context.SalesPeople
                      .FirstOrDefaultAsync(sp => sp.FullName == sale.SalesPersonFullName);

                     sale.SalesEmployee = employee;
                  
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.SaleId))
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
            ViewData["SalesPersonFullName"] = new SelectList(_context.SalesPeople, "FullName", "FullName", sale.SalesPersonFullName);
            return View(sale);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.SalesEmployee)
                .FirstOrDefaultAsync(m => m.SaleId == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.FindAsync(id);

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.SaleId == id);
        }
                
    }
}
