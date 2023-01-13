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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Furniture.Controllers
{

    public class CategoryProjectsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CategoryProjectsController(ModelContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: CategoryProjects
        public async Task<IActionResult> Index()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            return View(await _context.CategoryProjects.ToListAsync());
        }

        // GET: CategoryProjects/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            if (id == null)
            {
                return NotFound();
            }

            var categoryProject = await _context.CategoryProjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryProject == null)
            {
                return NotFound();
            }

            return View(categoryProject);
        }

        // GET: CategoryProjects/Create
        public IActionResult Create()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            return View();
        }

        // POST: CategoryProjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Categoryname,ImagePath,ImageFile")] CategoryProject categoryProject)
        {
            if (ModelState.IsValid)

            { 

                if(categoryProject.ImageFile !=null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + categoryProject.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/image/", imageName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await categoryProject.ImageFile.CopyToAsync(fileStream);
                    }
                    categoryProject.ImagePath = imageName;
                }
                TempData["success"] = "Category created successfully";


                _context.Add(categoryProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoryProject);
        }

        // GET: CategoryProjects/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            if (id == null)
            {
                return NotFound();
            }

            var categoryProject = await _context.CategoryProjects.FindAsync(id);
            if (categoryProject == null)
            {
                return NotFound();
            }
            return View(categoryProject);
        }

        // POST: CategoryProjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Categoryname,ImagePath,ImageFile")] CategoryProject categoryProject)
        {
            if (id != categoryProject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (categoryProject.ImageFile != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string imageName = Guid.NewGuid().ToString() + "_" + categoryProject.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/image/", imageName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await categoryProject.ImageFile.CopyToAsync(fileStream);
                        }
                        categoryProject.ImagePath = imageName;
                    }
                    _context.Update(categoryProject);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Category updated successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryProjectExists(categoryProject.Id))
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
            return View(categoryProject);
        }

        // GET: CategoryProjects/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            if (id == null)
            {
                return NotFound();
            }

            var categoryProject = await _context.CategoryProjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryProject == null)
            {
                return NotFound();
            }

            return View(categoryProject);
        }

        // POST: CategoryProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var categoryProject = await _context.CategoryProjects.FindAsync(id);
            _context.CategoryProjects.Remove(categoryProject);
            await _context.SaveChangesAsync();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryProjectExists(decimal id)
        {
            return _context.CategoryProjects.Any(e => e.Id == id);
        }
    }
}
