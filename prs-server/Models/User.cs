using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace prs_server.Models
{
    public class User
    {
        public int ID { get; set; }
        [Required, StringLength(30)]
        public string Username { get; set; }
        [Required, StringLength(30)]
        public string Password { get; set; }
        [Required, StringLength(30)]
        public string Firstname { get; set; }
        [Required, StringLength(30)]
        public string Lastname { get; set; }
        [StringLength(12)]
        public string Phone { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
        public bool IsReviewer { get; set; }
        public bool IsAdmin { get; set; }

    }
}
