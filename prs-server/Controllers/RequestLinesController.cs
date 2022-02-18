﻿using System;
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
    public class RequestLinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestLinesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RequestLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestLine>>> GetRequestLines()
        {
            return await _context.RequestLines.Include(x => x.Product).Include(x => x.Request).ToListAsync();
        }

        // GET: api/RequestLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestLine>> GetRequestLine(int id)
        {
            var requestLine = await _context.RequestLines.Include(x => x.Product).Include(x => x.Request).SingleOrDefaultAsync();

            if (requestLine == null)
            {
                return NotFound();
            }

            return requestLine;
        }

        // PUT: api/RequestLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestLine(int id, RequestLine requestLine)
        {
            if (id != requestLine.ID)
            {
                return BadRequest();
            }

            _context.Entry(requestLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                RecalculateRequestTotal(requestLine.ID);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestLineExists(id))
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

        // POST: api/RequestLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RequestLine>> PostRequestLine(RequestLine requestLine)
        {
            _context.RequestLines.Add(requestLine);
            await _context.SaveChangesAsync();
            RecalculateRequestTotal(requestLine.ID);

            return CreatedAtAction("GetRequestLine", new { id = requestLine.ID }, requestLine);
        }

        // DELETE: api/RequestLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequestLine(int id)
        {
            var requestLine = await _context.RequestLines.FindAsync(id);
            if (requestLine == null)
            {
                return NotFound();
            }

            _context.RequestLines.Remove(requestLine);
            await _context.SaveChangesAsync();
            RecalculateRequestTotal(requestLine.ID);
            return NoContent();
        }

        private bool RequestLineExists(int id)
        {
            return _context.RequestLines.Any(e => e.ID == id);
        }

        private async void RecalculateRequestTotal(int requestID)
        {
            var request = await _context.Requests.FindAsync(requestID);
            request.Total = (from rl in _context.RequestLines join p in _context.Products
                             on rl.ProductID equals p.ID
                             where rl.RequestID == requestID
                             select new
                             {
                                 LineTotal = rl.Quantity * p.Price
                             }).Sum(x => x.LineTotal);
            _context.SaveChanges();
        }
    }
}
