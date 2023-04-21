using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScavengeRUs.Models.Entities;
using ScavengeRUs.Services;
using System;
using System.Security.Claims;


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
    /// <remarks>This can only be called by admins</remarks>
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;
        string defaultPassword = "Etsupass12!";

        /// <summary>
        /// This is the dependecy injection for the User Repository that connects to the database
        /// </summary>
        /// <param name="userRepo"></param>
        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// This maps to the Views/User/Manage view. Which is the "Admin Portal". The parameter
        /// is a string that is optional and can be used to search for a user in the database.
        /// </summary>
        /// <returns>Views/User/Manage.cshtml</returns>
        public async Task<IActionResult> Manage(string searchString)
        {
            var users = await _userRepo.ReadAllAsync(); //Reads all the users in the db
            //if the admin didn't search for anything just return all the users
            //git 
            if(string.IsNullOrEmpty(searchString))
                //Below, the users list is passed to the view as the model for it
                return View(users);  //Right click and go to view to see HTML
                

            //this line of code filters out all the users whose emails and phone numbers do not
            //contain the search string
            var searchResults = users.Where(user => user.Email.Contains(searchString) 
            || !string.IsNullOrEmpty(user.PhoneNumber) && user.PhoneNumber.Contains(searchString)
            || (user.AccessCode?.Code?.Contains(searchString) ?? false));

            return View(searchResults); //passes the searchResults as the model for the view
        }

        /// <summary>
        /// This maps to the Views/User/Edit view which has a form to edit a user.
        /// The parameter is read from the URL when this method is called, and is used
        /// to search for what user to modify.
        /// </summary>
        /// <returns>Views/User/Edit.cshtml</returns>
        public async Task<IActionResult> Edit([Bind(Prefix = "id")]string username)
        {
            var user = await _userRepo.ReadAsync(username);
            return View(user); //passes the user as the model for this view
        }

        /// <summary>
        /// This handles the post request from the Views/User/Edit view when they submit the form.
        /// The parameter is read from the form when this method is called.
        /// </summary>
        /// <returns>Views/User/Manage.cshtml, or Views/User/Edit.cshtml is the model isn't valid</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                await _userRepo.UpdateAsync(user.Id, user);
                return RedirectToAction("Manage");
            }
            return View(user); //passes the user as the model for the view
        }

        /// <summary>
        /// This maps to the Views/User/Delete view.
        /// Which is the landing page to delete a user aka "Are you sure you want to delete user X?".
        /// The parameter is read from the URL when this method is called, which is used to search for that user to see if it exists.
        /// </summary>
        /// <returns>Views/User/Delete.cshtml, or Views/User/Manage.cshtml if the user doesn't exist</returns>
        public async Task<IActionResult> Delete([Bind(Prefix ="id")]string username)
        {
            var user = await _userRepo.ReadAsync(username);
            if (user == null)
            {
                return RedirectToAction("Manage");
            }
            return View(user); //passes the user as the model for the view
        }

        /// <summary>
        /// This method handles the post request from the Views/User/Delete view.
        /// This gets executed when you hit submit on that view.
        /// The parameter is read from the form when this method is called
        /// </summary>
        /// <returns>Views/User/Manage.cshtml</returns>
        /// 
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([Bind(Prefix = "id")]string username)
        {
            await _userRepo.DeleteAsync(username);
            return RedirectToAction("Manage");
        }

        /// <summary>
        /// This maps to the Views/User/Details view.
        /// Which is the page for viewing the details of a user.
        /// The parameter is read from the URL and is used to read the user from the database.
        /// </summary>
        /// <returns>Views/User/Details.cshtml</returns>
        public async Task<IActionResult> Details([Bind(Prefix = "id")]string username)
        {
            var user = await _userRepo.ReadAsync(username);

            return View(user); //passes the user as the model for this view
        }

        /// <summary>
        /// This maps to the Views/User/Create view which has a form to create a user.
        /// </summary>
        /// <returns>Views/User/Create.cshtml</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// This handles the post request that is called when you submit the form from the 
        /// Views/User/Create view. The parameter is read from the form when this method is called.
        /// </summary>
        /// <returns>Views/User/Details.cshtml, or Views/User/Create.cshtml if the model isn't valid</returns>
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                user.UserName = user.Email;
                await _userRepo.CreateAsync(user, defaultPassword);
                return RedirectToAction("Details", new { id = user.UserName });
            }
            return View(user); //passes the user as the model to the view
            
        }

        /// <summary>
        /// This maps to the Views/User/Profile view. Which is the profile page for a user. I don't think we use this, and instead use the Identity prescaffolded libaries.
        /// </summary>
        /// <returns>Views/User/Profile.cshtml</returns>
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userRepo.ReadAsync(User.Identity?.Name!);
            return View(currentUser); //passes the logged in user as the model for the view
        }
    }
}
