using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScavengeRUs.Data;

namespace ScavengeRUs.Controllers
{
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
