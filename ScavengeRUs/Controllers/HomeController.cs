using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScavengeRUs.Models;
using ScavengeRUs.Models.Entities;
using ScavengeRUs.Services;
using System.Diagnostics;

namespace ScavengeRUs.Controllers
{
    /// <summary>
    /// This file is a controller, that means it handles HTTP requests from the client, each method listed below
    /// can be called while the application is running by using this syntax in the URL: /{ControllerName}/{MethodName}. For example,
    /// to call the Index method of the Home controller, it would be /Home/Index<br></br>
    /// A method handles HTTP GET requests by default, to handle other requests, it will have a tag that specifies so above it.
    /// Most of these methods also return a view, which is the HTML file the client will recieve. You will find these files in the 
    /// Views folder of the project. They match up with the controller and method name they go to. For example, the view for the
    /// Index page in the Home controller is in the Views/Home/Index.cshtml directory.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Injects the objects used in this controller. These are the logger, userRepo, and signInRepo objects. The logger object is used
        /// for writing to the console while the app runs. The repo objects are used for reading the database.
        /// </summary>
        public HomeController(ILogger<HomeController> logger, IUserRepository userRepo, SignInManager<ApplicationUser> signInRepo)
        {
            _signInRepo = signInRepo;
            _userRepo = userRepo;
            _logger = logger; 
        }
        /// <summary>
        /// This will return the landing page for www.localhost.com.
        /// This view is in the Views/Home/Index
        /// </summary>
        /// <returns>Views/Home/Index.cshtml</returns>
        public IActionResult Index()
        {
            return View();      //Right click and go to view to see the HTML or see it in the Views/Home folder in the solution explorer
        }

        /// <summary>
        /// This doesn't do anything. Not sure why this is here but I'll leave it for now
        /// </summary>
        /// <returns>A view, but it doesn't exist so there will be an error</returns>
        public IActionResult LogIn()
        {
            return View();
        }

        /// <summary>
        /// Confirms a login when the index page sends a post request. 
        /// </summary>
        /// <returns>Redirects the user to the ViewTasks page if the accesscode is valid</returns>
        [HttpPost, ActionName("LogIn")]
        public async Task<IActionResult> LogInConfirmed(AccessCode accessCode)
        {
            if (accessCode.Code == null) //if access code is given
            {
                return View("Error", new ErrorViewModel() { Text = "Enter a valid access code." }); 
                    
            }
            var user = await _userRepo.FindByAccessCode(accessCode.Code!);
            if (user == null) //if the access code does not match up with a user
            {
                return View("Error", new ErrorViewModel() { Text = "Enter a valid access code." });
            }
            
            var timeRemaining = (user.Hunt!.EndDate - DateTime.Now).ToString();
            if (TimeSpan.Parse(timeRemaining).Seconds < 0) //checks if the access code is still valid
            {
                return View("Error", new ErrorViewModel() { Text = "This access code has expired." });
            }
            
            await _signInRepo.SignInAsync(user, false); //signs in the user if the code is valid
            return RedirectToAction("ViewTasks", "Hunt", new {id = user.Hunt?.Id}); // change to redirect to view of hunts
        }
        /// <summary>
        /// This is the landing page for www.localhost.com/Home/Privacy
        /// Only people that are "Admin" can view this 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles ="Admin")]
        public IActionResult Privacy()
        {
            return View();
        }


        /// <summary>
        /// This is the page displayed if there were a error
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}