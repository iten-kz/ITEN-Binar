using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BinarApp.API.Models
{
    public class Story
    {
        [Key]
        public long SessionId { get; set; }

        public string DeviceType { get; set; }

        public string DeviceNumber { get; set; }

        public DateTime? Date { get; set; }

        public string Street { get; set; }
    }    
}