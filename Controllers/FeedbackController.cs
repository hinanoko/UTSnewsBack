using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackModels feedback)
        {
            if (feedback == null)
            {
                return BadRequest("Feedback cannot be null.");
            }

            // Perform any additional validations if necessary, e.g.:
            // if (string.IsNullOrEmpty(feedback.TeacherEmail) || string.IsNullOrEmpty(feedback.FeedbackMessage))
            // {
            //     return BadRequest("Email and message are required.");
            // }

            try
            {
                await _context.Feedbacks.AddAsync(feedback);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(CreateFeedback), new { id = feedback.Id }, feedback);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/feedback
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackModels>>> GetAllFeedbacks()
        {
            var feedbacks = await _context.Feedbacks.ToListAsync();
            return Ok(feedbacks);
        }

        // Optional: Method to get feedback by ID
        // GET: api/feedback/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackModels>> GetFeedbackById(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            return Ok(feedback);
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetFeedbackByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var feedbacks = await _context.Feedbacks
                .Where(f => f.TeacherEmail == email) // Filter by teacher's email
                .ToListAsync();

            return Ok(feedbacks);
        }

    }
}
