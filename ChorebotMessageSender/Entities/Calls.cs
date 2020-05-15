using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChorebotMessageSender.Entities
{
    public partial class Calls
    {
        [Key]
        public int CallId { get; set; }
        [Required]
        [Column("URL")]
        [StringLength(300)]
        public string Url { get; set; }
        [Required]
        [StringLength(10)]
        public string CallType { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateStart { get; set; }
        public int IntervalTypeId { get; set; }
        public int Interval { get; set; }
    }
}
