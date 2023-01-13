using Furniture.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Furniture.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AdminController(ModelContext context , IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            int numberOfCustomer = _context.UserAccounts.Count();
            ViewBag.numberOfUser = numberOfCustomer;

            //Catgory
            int numberCategory = _context.CategoryProjects.Count();
            ViewBag.numberOfCategory = numberCategory;

            //Product 
            int numberOfProduct = _context.ProductProjects.Count();
            ViewBag.numberOfProduct = numberOfProduct;

            //TESTIMONIAL

            int numberoftestmonial = _context.Testimonials.Count();
            ViewBag.testimonial = numberoftestmonial;

            //name 

            string name = HttpContext.Session.GetString("FullName");
            ViewBag.name = name;

            int Sales = (int)_context.Payments.Sum(x => x.Amount);
            ViewBag.sales = Sales;


            var employee = _context.UserAccounts.Where(x => x.RoleId == 1);


            return View(employee);
        }
        public JsonResult GetPieChartJSONnnn()
        {
            var Orange = _context.UserAccounts.Where(p => p.Phone.StartsWith("077")).Count();
            var Umniah = _context.UserAccounts.Where(p => p.Phone.StartsWith("078")).Count();
            var Zain = _context.UserAccounts.Where(p => p.Phone.StartsWith("079")).Count();
            List<Chart> list = new List<Chart>();
            list.Add(new Chart { CategoryName = "Orange", PostCount = Orange });
            list.Add(new Chart { CategoryName = "Umniah", PostCount = Umniah });
            list.Add(new Chart { CategoryName = "Zain", PostCount = Zain });
            return Json(new { JSONList = list });
        }

        public JsonResult GetPieChartJSONnnnn()
        {
            var LivingRoom = _context.ProductOrders.Where(p => p.Product.Category.Id == 23).Count();
            var BathroomFurniture = _context.ProductOrders.Where(p => p.Product.Category.Id == 63).Count();
            var KidsFurniture = _context.ProductOrders.Where(p => p.Product.Category.Id == 64).Count();
            var DiningRoom = _context.ProductOrders.Where(p => p.Product.Category.Id == 65).Count();

            List<Chart> list = new List<Chart>();
            list.Add(new Chart { CategoryName = "Living Room Furniture", PostCount = LivingRoom });
            list.Add(new Chart { CategoryName = "Bathroom Furniture", PostCount = BathroomFurniture });
            list.Add(new Chart { CategoryName = "Kids & Toddler Furniture", PostCount = KidsFurniture });
            list.Add(new Chart { CategoryName = "Dining Room Furniture ", PostCount = DiningRoom });

            return Json(new { JSONList = list });
        }


        [HttpGet]
        public async Task<IActionResult> ShowProfile()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            decimal currentUser = Convert.ToDecimal(HttpContext.Session.GetInt32("id"));
            string username = HttpContext.Session.GetString("username");
            string password = HttpContext.Session.GetString("password");

            ViewBag.username = username;
            ViewBag.password = password;


            var user =  _context.UserAccounts.Where(x => x.Id.Equals(currentUser)).Include(x => x.Logins).FirstOrDefault();
          ViewData["RoleId"] = new SelectList(_context.RoleeUsers, "Id", "Rolename", user.RoleId);
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            decimal number = Convert.ToDecimal(HttpContext.Session.GetInt32("id"));

            string username = HttpContext.Session.GetString("username");
            string password = HttpContext.Session.GetString("password");

            ViewBag.username = username;
            ViewBag.password = password;

            //  string fullname = HttpContext.Session.GetString("FullName");
            //   string Email = HttpContext.Session.GetString("Email");

            var user = _context.UserAccounts.Where(x => x.Id.Equals(number)).Include(x=>x.Logins).SingleOrDefault();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(decimal id, [Bind("Id,Fullname,Phone,Email,RoleId,ImageFile")] UserAccount userAccount, string password, string username)
        {
            if (ModelState.IsValid)
            {

                if (userAccount.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string imageName = Guid.NewGuid().ToString() + "_" + userAccount.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/image/", imageName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await userAccount.ImageFile.CopyToAsync(fileStream);
                    }
                    userAccount.ImagePath = imageName;
                }

                userAccount.RoleId = 1;
                _context.Update(userAccount);
                await _context.SaveChangesAsync();

                var user = _context.Logins.Where(x => x.UserId.Equals(userAccount.Id)).FirstOrDefault();
                user.Passwordd = password;
                user.Username = username;

                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("ShowProfile", "Admin");
                }

            return View(userAccount);
        }

        public async Task<IActionResult> ShowTestimonial()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            var comments = _context.Testimonials.Where(x => x.Status.Equals("no")).Include(x => x.User);
            return View(comments);
        }
       

        [HttpGet]
        
        public async Task<IActionResult> DeleteTestimonial(decimal id)
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            if (id == null)
            {
                return NotFound();
            }

            var comments = await _context.Testimonials.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();

            if (comments == null)
            {
                return NotFound();
            }
          

            return View(comments);
        }

        [HttpPost, ActionName("DeleteTestimonial")]
        public async Task<IActionResult> ConfirmDeleteTestimonial(decimal id)
        {
            var comments =await _context.Testimonials.FindAsync(id);
            _context.Testimonials.Remove(comments);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowTestimonial");

        }


        [HttpPost]
        public async Task<IActionResult> ApproveTestimonial(decimal id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var comment = await _context.Testimonials.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            comment.Status = "yes";
            return View();

        }

        [HttpGet]
        public IActionResult Search()
        {

            ViewBag.image = HttpContext.Session.GetString("image");

            var modelContext = _context.Payments.Include(x => x.User);
            return View(modelContext);
        }

        [HttpPost]
        public async Task<IActionResult> Search(DateTime? startDate, DateTime? endDate)
        {
            var modelContext = _context.Payments.Include(x => x.User);

            if (startDate == null && endDate == null)
            {
                return View(await modelContext.ToListAsync());
            }
            else if (startDate == null && endDate != null)
            {
                var result = await modelContext.Where(x => x.PayDate.Value.Date <= endDate).ToListAsync();

                return View(result);
            }
            else if (startDate != null && endDate == null)
            {
                var result = await modelContext.Where(x => x.PayDate.Value.Date >= startDate).ToListAsync();

                return View(result);
            }
            else
            {
                var result = await modelContext.Where(x => x.PayDate.Value.Date >= startDate && x.PayDate.Value.Date <= endDate).ToListAsync();
                return View(result);

            }


        }

        public IActionResult Report()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            var model = _context.ProductOrders.Include(p => p.Product).Include(p => p.Order).Include(p => p.Order.User).Include(p => p.Product.Category);

            var TotalAmount = _context.Payments.Sum(x => x.Amount);
            ViewBag.Amount = TotalAmount;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Report(DateTime? stratDate, DateTime? endDate)
        {
            var pc = _context.ProductOrders.Include(p => p.Product).Include(p => p.Order).Include(p => p.Order.User).Include(p => p.Product.Category);
            if (stratDate == null && endDate == null)
            {
                return View(await pc.ToListAsync());
            }
            else if (stratDate == null && endDate != null)
            {
                var result = await pc.Where(p => p.Order.CreatedDate.Value.Date <= endDate).ToListAsync();
                return View(result);
            }
            else if (stratDate != null && endDate == null)
            {
                var result1 = await pc.Where(p => p.Order.CreatedDate.Value.Date >= stratDate).ToListAsync();
                return View(result1);
            }
            else if (stratDate == endDate && stratDate != null && endDate != null)
            {
                var ress = await pc.Where(p => p.Order.CreatedDate.Value.Date == stratDate && p.Order.CreatedDate.Value.Date == endDate).ToListAsync();
                return View(ress);
            }
            else
            {
                var result2 = await pc.Where(p => p.Order.CreatedDate.Value.Date >= stratDate && p.Order.CreatedDate.Value.Date <= endDate).ToListAsync();
                return View(result2);
            }

        }

       public IActionResult Emails()
        {
            var emails = _context.ContactUs.ToList();
            return View(emails);
        }

    }
}
