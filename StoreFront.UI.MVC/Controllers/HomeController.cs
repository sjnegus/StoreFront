using Microsoft.AspNetCore.Mvc;
using StoreFront.UI.MVC.Models;
using System.Diagnostics;

using MimeKit;
using MailKit.Net.Smtp;

namespace StoreFront.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // CONTACT 
        public IActionResult Contact() { return View(); }
        // 
        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
            if (!ModelState.IsValid)
            {
                return View(cvm);
            }

            string message = $"You have recieved a new email from your site's contact form. <br>" +
    $"Sender: {cvm.Name}<br />Email: {cvm.Email}<br />Subject: {cvm.Subject}<br />Message: {cvm.Message}";

            // Create a MimeMessage object to assist with storing/transporting the email information from the contact form
            var mm = new MimeMessage();

            // Even though the user is the one attempting to reach us, the actual sender of the email
            // will be the email user we set up with our hosting provider.

            // we can access the credentials for this email user from our appsettings.json file as shown below
            mm.From.Add(new MailboxAddress("Sender", _config.GetValue<string>("Credentials:Email:User")));

            // The recipient of this email will be our personal email address, also typed in appsettings.json
            mm.To.Add(new MailboxAddress("Personal", _config.GetValue<string>("Credentials:Email:Recipient")));

            // The subject will be the one provided by the user which is stored in the cvm object
            mm.Subject = cvm.Subject;

            // The body of the message will be formatted with the string we created above
            mm.Body = new TextPart("HTML") { Text = message };

            // We can set the priority of the messages as "urgent" so it will be flagged in our email client
            mm.Priority = MessagePriority.Urgent;

            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            using (var client = new SmtpClient())
            {
                try
                {
                client.Connect(_config.GetValue<string>("Credentials:Email:Client"));

                // Some ISPs may block the default SMTP port (25), so, if you encounter issues
                // sending email with the line above, comment it out and uncomment the line below,
                // which does the same thing but specifies to use the alternate SMTP port, 8889.
                //client.Connect(_config.GetValue<string>("Credentials:Email:Client"), 8889);

                // Log in to the mail server using the credentials for our email user
                client.Authenticate(

                    // Username
                    _config.GetValue<string>("Credentials:Email:User"),

                    // Password
                    _config.GetValue<string>("Credentials:Email:Password")

                    );

                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"There was an error processing your request. please" +
                        $" try again later.<br />Error Message: {ex.Message}";

                    return View(cvm);

                }

            }// when we get here, we will automatically get rid of the client object

            return View("EmailConfirmation", cvm);

            //return View();
        }

    }
}
