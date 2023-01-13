using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Furniture.Models;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Furniture.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AboutUsController(ModelContext context , IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: AboutUs
        public async Task<IActionResult> Index()
        {
            return View(await _context.AboutUs.ToListAsync());
        }

        // GET: AboutUs/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aboutU = await _context.AboutUs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aboutU == null)
            {
                return NotFound();
            }

            return View(aboutU);
        }

        // GET: AboutUs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AboutUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,ImageFile,Pargraph1,Pargraph2,Pargraph3")] AboutU aboutU)
        {
            if (ModelState.IsValid)
            {

                if (aboutU.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + aboutU.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/image/", imageName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await aboutU.ImageFile.CopyToAsync(fileStream);
                    }
                    aboutU.Image = imageName;
                }

                _context.Add(aboutU);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aboutU);
        }

        // GET: AboutUs/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aboutU = await _context.AboutUs.FindAsync(id);
            if (aboutU == null)
            {
                return NotFound();
            }
            return View(aboutU);
        }

        // POST: AboutUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Image,ImageFile,Pargraph1,Pargraph2,Pargraph3")] AboutU aboutU)
        {
            if (id != aboutU.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    if (aboutU.ImageFile != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string imageName = Guid.NewGuid().ToString() + "_" + aboutU.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/image/", imageName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await aboutU.ImageFile.CopyToAsync(fileStream);
                        }
                        aboutU.Image = imageName;
                    }

                    _context.Update(aboutU);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutUExists(aboutU.Id))
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
            return View(aboutU);
        }

        // GET: AboutUs/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aboutU = await _context.AboutUs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aboutU == null)
            {
                return NotFound();
            }

            return View(aboutU);
        }

        // POST: AboutUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var aboutU = await _context.AboutUs.FindAsync(id);
            _context.AboutUs.Remove(aboutU);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AboutUExists(decimal id)
        {
            return _context.AboutUs.Any(e => e.Id == id);
        }
    }
}
