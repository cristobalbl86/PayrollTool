﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayrollTool.Context;
using PayrollTool.Models;
using static PayrollTool.Helper;

namespace PayrollTool.Controllers
{
    public class AssistanceLogController : Controller
    {
        private readonly PayrollContext _context;

        public AssistanceLogController(PayrollContext context)
        {
            _context = context;
        }

        // GET: AssistanceLog
        [NoDirectAccessAttribute]
        public async Task<IActionResult> Index()
        {
            var payrollContext = _context.AssistanceLog
                .Include(a => a.Employee)
                .Where(a=> a.Date.Date >= DateTime.Today.AddDays(-14).Date);
            return View(await payrollContext.ToListAsync());
        }


        // GET: AssistanceLog/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(
                _context.Employee
                    .Where(e=> !_context.AssistanceLog.Any(a=> a.EmployeeId == e.EmployeeId && a.Date.Date == DateTime.Today.Date)),
                "EmployeeId", "Name");
            return View(new AssistanceLog() { Date = DateTime.Now});
        }

        // POST: AssistanceLog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssistanceLogId,Date,EmployeeId")] AssistanceLog assistanceLog)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    assistanceLog.Date = assistanceLog.Date.Date;
                    _context.Add(assistanceLog);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["EmployeeId"] = new SelectList(_context.Employee, "EmployeeId", "Name", assistanceLog.EmployeeId);
                return View(assistanceLog);
            }
            catch (Exception ex) {
                ViewBag.ErrorMessage = ex.InnerException.Message.ToString();

                var payrollContext = _context.AssistanceLog
                .Include(a => a.Employee)
                .Where(a => a.Date.Date >= DateTime.Today.AddDays(-14).Date);

                return View(nameof(Index), await payrollContext.ToListAsync());
            }
        }

        // GET: AssistanceLog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistanceLog = await _context.AssistanceLog
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.AssistanceLogId == id);
            if (assistanceLog == null)
            {
                return NotFound();
            }

            return View(assistanceLog);
        }

        // POST: AssistanceLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assistanceLog = await _context.AssistanceLog.FindAsync(id);
            _context.AssistanceLog.Remove(assistanceLog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssistanceLogExists(int id)
        {
            return _context.AssistanceLog.Any(e => e.AssistanceLogId == id);
        }
    }
}