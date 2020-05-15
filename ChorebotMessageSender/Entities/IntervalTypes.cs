using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChorebotMessageSender.Entities
{
    public partial class IntervalTypes
    {
        [Key]
        public int IntervalTypeId { get; set; }
        [Required]
        [StringLength(20)]
        public string IntervalTypeName { get; set; }
    }
}
