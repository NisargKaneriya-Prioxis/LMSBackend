using EA.Model.SpDbContext;
using Microsoft.EntityFrameworkCore;

namespace LM.Model.SpDbContext;

public partial class LMSSpContext:DbContext
{
                public LMSSpContext() { }
        public LMSSpContext(DbContextOptions<LMSSpContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ExecutreStoreProcedureResult> ExecutreStoreProcedureResult { get; set; }
        public virtual DbSet<ExecutreStoreProcedureResultWithSID> ExecutreStoreProcedureResultWithSID { get; set; }
        public virtual DbSet<ExecuteStoreProcedureResultWithId> ExecuteStoreProcedureResultWithId { get; set; }
        public virtual DbSet<ExecutreStoreProcedureResultList> ExecutreStoreProcedureResultList { get; set; }
        public virtual DbSet<ExecutreStoreProcedureResultWithEntitySID> ExecutreStoreProcedureResultWithEntitySID { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region SP           
            modelBuilder.Entity<ExecutreStoreProcedureResult>().HasNoKey().Property(e => e.ErrorMessage).HasConversion<System.String>();

            modelBuilder.Entity<ExecutreStoreProcedureResultWithSID>().HasNoKey().Property(e => e.ErrorMessage).HasConversion<System.String>();
            modelBuilder.Entity<ExecutreStoreProcedureResultWithSID>().HasNoKey().Property(e => e.SID).HasConversion<System.String>();
            modelBuilder.Entity<ExecuteStoreProcedureResultWithId>().HasNoKey().Property(e => e.ErrorMessage).HasConversion<string>();
            modelBuilder.Entity<ExecuteStoreProcedureResultWithId>().HasNoKey().Property(e => e.Id).HasConversion<int>();
            modelBuilder.Entity<ExecutreStoreProcedureResultList>().HasNoKey().Property(e => e.ErrorMessage).HasConversion<System.String>();
            modelBuilder.Entity<ExecutreStoreProcedureResultWithEntitySID>().HasNoKey().Property(e => e.ErrorMessage).HasConversion<System.String>();


            #endregion

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}