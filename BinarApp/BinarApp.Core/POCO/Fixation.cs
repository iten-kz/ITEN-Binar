using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.POCO
{
    public class Fixation
    {        
        [Key]
        public int Id { get; set; }
        public string PUNKT { get; set; }
        public string EntityId { get; set; }
        [Required]
        public DateTime FixationDate { get; set; }
        [Required]
        public decimal PenaltySum { get; set; }
        public string SY1 { get; set; }
        public string R05 { get; set; }
        [Required]
        public string GRNZ { get; set; }
        public string VU { get; set; }
        public string PDD { get; set; }
        [MaxLength(4096)]
        public string Description { get; set; }       
        public int Speed { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [MaxLength]
        public string Image { get; set; }

        [MaxLength(50)]
        public string NickName { get; set; }

        public int? EquipmentId { get; set; }
        public virtual Equipment Equipment { get; set; }

        //public int? IntruderId { get; set; }
        //public virtual Intruder Intruder { get; set; }

        public virtual ICollection<FixationDetail> FixationDetails { get; set; }
    }
}
