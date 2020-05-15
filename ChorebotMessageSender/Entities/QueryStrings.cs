using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChorebotMessageSender.Entities
{
    public partial class QueryStrings
    {
        [Key]
        public int QueryStringId { get; set; }
        public int CallId { get; set; }
        [Required]
        [StringLength(50)]
        public string KeyString { get; set; }
        [Required]
        [StringLength(200)]
        public string ValueString { get; set; }
        public bool IsActive { get; set; }
    }
}
