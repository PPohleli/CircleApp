﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        
        // Foreign Keys
        public int PostId { get; set; }
        public int UserId { get; set; }
        
        // Navigation Properties
        public Post Post { get; set; }
        public User User { get; set; }
    }
}
