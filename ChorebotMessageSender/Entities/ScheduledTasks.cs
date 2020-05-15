using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChorebotMessageSender.Entities
{
    public partial class ScheduledTasks
    {
        [Key]
        public int ScheduledTaskId { get; set; }
        public int CallId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateScheduled { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateSent { get; set; }
        public bool IsSent { get; set; }
        [StringLength(30)]
        public string ErrorLog { get; set; }
    }
}
