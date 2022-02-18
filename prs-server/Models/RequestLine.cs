using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace prs_server.Models
{
    public class RequestLine
    {
        public int ID { get; set; }
        public int RequestID { get; set; }
        public int ProductID { get; set; }
        [Required]
        public int Quantity { get; set; } = 0;

        [JsonIgnore]
        public virtual Request Request { get; set; }
        public virtual Product Product { get; set; }

        public RequestLine() { }
    }
}
