using Furniture.Models;
using Furniture.Models.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.IO;
using System.Linq;

using MimeKit;
using MailKit.Net.Smtp;

using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.AspNetCore.Authorization;

using System.Drawing.Printing;
using System.Xml.Linq;
using System.Net.Mail;
using System.Net;

using SmtpClient = System.Net.Mail.SmtpClient;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Reflection.Metadata;
using Document = iTextSharp.text.Document;
using System.Reflection.PortableExecutable;
using Org.BouncyCastle.Crypto.Tls;

namespace Furniture.Controllers

{
    class HeaderFooter : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfPTable tbHeader = new PdfPTable(3);
            tbHeader.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tbHeader.DefaultCell.Border = 0;

            tbHeader.AddCell(new Paragraph());
       

            PdfPCell _cell = new PdfPCell(new Paragraph("Invoice"));
            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
            _cell.Border = 0;
            tbHeader.AddCell(_cell);
            tbHeader.AddCell(new Paragraph());

            tbHeader.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetTop(document.TopMargin)+40,writer.DirectContent);

            PdfPTable tbFooter = new PdfPTable(3);
            tbFooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tbFooter.DefaultCell.Border = 0;
             _cell = new PdfPCell(new Paragraph("Jordan-Irbid"));
            _cell.HorizontalAlignment = Element.ALIGN_CENTER;

            _cell.Border = 0;
            tbFooter.AddCell(_cell);
            tbFooter.AddCell(new Paragraph());

            tbFooter.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetBottom(document.BottomMargin) -5, writer.DirectContent);




        }
    }

    public class CustomerController : Controller
    {
     
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public CustomerController(ILogger<HomeController> logger, ModelContext context , IWebHostEnvironment hostEnvironment)
        {

            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            var category = await _context.CategoryProjects.ToListAsync();

            return View(category);

        }

        public async Task<IActionResult> Products(int Id)
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            var product = await _context.ProductProjects.Where(x => x.CategoryId == Id).Include(p => p.Category).ToListAsync();
            return View(product);

        }

        [HttpGet]
        public async Task<IActionResult> AllProducts(string productname , string SearchString)
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            IQueryable<string> genreQuery = from p in _context.ProductProjects
                                            orderby p.ProductName
                                            select p.ProductName;
            var products = from p in _context.ProductProjects
                           select p;

            if (!String.IsNullOrEmpty(SearchString))
            {
                products = products.Where(s => s.ProductName!.Contains(SearchString));
            }

            if (!String.IsNullOrEmpty(productname))
            {
                products = products.Where(x => x.ProductName == productname);
            }

            var productVm = new ProductViewModel
            {
                productss = new SelectList(await genreQuery.Distinct().ToListAsync()),
                products = await products.ToListAsync()
            };

            // var modelContext = _context.ProductProjects.Include(p => p.Category);
            return View(productVm);

        }

        [HttpPost]
        public string AllProducts(string SearchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + SearchString;
        }
        [HttpGet]
        public IActionResult AddTestimonial()
        {
            ViewBag.image = HttpContext.Session.GetString("image");


            return View();

        }


        [HttpPost]
        public async Task<IActionResult> AddTestimonial([Bind("Id,UserId,Message")] Testimonial testimonial)

        {
            ViewBag.image = HttpContext.Session.GetString("image");

            if (ModelState.IsValid)
            {
                decimal id = (decimal)HttpContext.Session.GetInt32("id");
                testimonial.UserId = id;
                testimonial.Status = "no";
                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(testimonial);

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
            // ViewData["RoleId"] = new SelectList(_context.RoleeUsers, "Id", "Rolename",  user.RoleId);
            return View( user);
        }


        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            decimal number =Convert.ToDecimal(HttpContext.Session.GetInt32("id"));

            string username = HttpContext.Session.GetString("username");
            string password = HttpContext.Session.GetString("password");

            ViewBag.username = username;
            ViewBag.password = password;

            //  string fullname = HttpContext.Session.GetString("FullName");
            //   string Email = HttpContext.Session.GetString("Email");

            var user = _context.UserAccounts.Where(x => x.Id.Equals(number)).Include(x => x.Logins).FirstOrDefault();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(decimal id, [Bind("Id,Fullname,Phone,Email,RoleId,ImageFile")] UserAccount userAccount, string password , string username)
        {
            if (ModelState.IsValid)
            {

                if (userAccount.ImageFile  != null)
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

                userAccount.RoleId = 2;
                _context.Update(userAccount);
                await _context.SaveChangesAsync();

                var user = _context.Logins.Where(x => x.UserId.Equals(userAccount.Id)).FirstOrDefault();
                user.Passwordd = password;
                user.Username = username;

                _context.Update(user);
              await  _context.SaveChangesAsync();


                return RedirectToAction("ShowProfile", "Customer");
            }
            
            return View(userAccount);
        }

        
        public IActionResult Shopping(decimal id)
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            decimal currentUser = Convert.ToDecimal(HttpContext.Session.GetInt32("id"));
            OrderrProject orderr = new OrderrProject();
            orderr.UserId = currentUser;
            orderr.CreatedDate = DateTime.Now;

            _context.Add(orderr);
           _context.SaveChanges();

            ProductOrder productOrder = new ProductOrder();

            productOrder.ProductId = id;
            productOrder.OrderId = orderr.Id;
            productOrder.Status = "0";

            _context.Add(productOrder);
            _context.SaveChanges();

            return RedirectToAction("ShoppingCart");
        }

     

        public IActionResult ShoppingCart()
        {
            ViewBag.image = HttpContext.Session.GetString("image");

            decimal currentUser =(decimal)HttpContext.Session.GetInt32("id");
            var products = _context.ProductOrders.Where(x => x.Order.UserId == currentUser && x.Status == "0").Include(p => p.Order).Include(p => p.Product).Include(p => p.Product.Category).ToList();
            ViewBag.value = products.Sum(x=>x.Product.Price);

           // var another = products.Where(x => x.Status == "0");
            //  var products = _context.ProductOrders.Where(x =>x.Order.);
            return View(products);
        }
       
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var order = await _context.ProductOrders.FindAsync(id);
            _context.ProductOrders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShoppingCart");
        }

        public IActionResult Payment()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Payment([Bind("Id,Cardnumber,Cvv ,Amount")] Bank bank, decimal id)
        {
            if (ModelState.IsValid)
            {
                string splitCredit = String.Concat(bank.Cardnumber.Where(p => !char.IsWhiteSpace(p)));
                var account = await _context.Banks.Where(x => x.Cardnumber == splitCredit && x.Cvv == bank.Cvv).FirstOrDefaultAsync();
                if(account != null)
                {
                   
                    decimal currentUser = Convert.ToDecimal(HttpContext.Session.GetInt32("id"));
                    var products = _context.ProductOrders.Where(x => x.Order.UserId.Equals(currentUser) && x.Status.Equals("0"));

                    Payment payment = new Payment();
                    payment.PayDate = DateTime.Now;
                    payment.UserId = currentUser;
                    payment.Amount = products.Sum(x => x.Product.Price);

                    if(payment.Amount <= account.Amount)
                    {
                        account.Amount = account.Amount - payment.Amount;
                     
                        await _context.SaveChangesAsync();


                        Document document = new Document(iTextSharp.text.PageSize.LETTER,30f,20f,50f,40f);

                        PdfWriter pw = PdfWriter.GetInstance(document, new FileStream("Invoice.pdf", FileMode.Create));
                        pw.PageEvent = new HeaderFooter();
                        var invoice = _context.UserAccounts.Where(u => u.Id == (decimal)currentUser).FirstOrDefault();
                        var x = _context.ProductOrders.Where(p => p.Order.UserId == currentUser && p.Status == "0").Include(p => p.Order.User).Include(p => p.Product.Category).Include(p => p.Product).ToList();
                        document.Open();
            
                        PdfPTable table = new PdfPTable(4);
                        table.WidthPercentage = 100;

                        foreach (var item in x)
                        {
                            PdfPCell _cell = new PdfPCell();

                            _cell = new PdfPCell(new Paragraph(item.Product.ProductName));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(_cell);


                            _cell = new PdfPCell(new Paragraph(item.Product.Price.ToString()+"$"));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(_cell);

                            _cell = new PdfPCell(new Paragraph(item.Product.Category.Categoryname.ToString()));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(_cell);

                            _cell = new PdfPCell(new Paragraph(item.Order.CreatedDate.ToString()));
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(_cell);

      
                        }
                        // start image 


                        // end image
                        
                        document.Add(table);
                        document.Close();


           

                        try
                        {
                            SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com", 587);

                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient("smtp-mail.outlook.com", 587);
                            mail.From = new MailAddress("FurnitureGalley@outlook.com");
                            mail.To.Add(invoice.Email);
                            mail.Subject = "Purchase Invoice";
                            mail.Body = "Welcome to Furiture Gallery";
                            smtp.Credentials = new NetworkCredential("FurnitureGalley@outlook.com", "ameqshskwhjufarl");
                            mail.Attachments.Add(new Attachment("Invoice.pdf"));
                            smtp.EnableSsl = true;

                            smtp.Send(mail);

                            foreach (var item in products)
                            {
                                item.Status = "1";
                            }

                            _context.Add(payment);

                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {

                            throw ex.GetBaseException();
                        }




                        return RedirectToAction("ShoppingCart");
                    }

                    else
                    {
                        ModelState.AddModelError("", "You dont have a enough money");

                    }
                }
            }

            else
            {
                ModelState.AddModelError("", "Your card is not valid. Please try again.");
            }
            return View(bank);
        }
       
      

    }

   
}
