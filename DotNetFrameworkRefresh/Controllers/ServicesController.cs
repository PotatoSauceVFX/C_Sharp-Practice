﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotNetFrameworkRefresh.Data;
using DotNetFrameworkRefresh.Models;

namespace DotNetFrameworkRefresh.Controllers
{
    public class ServicesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Services
        public async Task<ActionResult> Index()
        {
            var services = db.Services.Include(s => s.Car);
            return View(await services.ToListAsync());
        }

        // GET: Services/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = await db.Services.FindAsync(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // GET: Services/Create
        public async Task<ActionResult> Create(int? carId)
        {
            if(!carId.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var car = await db.Cars.FindAsync(carId);

            if(car == null)
            {
                return HttpNotFound();
            }

            return View(new Service() { CarId = car.Id, Car = car });
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CarId,ServiceDateTime,ServiceHours,ServiceNotes")] Service service)
        {
            if (ModelState.IsValid)
            {
                db.Services.Add(service);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Cars", new { id = service.CarId });
            }

            if (!service.CarId.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var car = await db.Cars.FindAsync(service.CarId);

            if (car == null)
            {
                return HttpNotFound();
            }

            service.Car = car;

            return View(service);
        }

        // GET: Services/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = await db.Services.FindAsync(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            ViewBag.CarId = new SelectList(db.Cars, "Id", "IdentificationNumber", service.CarId);
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CarId,ServiceDateTime,ServiceHours,ServiceNotes")] Service service)
        {
            if (ModelState.IsValid)
            {
                db.Entry(service).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Cars", new { id = service.CarId });
            }
            ViewBag.CarId = new SelectList(db.Cars, "Id", "IdentificationNumber", service.CarId);
            return View(service);
        }

        // GET: Services/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = await db.Services.FindAsync(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Service service = await db.Services.FindAsync(id);

            if(service == null)
            {
                return HttpNotFound();
            }

            var carId = service.CarId;

            db.Services.Remove(service);
            await db.SaveChangesAsync();
            return RedirectToAction("Details", "Cars", new { id = carId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
