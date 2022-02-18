using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace prs_server.Models
{
    public class Product
    {
        public int ID { get; set; }
        [Required, StringLength(30)]
        public string PartNbr { get; set; }
        [Required, StringLength(30)]
        public string Name { get; set; }
        [Column(TypeName = "decimal(11,2)")]
        public Decimal Price { get; set; }
        [Required, StringLength(30)]
        public string Unit { get; set; }
        [StringLength(255)]
        public string PhotoPath { get; set; }

        [Required]
        public int VendorID { get; set; }
        public virtual Vendor Vendor { get; set; }

        public Product() { }
    }
}
