using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            DateTime Startdate= DateTime.Now.AddDays(-30);
            DateTime Enddate= DateTime.Today.AddDays(1).AddTicks(-1);

            List<Transaction> selectTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= Startdate && y.Date <= Enddate)
                .ToListAsync();

            // total income (guard Category != null)
            int totalincome = selectTransactions
                .Where(t => t.Category != null && t.Category.Type == "Income")
                .Sum(t => t.Amount);
            ViewBag.TotalIncome = totalincome.ToString("C0");

            // total expense (guard Category != null)
            int totalexpense = selectTransactions
                .Where(t => t.Category != null && t.Category.Type == "Expense")
                .Sum(t => t.Amount);
            ViewBag.TotalExpense = totalexpense.ToString("C0");

            // net balance
            int netbalance = totalincome - totalexpense;
            ViewBag.NetBalance = netbalance.ToString("C0");


            // --- Pie Chart Data - Category Breakdown ---
            var categoryBreakdown = selectTransactions
                .Where(t => t.Category.Type == "Expense")
                .GroupBy(t => t.Category.Title)
                .Select(g => new
                {
                    Category = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .ToList();
            // Convert to JSON for Chart.js
            ViewBag.CategoryLabels = System.Text.Json.JsonSerializer.Serialize(
                categoryBreakdown.Select(x => x.Category).ToArray()
            );
            ViewBag.CategoryAmounts = System.Text.Json.JsonSerializer.Serialize(
                categoryBreakdown.Select(x => x.Amount).ToArray()
            );

            // --- Line Chart Data - Income vs Expense (Last 7 Days) ---
            List<string> last7Days = new List<string>();
            List<decimal> incomeData = new List<decimal>();
            List<decimal> expenseData = new List<decimal>();

            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                last7Days.Add(date.ToString("MMM dd"));

                var dayIncome = selectTransactions
                    .Where(t => t.Date.Date == date && t.Category.Type == "Income")
                    .Sum(t => t.Amount);
                incomeData.Add(dayIncome);

                var dayExpense = selectTransactions
                    .Where(t => t.Date.Date == date && t.Category.Type == "Expense")
                    .Sum(t => t.Amount);
                expenseData.Add(dayExpense);
            }

            ViewBag.Last7Days = System.Text.Json.JsonSerializer.Serialize(last7Days);
            ViewBag.IncomeData = System.Text.Json.JsonSerializer.Serialize(incomeData);
            ViewBag.ExpenseData = System.Text.Json.JsonSerializer.Serialize(expenseData);

            // --- Recent 5 Transactions ---
            ViewBag.RecentTransactions = await _context.Transactions
                .Include(t => t.Category)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToListAsync();
            return View();
        }
    }
}
