using Furniture.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MimeKit;

using MailKit.Net.Smtp;

namespace Furniture.Controllers
{
	public class AuthenticationController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

		public AuthenticationController(IWebHostEnvironment webHostEnvironment , ModelContext context)
		{
			_webHostEnvironment = webHostEnvironment;
			_context = context;
		}
        public IActionResult Register()
		{
			return View();
		}
        [HttpPost]
        public async Task< IActionResult> Register([Bind("Id,Fullname,Phone,ImagePath,ImageFile,Email")] UserAccount userAccount , string username , string password)
        {
            if (ModelState.IsValid)
            {
                if (userAccount.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;

                    string fileName = Guid.NewGuid().ToString() + "_" + userAccount.ImageFile.FileName;

                    string path = Path.Combine(wwwRootPath + "/image/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await userAccount.ImageFile.CopyToAsync(fileStream);
                    }
                    userAccount.ImagePath = fileName;

                }
                userAccount.RoleId = 2;
               
                _context.Add(userAccount);

                await _context.SaveChangesAsync();

                Login login = new Login();
                login.Username = username;
                login.Passwordd = password;
                login.UserId = userAccount.Id;
                

                _context.Add(login);
                await _context.SaveChangesAsync();

                //send Email
                MimeMessage mail = new MimeMessage();
                MailboxAddress emailFrom = new MailboxAddress("Furniture Store", "furnituresstoreonline@gmail.com");
                MailboxAddress emailTo = new MailboxAddress("", userAccount.Email);
                BodyBuilder bodyBuilder = new BodyBuilder();
                mail.From.Add(emailFrom);
                mail.To.Add(emailTo);


                mail.Subject = "Thank you for Regstration";
                BodyBuilder bb = new BodyBuilder();
                DateTime dt = DateTime.Now;
                bb.HtmlBody = "<html>" + "<h1>" + "Welcome " + login.Username + dt.ToString() + "</h1>" + "</html>";
                mail.Body = bb.ToMessageBody();

                SmtpClient emailClinet = new SmtpClient();
                emailClinet.Connect("smtp.gmail.com", 465, true);
                emailClinet.Authenticate("furnituresstoreonline@gmail.com", "oucuhhjndmflrftf");
                emailClinet.Send(mail);

                emailClinet.Disconnect(true);
                emailClinet.Dispose();



                return RedirectToAction("Login", "Authentication");


            }
                return View(userAccount);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Id,Username,Passwordd,UserId")] Login userLogin)
        {
            var user = _context.Logins.Where(x => x.Username.Equals(userLogin.Username) && x.Passwordd.Equals(userLogin.Passwordd)).FirstOrDefault();
            if(user != null)
            {
                var customer = _context.UserAccounts.Where(x => x.Id.Equals(user.UserId)).FirstOrDefault();
                switch (customer.RoleId)
                {
                    case 1: //Admin 
                        HttpContext.Session.SetInt32("id", (int)customer.Id);
                        HttpContext.Session.SetString("FullName", customer.Fullname);
                        HttpContext.Session.SetInt32("loginid", (int)user.Id);
                        HttpContext.Session.SetString("Email", customer.Email);
                        HttpContext.Session.SetString("username", user.Username);
                        HttpContext.Session.SetString("password", user.Passwordd);
                        HttpContext.Session.SetString("image", customer.ImagePath);

                        

                        return RedirectToAction("Index", "Admin");
                    case 2: //Customer
                        HttpContext.Session.SetInt32("id", (int)customer.Id);
                        HttpContext.Session.SetString("FullName", customer.Fullname);
                        HttpContext.Session.SetInt32("loginid", (int)user.Id);
                        HttpContext.Session.SetString("Email", customer.Email);
                        HttpContext.Session.SetString("username", user.Username);
                        HttpContext.Session.SetString("password", user.Passwordd);
                        // HttpContext.Session.SetString("Email", customer.Email);
                        HttpContext.Session.SetString("image", customer.ImagePath);




                        return RedirectToAction("Index", "Customer");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid Email or password");

            return View();
        }

        public IActionResult Logout()
        {

            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

       
    }



}
