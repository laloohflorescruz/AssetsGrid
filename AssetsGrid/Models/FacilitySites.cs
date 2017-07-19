using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AssetsGrid.Models
{
    public class FacilitySites
    {
        [Key]
        public Guid FacilitySiteID { get; set; }
        public string FacilityName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }

 
    }
}