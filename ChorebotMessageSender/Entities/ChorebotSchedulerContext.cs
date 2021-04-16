using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ChorebotMessageSender.Entities
{
    public partial class ChorebotSchedulerContext : DbContext
    {
        public ChorebotSchedulerContext()
        {
        }

        public ChorebotSchedulerContext(DbContextOptions<ChorebotSchedulerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Calls> Calls { get; set; }
        public virtual DbSet<IntervalTypes> IntervalTypes { get; set; }
        public virtual DbSet<QueryStrings> QueryStrings { get; set; }
        public virtual DbSet<ScheduledTasks> ScheduledTasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer("Server=RYAN-PC\\SQLEXPRESS;Database=ChorebotScheduler;Trusted_Connection=True;MultipleActiveResultSets=True;");
                optionsBuilder.UseSqlServer("Server=198.71.227.2;Database=ChorebotScheduler;MultipleActiveResultSets=True;user id=RyanCaigoy;password=PoppaCags1234$;");

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
