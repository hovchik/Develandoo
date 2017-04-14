using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Develandoo.EmailSender;
using Develandoo.Infrastructure;

namespace Develandoo.Controllers
{
    public class MainController : Controller
    {
        private UnitOfWork unitOfWork;
        /// <summary>
        /// MainController constructor for initialization UnitOfWork object
        /// </summary>
        public MainController()
        {
            unitOfWork = new UnitOfWork();
        }
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Http Get function for view registration form to user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        /// <summary>
        /// Http Post function to get model from user input
        /// </summary>
        /// <param name="model">User input model for createing record in database</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Registration(Users model)
        {
            if (ModelState.IsValid)
            {
                MailCredentional mc = new MailCredentional()
                {
                    Email = ConfigurationManager.AppSettings["email"],
                    Password = ConfigurationManager.AppSettings["pass"]
                };
                MailSender sender = new MailSender(mc);
                var actCode = Guid.NewGuid();
                var newUser = new Users()
                {
                    Id = Counter.Count,
                    Username = model.Username,
                    Email = model.Email,
                    IsVerifyed = false,
                    Password = model.Password, ///dont used any crypto algorithm for simplify code 
                    UserActivationCode = actCode

                };
                int UserIsAlreadyCreated = CheckUserByMail(newUser.Email);
                switch (UserIsAlreadyCreated)
                {
                    case 1:
                        break;
                    case 2:
                        ViewBag.ErrorMessage = "This user is not Activated, please check email";
                        ViewBag.index = 1;
                        return View("Error");
                    case 0:
                        ViewBag.ErrorMessage = "This user is already exists";
                        return View("Error");
                }



                string body = GenerateMailBody(newUser.Id, actCode);

                unitOfWork.Users.Create(newUser);
                await unitOfWork.SaveAsync();
                await sender.SendAsync("Confirmation by Email", body, model.Email);



            }
            ViewBag.ErrorMessage = "Please check Your Email for confirming your account! ";
            return View("UserLogin");
        }

        /// <summary>
        /// Help function for checking user email validation
        /// </summary>
        /// <param name="email">user email address</param>
        /// <returns></returns>
        private int CheckUserByMail(string email)
        {

            int count = unitOfWork.Users.CountByRule(p => p.Email.ToLower() == email.ToLower());
            if (count == 0)
            {
                return 1;
            }
            if (count > 0 && unitOfWork.Users.CountByRule(p => p.Email.ToLower() == email.ToLower() && p.IsVerifyed == false) > 0)
            {
                return 2;
            }
            else
            {
                return 0;
            }

        }
        /// <summary>
        /// This function get email confirmation request and check must user verifyed or not
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ConfirmEmailView()
        {
            string activationCode = !string.IsNullOrEmpty(Request.QueryString["ActivationCode"]) ? Request.QueryString["ActivationCode"] : Guid.Empty.ToString();
            var userId = !string.IsNullOrEmpty(Request.QueryString["userId"]) ? int.Parse(Request.QueryString["userId"]) : -1;
            string Status = String.Empty;


            var findedUser = unitOfWork.Users.Get(userId);
            if (findedUser?.UserActivationCode == Guid.Parse(activationCode))
            {
                findedUser.IsVerifyed = true;
                unitOfWork.Users.Update(findedUser);
                await unitOfWork.SaveAsync();
                Status = "User is activated!";
            }
            else
            {
                Status = "Incorect User";
            }
            ViewBag.Status = Status;


            return View();
        }
        /// <summary>
        /// helper function to generate simple message body for email confirmation
        /// </summary>
        /// <param name="modelId">send to user his Id/ it is not secure , but simple</param>
        /// <param name="activationCode">user activation token, simplyfied to GUID</param>
        /// <returns></returns>
        private string GenerateMailBody(Int64 modelId, Guid activationCode)
        {

            var link = Url.Action("ConfirmEmailView", "Main", new { userId = modelId, ActivationCode = activationCode }, protocol: Request.Url.Scheme);
            return $"You can verify registration by this link {link}";
        }
        /// <summary>
        /// http get function to present user the login form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UserLogin()
        {
            return View();
        }
        /// <summary>
        /// http post function to get user login input and check it correctness
        /// </summary>
        /// <param name="model">user input model</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserLogin(Users model)
        {
            var user = unitOfWork.Users.GetWithPredict(p => p.Email == model.Email);
            if (user == null)
            {
                ViewBag.Status = $"User by {model.Email} email not exists !!!";
                ViewBag.Index = 2;
            }
            else
            if (user?.Password != model.Password)
            {
                ViewBag.Status = $"Incorrect password for {model.Email} !!!";
                ViewBag.Index = 3;
            }
            else
                if (user != null && user.IsVerifyed)
            {
                ViewBag.Status = $"User {user.Username} is loged on";
                ViewBag.Index = 0;
            }
            else
            {
                ViewBag.Status = $"User {model.Email} is not verifyed";
                ViewBag.Index = 1;
            }
            return View("UserLand");

        }


        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}