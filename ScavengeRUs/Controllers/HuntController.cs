using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengeRUs.Models.Entities;
using ScavengeRUs.Services;
using System.Collections.Immutable;
using System.Net.Mail;

namespace ScavengeRUs.Controllers
{
    /// <summary>
    /// This controller is for Hunt methods.
    /// <br></br>
    /// This file is a controller, that means it handles HTTP requests from the client, each method listed below
    /// can be called while the application is running by using this syntax in the URL: /{ControllerName}/{MethodName}. For example,
    /// to call the Index method of the Home controller, it would be /Home/Index<br></br>
    /// A method handles HTTP GET requests by default, to handle other requests, it will have a tag that specifies so above it.
    /// Most of these methods also return a view, which is the HTML file the client will recieve. You will find these files in the 
    /// Views folder of the project. They match up with the controller and method name they go to. For example, the view for the
    /// Index page in the Home controller is in the Views/Home/Index.cshtml directory.
    /// </summary>
    public class HuntController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IHuntRepository _huntRepo;

        /// <summary>
        /// This injects the user repository and hunt repository, if it didn't do this then the _userRepo and _huntRepo variables
        /// would be null. The userRepo and huntRepo are used for database methods for users and hunts respectively.
        /// </summary>
        public HuntController(IUserRepository userRepo, IHuntRepository HuntRepo)
        {
            _userRepo = userRepo;
            _huntRepo = HuntRepo;
        }

        /// <summary>
        /// Returns the Index file for the Hunt controller. This can only be viewed by logged in Admins. There is also
        /// an optional parameter that can sort the hunts on the view. If no parameter is given for it, then it will just return the hunts without sorting it.
        /// The page lists all hunts in the database.
        /// </summary>
        /// <returns>Views/Hunt/Index.cshtml</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string type)
        {
            var hunts = await _huntRepo.ReadAllAsync();
            var sortedHunts = await _huntRepo.ReadAllAsync();

            //if the admin didn't search for anything just return all the users
            if (string.IsNullOrEmpty(type))
                return View(hunts);  //Right click and go to view to see HTML

            switch (type)
            {
                case "1":
                    sortedHunts = hunts.OrderByDescending(item => item.StartDate).ToList();
                    break;
                case "2":
                    sortedHunts = hunts.OrderBy(item => item.StartDate).ToList();
                    break;
                case "3":
                    sortedHunts = hunts.Where(item => item.EndDate > DateTime.Now).ToList();
                    break;
                case "4":
                    sortedHunts = hunts.Where(item => item.EndDate < DateTime.Now).ToList();
                    break;
                case "5":
                    sortedHunts = hunts.Where(item => item.StartDate > DateTime.Now).ToList();
                    break;
            }

            return View(sortedHunts);
        }

        /// <summary>
        /// Returns the view to create a hunt. This view has a form that user can fill out. This is only accessable for Admins.
        /// </summary>
        /// <returns>Views/Hunt/Create.cshtml</returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// This method handles the post method from the Views/Hunt/Create view. It reads in the hunt from the form and tries to add it to the database.
        /// The way it reads the Hunt object from the form is from the hunt parameter. It can only be called by Admins.
        /// </summary>
        /// <param name="hunt"></param>
        /// <returns>Redirect to the Views/Hunt/Index.cshtml page if created successfully. Will return to Views/Hunt/Create.cshtml if it was not valid</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Hunt hunt)
        {
            hunt.URL = $"Hunt/ViewTasks/{hunt.Id.ToString()}"; //this sets the URL of a hunt, might need to change this in the future
            if (ModelState.IsValid)
            {
                await _huntRepo.CreateAsync(hunt);
                return RedirectToAction("Index");
            }
            return View(hunt); //passes a hunt object as the model for the view

        }

        /// <summary>
        /// Returns the view that shows details about a given hunt. 
        /// The parameter is the id of the hunt that is passed in the url.
        /// This id is used to read from the database to get a hunt object which will
        /// be displayed in the view. This can only be called by admins.
        /// </summary>
        /// <returns>Views/Hunt/Details.cshtml</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details([Bind(Prefix = "Id")] int huntId)
        {
            if (huntId == 0) //int defaults to 0 if it is not given a value, so if no id was given to this method, then it will just redirect to the index
            {
                return RedirectToAction("Index");
            }
            var hunt = await _huntRepo.ReadAsync(huntId);
            if (hunt == null) //if the hunt does not exist
            {
                return RedirectToAction("Index");
            }
            return View(hunt); //passes a hunt object as the model for the view
        }

        /// <summary>
        /// Returns the view to ask if the user is sure they want to delete the hunt.
        /// The parameter is the id of the hunt that is passed in the url.
        /// This id is used to read from the database to see if it exists, and then
        /// is displayed in the view. This can only be called by admins.
        /// </summary>
        /// <returns>Views/Hunt/Delete.cshtml, if the hunt does not exist it will redirect to Views/Hunt/Index.cshtml</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([Bind(Prefix = "Id")] int huntId)
        {
            if (huntId == 0) //int defaults to 0 if it is not given a value, so if no id was given to this method, then it will just redirect to the index
            {
                return RedirectToAction("Index");
            }
            var hunt = await _huntRepo.ReadAsync(huntId);
            if (hunt == null) //if the hunt does not exist
            {
                return RedirectToAction("Index"); //passes a hunt object as the model for the view
            }
            return View(hunt);
        }
        /// <summary>
        /// This method handles the post request from the Views/Hunt/Delete page. This method reads a form that just has the id
        /// of the hunt to be deleted. It gets this from the parameter of the method. It will then call the database method
        /// to delete a hunt from the _huntRepo variable. This can only be called by admins.
        /// </summary>
        /// <returns>Redirect to the Views/Hunt/Index.cshtml</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([Bind(Prefix = "id")] int huntId)
        {
            await _huntRepo.DeleteAsync(huntId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Returns a view that displays all the players in a hunt. It reads a hunt from the database
        /// using the id that is passed to the method in the paramter. It gets this from the url.
        /// This can only be called by admins.
        /// </summary>
        /// <returns>Views/Hunt/ViewPlayers.cshtml is the hunt exists. If it doesn't it'll redirect to Views/Hunt/Index.cshtml</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewPlayers([Bind(Prefix = "Id")] int huntId)
        {
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntId);
            ViewData["Hunt"] = hunt; //this passes in a hunt object to be stored in ViewData, which is a dictionary that holds values you can use in a view
            if (hunt == null)
            {
                return RedirectToAction("Index");
            }

            return View(hunt.Players); //passes a list of users for this hunt as a model for the view
        }
        
        /// <summary>
        /// This method returns the view to add a player to hunt. It also passes in the id of the hunt
        /// to the view. It gets this id from the parameter of the method. This is read from the url.
        /// </summary>
        /// <returns>Views/Hunt/AddPlayerToHunt.cshtml</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPlayerToHunt([Bind(Prefix = "Id")] int huntId)
        {
            var hunt = await _huntRepo.ReadAsync(huntId);
            ViewData["Hunt"] = hunt; //this passes in a hunt object to be stored in ViewData, which is a dictionary that holds values you can use in a view
            return View();

        }

        /// <summary>
        /// This method handles the post request from the AddPlayerToHunt view. It reads in the form from the request to the parameter of this
        /// method. This method will then try to read the hunt and player from the database to see if they exist. It will create a 
        /// new user if the user does not exist yet. It will then create a new access code to be used in this hunt for that user.
        /// </summary>
        /// <returns>Views/Hunt/Index.cshtml</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddPlayerToHunt([Bind(Prefix = "Id")] int huntId, ApplicationUser user)
        {

            if (huntId == 0) //int defaults to 0 if it is not given a value, so if no id was given to this method, then it will just redirect to the index
            {
                RedirectToAction("Index");
            }
            var hunt = await _huntRepo.ReadAsync(huntId);
            var existingUser = await _userRepo.ReadAsync(user.Email);
            var newUser = new ApplicationUser();

            if (existingUser == null) //creates a new user if the user they are adding doesn't exist yet
            {
                newUser.Email = user.Email;
                newUser.PhoneNumber = user.PhoneNumber;
                newUser.FirstName = user.FirstName;
                newUser.LastName = user.LastName;
                newUser.AccessCode = user.AccessCode;
                newUser.UserName = user.Email;
            }
            else
            {
                newUser = existingUser;
                newUser.AccessCode = user.AccessCode;
            }
            if (newUser.AccessCode!.Code == null)       //If the admin didn't specify an access code (If we need to, I have the field readonly currently)
            {
                newUser.AccessCode = new AccessCode()
                {
                    //added setter for hunt ID
                    Hunt = hunt,
                    HuntId = hunt.Id,                 //Setting foriegn key
                    Code = $"{newUser.PhoneNumber}"   //{hunt.HuntName!.Replace(" ", string.Empty)}",            //This is the access code generation
                };
                newUser.AccessCode.Users.Add(newUser);  //Setting foriegn key
            }
            else
            {
                newUser.AccessCode = new AccessCode()
                {
                    //added setter for hunt ID
                    Hunt = hunt,
                    HuntId = hunt.Id,
                    Code = newUser.AccessCode.Code,
                };
                newUser.AccessCode.Users.Add(newUser);
            }
            await _huntRepo.AddUserToHunt(huntId, newUser); //This methods adds the user to the database and adds the database relationship to a hunt.
      
            return RedirectToAction("Index");
        }

        /// <summary>
        /// This method returns the view to remove a user from a hunt. It passes in the id of the hunt and the user
        /// to the view. It gets this from the parameter of this method, which is received from the url when this method is called.
        /// </summary>
        /// <returns>Views/Hunt/RemoveUser.cshtml</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUser([Bind(Prefix = "Id")] string username, [Bind(Prefix = "huntId")] int huntid)
        {
            ViewData["Hunt"] = huntid; //this passes in a hunt object to be stored in ViewData, which is a dictionary that holds values you can use in a view
            var user = await _userRepo.ReadAsync(username);
            return View(user); //passes in the user object as the model for the view

        }

        /// <summary>
        /// This method handles the post request from the RemoveUser view.
        /// It reads the form from the post request by using the parameter of this method.
        /// It then calls a database method to remove the user using the data from the form.
        /// </summary>
        /// <returns>Views/Hunt/Index.cshtml</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RemoveUserConfirmed(string username, int huntid)
        {
            await _huntRepo.RemoveUserFromHunt(username, huntid);
            return RedirectToAction("Index");

        }

        /// <summary>
        /// This method returns the ViewTasks view which is the main page users will be on to
        /// complete a hunt. It has a parameter of the hunt id which is read from the url when this method is called.
        /// It also passes in a few variables to the view to be used in displaying the page properly.
        /// </summary>
        /// <returns>Views/Hunt/ViewTasks.cshtml if the user is logged in and the huntid is valid. If it
        /// isn't then it will redirect to Views/Home/Index.
        /// </returns>
        public async Task<IActionResult> ViewTasks([Bind(Prefix = "Id")] int huntid)
        {
            if(!User.Identity!.IsAuthenticated)
            {
                //changed the hunt URL to include unique ID for each hunt
                return RedirectToAction("Index", "Home", new
                {
                    id = huntid
                }) ; 
            }
            var currentUser = await _userRepo.ReadAsync(User.Identity?.Name!);
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntid);
            ViewData["Hunt"] = hunt; //this passes in a hunt object to be stored in ViewData, which is a dictionary that holds values you can use in a view
            ViewData["Player"] = currentUser; //this passes in a ApplicationUser object to be stored in ViewData, which is a dictionary that holds values you can use in a view
            if (hunt == null)
            {
                return RedirectToAction("Index");
            }

            if (DateTime.Now > hunt.EndDate) //We are passing a variable here to check if the hunt has already ended
            {
                ViewBag.HasEnded = true; //true if it has ended
                //ViewBag is like ViewData except it is not a dictionary and has properties instead to store values
            }
            else
            {
                ViewBag.HasEnded = false; //false it not
            }

            var tasks = await _huntRepo.GetLocations(hunt.HuntLocations);
            foreach (var item in tasks)
            {
                if (currentUser.TasksCompleted.Count() > 0)
                {
                    var usertask = currentUser.TasksCompleted.FirstOrDefault(a => a.Id == item.Id);
                    if (usertask != null && tasks.Contains(usertask))
                    {
                        item.Completed = "Completed";
                    }
                }
                else
                {
                    item.Completed = "Not completed";
                }
            }
            return View(tasks);
        }

        /// <summary>
        /// This method returns the view that lets the admin add or remove tasks from a hunt.
        /// It has a parameter for the huntid which is read from the url. This method uses the
        /// id to read a hunt from a database which then passes the tasks of the hunt to the view.
        /// This can only be called by a user that is logged in and is an admin.
        /// </summary>
        /// <returns>Views/Hunt/ManageTasks.cshtml if the hunt is still going on. If it has already ended then it will
        /// redirect to Views/Hunt/ViewTasks.cshtml
        /// </returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageTasks([Bind(Prefix = "Id")] int huntid)
        {
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntid);
            //var existingLocations = await _huntRepo.GetLocations(hunt.HuntLocations);

            ViewData["Hunt"] = hunt; //stores a hunt object in the ViewData dictionary, which allows you to store objects to be used in a view

            if (DateTime.Now > hunt.EndDate)
            {
                return RedirectToAction("ViewTasks", new { id = hunt.Id }); //redirect if the hunt has already ended
            }

            var allLocations = await _huntRepo.GetAllLocations();
            //var locations = allLocations.Except(existingLocations);
            return View(allLocations); //passes a collection of locations as the model for this view
        }

        /// <summary>
        /// This method adds a task to a hunt given the id of the task and hunt. It gets this from the parameters which is
        /// given by the url when this method is called. It is called by clicking "Add Task" in the Views/Hunt/ManageTasks view.
        /// It reads the hunt from the database from the given huntid and calls the database method to add it.
        /// This method can only be called by an admin.
        /// </summary>
        /// <returns>Views/Hunt/ManageTasks.cshtml</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTask(int id, int huntid)
        {
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntid);
            ViewData["Hunt"] = hunt; //stores a hunt object in the ViewData dictionary, which allows you to store objects to be used in a view
            await _huntRepo.AddLocation(id, huntid);
            return RedirectToAction("ManageTasks", new { id = huntid });
        }

        /*** This code block below is not used for anything. It can be removed whenever. ***/
        /// <summary>
        /// This is the get method for removing a task from a hunt. This is executed when clicking "Remove" from the Hunt/ViewTasks screen
        /// </summary>
        /// <param name="id"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        //public async Task<IActionResult> RemoveTasks(int id, int huntid)
        //{
        //    var hunt = await _huntRepo.ReadAsync(huntid);
        //    ViewData["Hunt"] = hunt;
        //    var task = await _huntRepo.ReadLocation(id);
        //    return View(task);
        //}

        /// <summary>
        /// This method removes a task from a hunt given the id of the task and hunt from the url. It reads it into the parameter of
        /// this method.  It is called by clicking the "Remove Task" button in the Views/Hunt/ManageTasks view.
        /// It calls the database method to delete the hunt given the id of the task and hunt.
        /// </summary>
        /// <returns>Views/Hunt/ManageTasks.cshtml</returns>
        public async Task<IActionResult> RemoveTask(int id, int huntid)
        {
            await _huntRepo.RemoveTaskFromHunt(id, huntid);
            return RedirectToAction("ManageTasks", "Hunt", new { id = huntid });
        }

        /// <summary>
        /// This method returns the view that displays the scoreboard of the hunt. It has a
        /// parameter for the huntid that is read from the url when this method is called.
        /// It also passes in the hunt and user objects to the view for the logic there.
        /// </summary>
        /// <returns>Views/Hunt/Scoreboard.cshtml, if the hunt is null after being read from the database
        /// then it will redirect to Views/Hunt/ViewTasks
        /// </returns>
        public async Task<IActionResult> Scoreboard([Bind(Prefix = "id")] int huntid)
        {
            var hunt = await _huntRepo.ReadAsync(huntid);
            if (hunt == null)
            {
                return RedirectToAction("ViewTasks", new { id = huntid });
            }
            ViewData["hunt"] = hunt; //stores a hunt object in the ViewData dictionary which allows you to store objects to be used in a view
            ViewData["player"] = await _userRepo.ReadAsync(User.Identity?.Name!); //same as the line above but is a ApplicationUser object instead
            var players = hunt.Players;
            return View(players); //passes in the players of a hunt as the model for the view
        }

        /// <summary>
        /// This method handles post requests from the Views/Hunt/ViewPlayers view. This view
        /// calls this method whenever an admin clicks the send email button. Only an admin can call this method.
        /// This method has parameters for the username and huntid that is read from the form. 
        /// <br></br><br></br>
        /// The method will try to read the user and hunt from the database by using the parameters given to it. An email message is then
        /// constructed that will send to that user's email. It will also hold the access code and url of the hunt in the
        /// email message. It sends this message using an SMTP server that is hosted at ElasticEmail.com. The account information
        /// for this will be in the documentation for this project.
        /// </summary>
        /// <returns>JSON response for true if sent successfully and false if not.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EmailSend([FromForm]string userName, int huntId)
        {
            var user = await _userRepo.ReadAsync(userName);
            var hunt = await _huntRepo.ReadAsync(huntId);
            if (user != null && hunt != null) //we only try to send an email if the hunt and user exists
            {
                string to = user.Email;
                string from = "buchunt69@gmail.com"; //this is the account we made to have as the sender for the emails
                string subject = "BucHunt Invite!";
                string body = $"Your access code to {hunt.HuntName} is: {user.AccessCode!.Code}\nYour URL is: {hunt.URL}";
                MailMessage message = new MailMessage(from, to, subject, body);
                SmtpClient client = new SmtpClient("smtp.elasticemail.com", 2525);
                // Credentials are necessary if the server requires the client 
                // to authenticate before it will send e-mail on the client's behalf.
                client.Credentials = new System.Net.NetworkCredential("buchunt69@gmail.com", "3650F08D120A1853AE0EA1F2F4DBF0EA674A");
                try
                {
                    client.Send(message);
                    Console.WriteLine("Email sent successfully!");
                    return Json(new { success = true });
                }
                catch (Exception ex) //exception might happen if it cannot access the server or other problems
                {
                    Console.WriteLine("Exception caught in CreateTestMessage1(): {0}",
                                ex.ToString());
                    return Json(new { success = false });
                }
            }
            else
            {
                return Json(new { success = false }); //returns false if the user or hunt does not exist
            }
        }
    }
}
