using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prs_server.Models
{
    public class Poline
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }

        public Poline() { }
        public Poline(string product, int quantity, decimal price, decimal lineTotal)
        {
            Product = product;
            Quantity = quantity;
            Price = price;
            LineTotal = lineTotal;
        }
    }
}
