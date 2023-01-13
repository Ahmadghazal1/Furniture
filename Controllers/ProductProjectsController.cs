using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Furniture.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Furniture.Controllers
{

    public class ProductProjectsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductProjectsController(ModelContext context , IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: ProductProjects
        public async Task<IActionResult> Index()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            var modelContext = _context.ProductProjects.Include(p => p.Category);
            return View(await modelContext.ToListAsync());
        }

        // GET: ProductProjects/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            if (id == null)
            {
                return NotFound();
            }

            var productProject = await _context.ProductProjects
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productProject == null)
            {
                return NotFound();
            }

            return View(productProject);
        }

        // GET: ProductProjects/Create
        public IActionResult Create()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            ViewData["CategoryId"] = new SelectList(_context.CategoryProjects, "Id", "Categoryname");
            return View();
        }

        // POST: ProductProjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductName,ImagePath,Price,Valuee,CategoryId,ImageFile")] ProductProject productProject)
        {
            if (ModelState.IsValid)
            {

                if (productProject.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + productProject.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/image/", imageName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await productProject.ImageFile.CopyToAsync(fileStream);
                    }
                    productProject.ImagePath = imageName;
                }





                _context.Add(productProject);
                await _context.SaveChangesAsync();
                TempData["success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.CategoryProjects, "Id", "Categoryname", productProject.CategoryId);
            return View(productProject);
        }

        // GET: ProductProjects/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            if (id == null)
            {
                return NotFound();
            }

            var productProject = await _context.ProductProjects.FindAsync(id);
            if (productProject == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.CategoryProjects, "Id", "Categoryname", productProject.CategoryId);
            return View(productProject);
        }

        // POST: ProductProjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,ProductName,ImagePath,Price,Valuee,CategoryId,ImageFile")] ProductProject productProject)
        {
            if (id != productProject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (productProject.ImageFile != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string imageName = Guid.NewGuid().ToString() + "_" + productProject.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/image/", imageName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await productProject.ImageFile.CopyToAsync(fileStream);
                        }
                        productProject.ImagePath = imageName;
                    }




                    _context.Update(productProject);
                    TempData["success"] = "Product update successfully";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductProjectExists(productProject.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.CategoryProjects, "Id", "Categoryname", productProject.CategoryId);
            return View(productProject);
        }

        // GET: ProductProjects/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            if (id == null)
            {
                return NotFound();
            }

            var productProject = await _context.ProductProjects
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productProject == null)
            {
                return NotFound();
            }

            return View(productProject);
        }

        // POST: ProductProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var productProject = await _context.ProductProjects.FindAsync(id);
            _context.ProductProjects.Remove(productProject);
            await _context.SaveChangesAsync();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool ProductProjectExists(decimal id)
        {
            return _context.ProductProjects.Any(e => e.Id == id);
        }
    }
}
