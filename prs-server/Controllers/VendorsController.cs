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
    public class VendorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendorsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Vendors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors()
        {
            return await _context.Vendors.ToListAsync();
        }

        // GET: api/Vendors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return vendor;
        }

        // PUT: api/Vendors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendor(int id, Vendor vendor)
        {
            if (id != vendor.ID)
            {
                return BadRequest();
            }

            _context.Entry(vendor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorExists(id))
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

        // POST: api/Vendors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vendor>> PostVendor(Vendor vendor)
        {
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVendor", new { id = vendor.ID }, vendor);
        }

        // DELETE: api/Vendors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        private bool VendorExists(int id)
        {
            return _context.Vendors.Any(e => e.ID == id);
        }

        // GET: api/vendors/po/5
        [HttpGet("po/{vendorID}")]
        public async Task<ActionResult<Po>> CreatePO(int vendorID)
        {
            
            Po po = new Po();
            po.Vendor = await _context.Vendors.FindAsync(vendorID);

            var qResult = (
                      (from v in _context.Vendors
                       join p in _context.Products
                       on v.ID equals p.VendorID
                       join l in _context.RequestLines
                       on p.ID equals l.ProductID
                       join r in _context.Requests
                       on l.RequestID equals r.ID
                       where r.Status == "APPROVED"
                       && v.ID == vendorID
                       select new
                       {
                           p.ID,
                           Product = p.Name,
                           l.Quantity,
                           p.Price,
                           LineTotal = p.Price * l.Quantity
                       })) ;

            var sortedLines = new SortedList<int, Poline>();
            foreach(var line in qResult)
            {
                if (!sortedLines.ContainsKey(line.ID))
                {
                    var poline = new Poline()
                    {
                        Product = line.Product,
                        Quantity = 0,
                        Price = line.Price,
                        LineTotal = line.LineTotal
                    };
                    sortedLines.Add(line.ID, poline);
                }
                sortedLines[line.ID].Quantity += line.Quantity;
            };

            po.Polines = sortedLines.Values;
            po.PoTotal = po.Polines.Sum(x => x.LineTotal);

            //Po po = new Po(vendor, polines, poTotal);


            /*
            Vendor vendor = await _context.Vendors.FindAsync(vendorID);
            IEnumerable<Product> products = _context.Products.Where(x => x.VendorID == vendor.ID);
            IEnumerable<RequestLine> requestLines = (IEnumerable<RequestLine>)(from rl in _context.RequestLines
                                                    join p in products
                                                    on rl.ProductID equals p.ID
                                                    select new
                                                    {
                                                        rl.ID,
                                                        rl.ProductID,
                                                        rl.RequestID,
                                                        rl.Quantity
                                                    }).ToListAsync();
            IEnumerable<Request> requests = (IEnumerable<Request>)(
                                            from r in _context.Requests
                                            join rl in _context.RequestLines
                                            on r.ID equals rl.RequestID
                                            where r.Status == "APPROVED"
                                            select new
                                            {
                                                r.ID,
                                            }).ToListAsync();


            var purchaseOrder = (from v in _context.Vendors
                                 join p in _context.Products
                                 on v.ID equals p.VendorID
                                 join rl in _context.RequestLines
                                 on p.ID equals rl.ProductID
                                 join r in _context.Requests
                                 on rl.ProductID equals r.ID

                                 where r.Status == "APPROVED"

                                 select new
                                 {
                                     v.ID,
                                     v.Name,
                                     ProductID = p.ID,
                                     PartNumber = p.PartNbr,
                                     ProductName = p.Name,
                                     p.Price,
                                     rl.Quantity
                                 });
               
            var purchaseOrder = (from r in _context.Requests
                                 join rl in _context.RequestLines
                                 on r.ID equals rl.RequestID
                                 join p in _context.Products
                                 on rl.ProductID equals p.ID
                                 join v in _context.Vendors
                                 on p.VendorID equals v.ID

                                 where r.Status == "APPROVED"

                                 select new
                                 {
                                     v.ID,
                                     v.Name,
                                     ProductID = p.ID,
                                     PartNumber = p.PartNbr,
                                     ProductName = p.Name,
                                     p.Price,
                                     rl.Quantity
                                 });
            */

            return po;
        }


    }
}
