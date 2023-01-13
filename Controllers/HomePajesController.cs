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
    public class HomePajesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public HomePajesController(ModelContext context , IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _context.HomePajes.ToListAsync());
        }


        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homePaje = await _context.HomePajes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homePaje == null)
            {
                return NotFound();
            }

            return View(homePaje);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,ImageFile,Logo,Paragraph,Email,Phone,Address,Textt")] HomePaje homePaje)
        {
            if (ModelState.IsValid)
            {

                if (homePaje.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + homePaje.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/image/", imageName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await homePaje.ImageFile.CopyToAsync(fileStream);
                    }
                    homePaje.Image = imageName;
                }
                _context.Add(homePaje);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(homePaje);
        }

        // GET: HomePajes/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homePaje = await _context.HomePajes.FindAsync(id);
            if (homePaje == null)
            {
                return NotFound();
            }
            return View(homePaje);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Image,ImageFile,Logo,Paragraph,Email,Phone,Address,Textt")] HomePaje homePaje)
        {
            if (id != homePaje.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (homePaje.ImageFile != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string imageName = Guid.NewGuid().ToString() + "_" + homePaje.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/image/", imageName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await homePaje.ImageFile.CopyToAsync(fileStream);
                        }
                        homePaje.Image = imageName;
                    }
                    _context.Update(homePaje);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomePajeExists(homePaje.Id))
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
            return View(homePaje);
        }

        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homePaje = await _context.HomePajes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homePaje == null)
            {
                return NotFound();
            }

            return View(homePaje);
        }

 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var homePaje = await _context.HomePajes.FindAsync(id);
            _context.HomePajes.Remove(homePaje);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HomePajeExists(decimal id)
        {
            return _context.HomePajes.Any(e => e.Id == id);
        }
    }
}
