using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BinarApp.API.Models
{
    public class ImageInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Type { get; set; }

        public string Speed { get; set; }

        public string Threshold { get; set; }

        public DateTime? Date { get; set; }

        public string SDNumber { get; set; }

        public string Checksum { get; set; }

        public string Direction { get; set; }

        public byte[] Data { get; set; }

        [ForeignKey("Story")]
        public long SessionId { get; set; }

        public virtual Story Story { get; set; }
    }
}