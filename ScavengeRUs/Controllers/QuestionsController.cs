using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScavengeRUs.Data;

namespace ScavengeRUs.Controllers
{
    /*** I don't think this controller is being used for anything. Consider removing it in the future. ***/
    /// <summary>
    /// This file is a controller, that means it handles HTTP requests from the client, each method listed below
    /// can be called while the application is running by using this syntax in the URL: /{ControllerName}/{MethodName}. For example,
    /// to call the Index method of the Home controller, it would be /Home/Index<br></br>
    /// A method handles HTTP GET requests by default, to handle other requests, it will have a tag that specifies so above it.
    /// Most of these methods also return a view, which is the HTML file the client will recieve. You will find these files in the 
    /// Views folder of the project. They match up with the controller and method name they go to. For example, the view for the
    /// Index page in the Home controller is in the Views/Home/Index.cshtml directory.
    /// </summary>
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// This constructor takes in a context so that the controller has access to the database
        /// </summary>
        /// <param name="context"></param>
        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This view will show on the /Questions page. It passes a List of the items from the Location
        /// Model to that page which will show each location with the question associated with it
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Location.ToListAsync());
        }
    }
}
