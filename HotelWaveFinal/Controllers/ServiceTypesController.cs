using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelWaveFinal.DB;
using HotelWaveFinal.Models;
using Microsoft.AspNetCore.Authorization;

namespace HotelWaveFinal.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class ServiceTypesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public ServiceTypesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: ServiceTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiceTypes.ToListAsync());
        }

        // GET: ServiceTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceType = await _context.ServiceTypes
                .FirstOrDefaultAsync(m => m.ServiceTypeId == id);
            if (serviceType == null)
            {
                return NotFound();
            }

            return View(serviceType);
        }

        // GET: ServiceTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiceTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceTypeId,TypeName,Description,BasePrice")] ServiceType serviceType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceType);
                await _context.SaveChangesAsync();
                TempData["success"] = "Service Type Added Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(serviceType);
        }

        // GET: ServiceTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceType = await _context.ServiceTypes.FindAsync(id);
            if (serviceType == null)
            {
                return NotFound();
            }
            return View(serviceType);
        }

        // POST: ServiceTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceTypeId,TypeName,Description,BasePrice")] ServiceType serviceType)
        {
            if (id != serviceType.ServiceTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceType);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Service Type Updated Successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceTypeExists(serviceType.ServiceTypeId))
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
            return View(serviceType);
        }

        // GET: ServiceTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceType = await _context.ServiceTypes
                .FirstOrDefaultAsync(m => m.ServiceTypeId == id);
            if (serviceType == null)
            {
                return NotFound();
            }

            return View(serviceType);
        }

        // POST: ServiceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceType = await _context.ServiceTypes.FindAsync(id);
            if (serviceType != null)
            {
                _context.ServiceTypes.Remove(serviceType);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Serviece Type Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceTypeExists(int id)
        {
            return _context.ServiceTypes.Any(e => e.ServiceTypeId == id);
        }
    }
}
