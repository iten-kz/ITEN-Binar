using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.POCO
{
    public class Intruder
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string IIN { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [MaxLength]
        public string ImageUrl { get; set; }
        public string ImageBlobName { get; set; }

        public virtual ICollection<Fixation> Fixations { get; set; }
    }
}
