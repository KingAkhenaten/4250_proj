﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengeRUs.Models.Entities;
using ScavengeRUs.Services;
using System.Collections.Immutable;
using System.Net.Mail;

namespace ScavengeRUs.Controllers
{
    /// <summary>
    /// This class is the controller for any page realted to hunts
    /// </summary>
    public class HuntController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IHuntRepository _huntRepo;

        /// <summary>
        /// Injecting the user repository and hunt repository (Db classes)
        /// </summary>
        /// <param name="userRepo"></param>
        /// <param name="HuntRepo"></param>
        public HuntController(IUserRepository userRepo, IHuntRepository HuntRepo)
        {
            _userRepo = userRepo;
            _huntRepo = HuntRepo;
        }
        /// <summary>
        /// www.localhost.com/hunt/index Returns a list of all hunts
        /// </summary>
        /// <returns></returns>
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
        /// www.localhost.com/hunt/create This is the get method for creating a hunt
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// www.localhost.com/hunt/create This is the post method for creating a hunt
        /// </summary>
        /// <param name="hunt"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Hunt hunt)
        {
            hunt.URL = $"Hunt/ViewTasks/{hunt.Id.ToString()}";
            if (ModelState.IsValid)
            {
                await _huntRepo.CreateAsync(hunt);
                return RedirectToAction("Index");
            }
            return View(hunt);

        }

        /// <summary>
        /// www.localhost.com/hunt/details/{huntId} This is the details view of a hunt
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details([Bind(Prefix = "Id")] int huntId)
        {
            if (huntId == 0)
            {
                return RedirectToAction("Index");
            }
            var hunt = await _huntRepo.ReadAsync(huntId);
            if (hunt == null)
            {
                return RedirectToAction("Index");
            }
            return View(hunt);
        }

        /// <summary>
        /// www.localhost.com/hunt/delete/{huntId} This is the get method for deleting a hunt
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([Bind(Prefix = "Id")] int huntId)
        {
            if (huntId == 0)
            {
                return RedirectToAction("Index");
            }
            var hunt = await _huntRepo.ReadAsync(huntId);
            if (hunt == null)
            {
                return RedirectToAction("Index");
            }
            return View(hunt);
        }
        /// <summary>
        /// www.localhost.com/hunt/delete/{huntId} This is the post method for deleteing a hunt.
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([Bind(Prefix = "id")] int huntId)
        {
            await _huntRepo.DeleteAsync(huntId);
            return RedirectToAction("Index");
        }
        /// <summary>
        /// www.localhost.com/hunt/viewplayers/{huntId} Returns a list of all players in a specified hunt
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewPlayers([Bind(Prefix = "Id")] int huntId)
        {
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntId);
            ViewData["Hunt"] = hunt;
            if (hunt == null)
            {
                return RedirectToAction("Index");
            }

            return View(hunt.Players);
        }
        
        /// <summary>
        /// www.localhost.com/hunt/addplayertohunt{huntid} Get method for adding a player to a hunt. 
        /// </summary>
        /// <param name="huntId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPlayerToHunt([Bind(Prefix = "Id")] int huntId)
        {
            var hunt = await _huntRepo.ReadAsync(huntId);
            ViewData["Hunt"] = hunt;
            return View();

        }
        /// <summary>
        /// www.localhost.com/hunt/addplayertohunt{huntid} Post method for the form submission. This creates a user and assigns the access code for the hunt. 
        /// </summary>
        /// <param name="huntId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddPlayerToHunt([Bind(Prefix = "Id")] int huntId, ApplicationUser user)
        {

            if (huntId == 0)
            {
                RedirectToAction("Index");
            }
            var hunt = await _huntRepo.ReadAsync(huntId);
            var existingUser = await _userRepo.ReadAsync(user.Email);
            var newUser = new ApplicationUser();
            if (existingUser == null)
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
                    Hunt = hunt,                        //Setting foriegn key
                    Code = $"{newUser.PhoneNumber}/{hunt.HuntName!.Replace(" ", string.Empty)}",            //This is the access code generation
                };
                newUser.AccessCode.Users.Add(newUser);  //Setting foriegn key
            }
            else
            {
                newUser.AccessCode = new AccessCode()
                {
                    Hunt = hunt,
                    Code = newUser.AccessCode.Code,
                };
                newUser.AccessCode.Users.Add(newUser);
            }
            await _huntRepo.AddUserToHunt(huntId, newUser); //This methods adds the user to the database and adds the database relationship to a hunt.
      
            return RedirectToAction("Index");
        }
        /// <summary>
        /// www.localhost.com/hunt/removeuser/{username}/{huntId} This is the get method for removing a user from a hunt.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUser([Bind(Prefix = "Id")] string username, [Bind(Prefix = "huntId")] int huntid)
        {
            ViewData["Hunt"] = huntid;
            var user = await _userRepo.ReadAsync(username);
            return View(user);

        }
        /// <summary>
        /// www.localhost.com/hunt/removeuser/{username}/{huntId} This is the post method for removing a user from a hunt.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RemoveUserConfirmed(string username, int huntid)
        {
            await _huntRepo.RemoveUserFromHunt(username, huntid);
            return RedirectToAction("Index");

        }
        /// <summary>
        /// This method generates a view of all task associated with a hunt. Pasing the huntid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        public async Task<IActionResult> ViewTasks([Bind(Prefix = "Id")] int huntid)
        {
            if(!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            var currentUser = await _userRepo.ReadAsync(User.Identity?.Name!);
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntid);
            ViewData["Hunt"] = hunt;
            ViewData["Player"] = currentUser;
            if (hunt == null)
            {
                return RedirectToAction("Index");
            }

            if (DateTime.Now > hunt.EndDate) //We are passing a variable here to check if the hunt has already ended
            {
                ViewBag.HasEnded = true; //true if it has ended
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
        /// This method shows all tasks that can be added to the hunt. Exculding the tasks that are already added
        /// </summary>
        /// <param name="huntid"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageTasks([Bind(Prefix = "Id")] int huntid)
        {
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntid);
            //var existingLocations = await _huntRepo.GetLocations(hunt.HuntLocations);

            ViewData["Hunt"] = hunt;

            if (DateTime.Now > hunt.EndDate)
            {
                return RedirectToAction("ViewTasks", new { id = hunt.Id }); //redirect if the hunt has already ended
            }

            var allLocations = await _huntRepo.GetAllLocations();
            //var locations = allLocations.Except(existingLocations);
            return View(allLocations);
        }
        /// <summary>
        /// This method is the post method for adding a task. This gets executed when you click "Add Task"
        /// </summary>
        /// <param name="id"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTask(int id, int huntid)
        {
            var hunt = await _huntRepo.ReadHuntWithRelatedData(huntid);
            ViewData["Hunt"] = hunt;
            await _huntRepo.AddLocation(id, huntid);
            return RedirectToAction("ManageTasks", new { id = huntid });
        }
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
        /// This is the post method for removing a task. This is executed when you click "Remove" from the Hunt/RemoveTask screen
        /// </summary>
        /// <param name="id"></param>
        /// <param name="huntid"></param>
        /// <returns></returns>
        public async Task<IActionResult> RemoveTask(int id, int huntid)
        {
            await _huntRepo.RemoveTaskFromHunt(id, huntid);
            return RedirectToAction("ManageTasks", "Hunt", new { id = huntid });
        }

        public async Task<IActionResult> Scoreboard([Bind(Prefix = "id")] int huntid)
        {
            var hunt = await _huntRepo.ReadAsync(huntid);
            if (hunt == null)
            {
                return RedirectToAction("ViewTasks", new { id = huntid });
            }
            ViewData["hunt"] = hunt;
            ViewData["player"] = await _userRepo.ReadAsync(User.Identity?.Name!);
            var players = hunt.Players;
            return View(players);
        }

        [HttpPost]
        public async Task<IActionResult> EmailSend(string userName, int huntId)
        {
            var user = await _userRepo.ReadAsync(userName);
            var hunt = await _huntRepo.ReadAsync(huntId);

            string to = user.Email;
            string from = "chrisseals9893@gmail.com";
            string subject = "BucHunt Invite!";
            string body = $"Your access code to {hunt.HuntName} is: {user.AccessCode!.Code}\nYour URL is: BADLINK";
            MailMessage message = new MailMessage(from, to, subject, body);
            SmtpClient client = new SmtpClient("smtp.elasticemail.com", 2525);
            // Credentials are necessary if the server requires the client 
            // to authenticate before it will send e-mail on the client's behalf.
            client.Credentials = new System.Net.NetworkCredential("chrisseals9893@gmail.com", "435E009F769EBB649B563DA092D862EC2439");
            try
            {
                client.Send(message);
                Console.WriteLine("Email sent successfully!");
                    return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage1(): {0}",
                            ex.ToString());
                return Json(new { success = false });
            }
        }
    }
}
