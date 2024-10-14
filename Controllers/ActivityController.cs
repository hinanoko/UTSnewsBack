using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Activity
        // This method is used to post (create) a new activity news item
        [HttpPost]
        public async Task<IActionResult> PostActivity([FromBody] ActivityModels newActivity)
        {
            if (newActivity == null || string.IsNullOrEmpty(newActivity.Title) || string.IsNullOrEmpty(newActivity.Content))
            {
                return BadRequest("Invalid activity data.");
            }

            // Add the new activity to the database
            _context.Activities.Add(newActivity);
            await _context.SaveChangesAsync();

            return Ok(newActivity);
        }

        // GET: api/Activity
        // This method is used to retrieve all activity news items
        [HttpGet]
        public IActionResult GetAllActivities()
        {
            // Fetch all activities from the database
            var activities = _context.Activities.ToList();

            if (activities == null || activities.Count == 0)
            {
                return NotFound("No activities found.");
            }

            return Ok(activities);
        }


        // DELETE: api/Activity/{id}
        // This method is used to delete an activity news item by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            // Find the activity by ID
            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
            {
                return NotFound($"Activity with ID {id} not found.");
            }

            // Remove the activity from the database
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return Ok($"Activity with ID {id} has been deleted successfully.");
        }
    }
}
