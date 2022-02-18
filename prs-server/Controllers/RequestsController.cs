using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prs_server.Models;

namespace prs_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return await _context.Requests.Include(x => x.User).ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
            var request = await _context.Requests.Include(x => x.User).Include(x => x.RequestLines).SingleOrDefaultAsync(x => x.ID == id);
            if (request == null)
            {
                return NotFound();
            }

            return request;
        }

        // GET api/Requests/5/review
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetReviews(int id)
        {
            var reviews = await _context.Requests.Where(x => x.Status == "REVIEW" && x.UserID != id).Include(x => x.User).Include(x => x.RequestLines).ToListAsync();
            return reviews;
        }

        // PUT: api/Requests/5/review
        [HttpPut("{id}")]
        public async Task<IActionResult> Review(int id)
        {
            Request request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            if(request.Total <= 50)
            {
                request.Status = "APPROVED";
            }
            else
            {
                request.Status = "REVIEW";
            }

            _context.SaveChanges();
            return NoContent();
        }

        // PUT: api/Requests/5/approve
        [HttpPut("{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            Request request = await _context.Requests.FindAsync(id);
            if(request == null)
            {
                return NotFound();
            }
            request.Status = "APPROVED";
            _context.SaveChanges();
            return NoContent();
        }

        // PUT: api/Requests/5/reject
        [HttpPut("{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            Request request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            request.Status = "REJECTED";
            _context.SaveChanges();
            return NoContent();
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.ID)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request)
        {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.ID }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.ID == id);
        }
    }
}
