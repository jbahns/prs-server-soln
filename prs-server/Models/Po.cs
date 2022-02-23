using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prs_server.Models
{
    public class Po
    {
        public Vendor Vendor { get; set; }
        public IEnumerable<Poline> Polines { get; set; }
        public decimal PoTotal { get; set; }

        public Po() { }
        public Po(Vendor vendor, IEnumerable<Poline> polines, decimal poTotal)
        {
            Vendor = vendor;
            Polines = polines;
            PoTotal = poTotal;
        }
    }
}
