using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScavengeRUs.Data;
using ScavengeRUs.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using ScavengeRUs.Services;

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
    public class LocationsController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IHuntRepository _huntRepo;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// This method injects the context into the class so that the methods can connect to
        /// the database
        /// </summary>
        /// <param name="context"></param>
        public LocationsController(ApplicationDbContext context, IHuntRepository huntRepo, IUserRepository userRepo)
        {
            _userRepo = userRepo;
            _huntRepo = huntRepo;
            _context = context;
        }

        /// <summary>
        /// This method maps to the Views/Locations/Index view. It shows the table of all locations in the database. Only an admin can call this method.
        /// </summary>
        /// <returns>Views/Locations/Index.cshtml</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
              return View(await _context.Location.ToListAsync()); //passes a list of all locations from the database as the model for the view
        }

        /// <summary>
        /// This method maps to the Views/Locations/Details view. The parameter is the id of the location
        /// and it gets this from the URL. Only an admin can call this method. The view for this just
        /// shows more information about the given location.
        /// </summary>
        /// <returns>Views/Locations/Details.cshtml, or will return a 404 error if the location or id does not exist</returns>
        /// 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Location == null)
            {
                return NotFound();
            }

            var location = await _context.Location
                .FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location); //passes the location object as the model for the view
        }

        /// <summary>
        /// This maps to the Views/Locations/Create view. Can only be called by an admin.
        /// This is the page that will be shown when the admin want to create a new location from the 
        /// website.
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "Admin")]
        public IActionResult Create([Bind(Prefix = "Id")] int huntid)
        {
            return View();
        }

        /// <summary>
        /// This method handles the post request from the Views/Locations/Create view which has a form.
        /// The parameters are what is read from the form when this method is called. This can only be called by an admin.
        /// </summary>
        /// <returns>Views/Hunt/MaangeTasks.cshtml, or will redirect to Views/Locations/Create.cshtml if the hunt id is 0</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind(Prefix = "Id")] int huntid, Location location)
        {
            if (ModelState.IsValid)
            {
                _context.Add(location);
                await _context.SaveChangesAsync();
                if (huntid != 0)
                {
                    return RedirectToAction("ManageTasks", "Hunt", new { id = huntid });
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location); //passes the location object as the model for the view
        }

        /// <summary>
        /// This maps to the Views/Locations/Edit.cshtml. Can only be called by an admin.
        /// The parameter is read from the URL which is the id of the location to edit.
        /// This is the page that will be shown when the admin attempts to edit a location
        /// </summary>
        /// <returns>Views/Locations/Edit.cshtml, will return a 404 error if the id is null or if the location does not exist</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Location == null)
            {
                return NotFound(); //returns a 404 error
            }

            var location = await _context.Location.FindAsync(id);
            if (location == null)
            {
                return NotFound(); //returns a 404 error
            }
            return View(location); //passes the location object as the model for the view
        }

        /// <summary>
        /// This method handles the post request from the Views/Locations/Edit.cshtml view which has a form.
        /// It can only be called by an admin. The parameters are read from the form when this method is called.
        /// This method will be called when the admin submits the edit to the location from the previous method.
        /// </summary>
        /// <returns>Views/Location/Index.cshtml, will return to Views/Locations/Edit.cshtml if the model isn't valid. 
        /// Will also return a 404 error if the id does not match up with a current id.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HuntId,Place,Lat,Lon,Task,AccessCode,QRCode,Answer")] Location location)
        {
            if (id != location.Id)
            {
                return NotFound(); //returns a 404 error
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.Id))
                    {
                        return NotFound(); //returns a 404 error
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location); //passes the location object as the model for the view
        }

        /// <summary>
        /// This method maps to the Views/Locations/Delete view. This can only be called from an admin.
        /// The parameter is the id of the location to delete and is received from the url when this method is called.
        /// This page will be shown when the admin attempts to delete a location from the site
        /// </summary>
        /// <returns>Views/Locations/Delete.cshtml, or will return a 404 error if the id is null or the location does not exist</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Location == null)
            {
                return NotFound(); //returns a 404 error
            }

            var location = await _context.Location
                .FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound(); //returns a 404 error
            }

            return View(location); //passes the location object as the model for the view
        }

        /// <summary>
        /// This method handles the post request from the Views/Locations/Delete view which has a form.
        /// The parameter is an id for the location which is read from the form when this method is called.
        /// This can only be called by an admin.
        /// This method will activate when the admin submits the delete action on a location
        /// </summary>
        /// <returns>Views/Locations/Index.cshtml, or will return a problem is something goes wrong</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Location == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Location'  is null.");
            }
            var location = await _context.Location.FindAsync(id);
            if (location != null)
            {
                _context.Location.Remove(location);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// This method checks to make sure that a Location row exists in the database.
        /// The id is the primary key of the location objects in the database.
        /// </summary>
        /// <returns>true if the location exists, false if not</returns>
        private bool LocationExists(int id)
        {
          return _context.Location.Any(e => e.Id == id);
        }

        /// <summary>
        /// This handles the post request for when a user submits an answer for a task. This can only be called by admins and players.
        /// This method validates an answer for a task. Used by an AJAX call from the hunt page. See site.js
        /// </summary>
        /// <param name="id"></param>
        /// <param name="taskid"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin, Player")]
        public async Task<IActionResult> ValidateAnswer([FromForm]int id, int taskid, string answer)
        {
            var currentUser = await _userRepo.ReadAsync(User.Identity?.Name!);                              //gets current user
            var location = await _context.Location.FirstOrDefaultAsync(m => m.Id == taskid);                //gets the task
            if (answer != null && answer.Equals(location?.Answer, StringComparison.OrdinalIgnoreCase))      //check is answer matches
            {
                currentUser?.TasksCompleted!.Add(location); //Update the players completed tasks
                await _context.SaveChangesAsync();          
                return Json(new { success = true});
            }
            return Json(new { success = false });
        }
    }
}
