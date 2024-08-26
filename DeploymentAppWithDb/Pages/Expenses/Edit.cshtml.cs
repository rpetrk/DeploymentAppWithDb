using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DeploymentAppWithDb.Data;
using DeploymentAppWithDb.Models;

namespace DeploymentAppWithDb.Pages.Expenses
{
    public class EditModel : PageModel
    {
        private readonly DeploymentAppWithDb.Data.ProjectContext _context;

        public EditModel(DeploymentAppWithDb.Data.ProjectContext context)
        {
            _context = context;
        }


        [BindProperty]
        public Expense Expense { get; set; } = default!;

        public SelectList ExpenseTypeList { get; set; }

        public void PopulateExpenseTypeDropDownList(object selectedExpenseType = null)
        {
            var expenseTypes = _context.ExpenseTypes;
            ExpenseTypeList = new SelectList(expenseTypes, "ID", "Name", selectedExpenseType);
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Expenses == null)
            {
                return NotFound();
            }

            var expense =  await _context.Expenses.FirstOrDefaultAsync(m => m.ID == id);
            if (expense == null)
            {
                return NotFound();
            }
            Expense = expense;
            //ViewData["ExpenseType"] = new SelectList(_context.ExpenseTypes, "ID", "Name");
            
            PopulateExpenseTypeDropDownList(Expense.ExpenseTypeID);

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            var modifiedExpense = new Expense();

            if (await TryUpdateModelAsync<Expense>(
                modifiedExpense,
                "expense",
                s => s.ID, s => s.DateIncurred, s => s.Description, s => s.Location, s => s.Price, s => s.ExpenseTypeID, s => s.UserID))
            {
            }

                _context.Expenses.Attach(modifiedExpense).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(Expense.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ExpenseExists(int id)
        {
          return _context.Expenses.Any(e => e.ID == id);
        }
    }
}
