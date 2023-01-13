using Furniture.Models;
using Furniture.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Furniture.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ModelContext _context;
        public HomeController(ILogger<HomeController> logger , ModelContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task< IActionResult> Index(decimal? id)
        {

            //var edit = _context.HomePajes.Where(x => x.Id == 1).FirstOrDefault();

            //HttpContext.Session.SetString("email", edit.Email);
            //HttpContext.Session.SetString("phone", edit.Phone);
            //HttpContext.Session.SetString("Address", edit.Address);


            //ViewBag.Email = HttpContext.Session.GetString("email");
            //ViewBag.Phone = HttpContext.Session.GetString("phone");
            //ViewBag.Address = HttpContext.Session.GetString("Address");



            if(id != null)
            {
                var test = _context.Testimonials.Where(x => x.Id.Equals(id)).FirstOrDefault();
                test.Status = "yes";
                _context.Update(test);
                await _context.SaveChangesAsync();
            }




            var model = new DataVM
            {
                categoryProjects = await _context.CategoryProjects.ToListAsync(),
                testimonials = _context.Testimonials.Where(x => x.Status.Equals("yes")).Include(x => x.User).Take(3),
                productProjects = _context.ProductProjects.Take(8),
                homePajes = _context.HomePajes.Where(x => x.Id == 1)
              
                    
                };

            return View( model);

        }

        public IActionResult Contact()
        {
            var edit = _context.HomePajes.Where(x => x.Id == 1).FirstOrDefault();

            HttpContext.Session.SetString("email", edit.Email);
            HttpContext.Session.SetString("phone", edit.Phone);
            HttpContext.Session.SetString("Address", edit.Address);


            ViewBag.Email = HttpContext.Session.GetString("email");
            ViewBag.Phone = HttpContext.Session.GetString("phone");
            ViewBag.Address = HttpContext.Session.GetString("Address");
            return View();
        }

        [HttpPost]
        public IActionResult Contact([Bind("Id, Email,Namee ,Message")] ContactU contact)
        {
            _context.ContactUs.Add(contact);
            _context.SaveChanges();
            
            return View();
        }

        public IActionResult AboutUs()
        {
            var edit = _context.HomePajes.Where(x => x.Id == 1).FirstOrDefault();

            HttpContext.Session.SetString("email", edit.Email);
            HttpContext.Session.SetString("phone", edit.Phone);
            HttpContext.Session.SetString("Address", edit.Address);


            ViewBag.Email = HttpContext.Session.GetString("email");
            ViewBag.Phone = HttpContext.Session.GetString("phone");
            ViewBag.Address = HttpContext.Session.GetString("Address");
            var model = new DataVM
            {

                testimonials = _context.Testimonials.Where(x => x.Status.Equals("yes")).Include(x => x.User),
                productProjects = _context.ProductProjects.Take(8),
                AboutUs = _context.AboutUs.Where(x=>x.Id == 1),
               userAccounts= _context.UserAccounts.Where(x => x.RoleId == 1)
        };

           
            return View(model);
        }

        public async Task<IActionResult> Products(int Id)
        {
            var edit = _context.HomePajes.Where(x => x.Id == 1).FirstOrDefault();

            HttpContext.Session.SetString("email", edit.Email);
            HttpContext.Session.SetString("phone", edit.Phone);
            HttpContext.Session.SetString("Address", edit.Address);


            ViewBag.Email = HttpContext.Session.GetString("email");
            ViewBag.Phone = HttpContext.Session.GetString("phone");
            ViewBag.Address = HttpContext.Session.GetString("Address");
            var product = await _context.ProductProjects.Where(x => x.CategoryId == Id).Include(p => p.Category).ToListAsync();
            return View(product);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Testiomnils()
        {
            var edit = _context.HomePajes.Where(x => x.Id == 1).FirstOrDefault();

            HttpContext.Session.SetString("email", edit.Email);
            HttpContext.Session.SetString("phone", edit.Phone);
            HttpContext.Session.SetString("Address", edit.Address);


            ViewBag.Email = HttpContext.Session.GetString("email");
            ViewBag.Phone = HttpContext.Session.GetString("phone");
            ViewBag.Address = HttpContext.Session.GetString("Address");
            var customers = _context.Testimonials.Include(x => x.User).ToList();
            return View(customers);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }
}
