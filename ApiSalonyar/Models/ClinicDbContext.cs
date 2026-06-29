using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApiSalonyar.Models
{
    public partial class ClinicDbContext : DbContext
    {
        public ClinicDbContext()
        {
        }

        public ClinicDbContext(DbContextOptions<ClinicDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Allergy> Allergies { get; set; } = null!;
        public virtual DbSet<BloodType> BloodTypes { get; set; } = null!;
        public virtual DbSet<Branch> Branches { get; set; } = null!;
        public virtual DbSet<ConsentFormType> ConsentFormTypes { get; set; } = null!;
        public virtual DbSet<Disease> Diseases { get; set; } = null!;
        public virtual DbSet<Gender> Genders { get; set; } = null!;
        public virtual DbSet<MaritalStatus> MaritalStatuses { get; set; } = null!;
        public virtual DbSet<Patient> Patients { get; set; } = null!;
        public virtual DbSet<PatientAllergy> PatientAllergies { get; set; } = null!;
        public virtual DbSet<PatientConsentForm> PatientConsentForms { get; set; } = null!;
        public virtual DbSet<PatientDisease> PatientDiseases { get; set; } = null!;
        public virtual DbSet<PatientImage> PatientImages { get; set; } = null!;
        public virtual DbSet<PatientVisit> PatientVisits { get; set; } = null!;
        public virtual DbSet<Permission> Permissions { get; set; } = null!;
        public virtual DbSet<Profession> Professions { get; set; } = null!;
        public virtual DbSet<ReferralSource> ReferralSources { get; set; } = null!;
        public virtual DbSet<ReservationStatus> ReservationStatuses { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<RoomReservation> RoomReservations { get; set; } = null!;
        public virtual DbSet<RoomUsageType> RoomUsageTypes { get; set; } = null!;
        public virtual DbSet<Treatment> Treatments { get; set; } = null!;
        public virtual DbSet<TreatmentGroup> TreatmentGroups { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserPermission> UserPermissions { get; set; } = null!;
        public virtual DbSet<staff> staff { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=ClinicDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Allergy>(entity =>
            {
                entity.ToTable("Allergy", "Core");

                entity.Property(e => e.Title).HasMaxLength(150);
            });

            modelBuilder.Entity<BloodType>(entity =>
            {
                entity.ToTable("BloodType", "Core");

                entity.Property(e => e.Title).HasMaxLength(10);
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("Branch", "Core");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.Title).HasMaxLength(150);
            });

            modelBuilder.Entity<ConsentFormType>(entity =>
            {
                entity.ToTable("ConsentFormType", "Doc");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<Disease>(entity =>
            {
                entity.ToTable("Disease", "Core");

                entity.Property(e => e.Title).HasMaxLength(150);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.ToTable("Gender", "Core");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<MaritalStatus>(entity =>
            {
                entity.ToTable("MaritalStatus", "Core");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient", "Clinic");

                entity.HasIndex(e => e.NationalCode, "UQ__Patient__3DFA410604D98819")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.EmergencyContactName).HasMaxLength(150);

                entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Mobile).HasMaxLength(20);

                entity.Property(e => e.NationalCode).HasMaxLength(15);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.ProfileImagePath).HasMaxLength(500);

                entity.Property(e => e.ReferralDate).HasColumnType("date");

                entity.HasOne(d => d.BloodType)
                    .WithMany(p => p.Patients)
                    .HasForeignKey(d => d.BloodTypeId)
                    .HasConstraintName("FK__Patient__BloodTy__656C112C");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Patients)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient__BranchI__619B8048");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Patients)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("FK__Patient__GenderI__628FA481");

                entity.HasOne(d => d.MaritalStatus)
                    .WithMany(p => p.Patients)
                    .HasForeignKey(d => d.MaritalStatusId)
                    .HasConstraintName("FK__Patient__Marital__6477ECF3");

                entity.HasOne(d => d.ReferralSource)
                    .WithMany(p => p.Patients)
                    .HasForeignKey(d => d.ReferralSourceId)
                    .HasConstraintName("FK__Patient__Referra__6383C8BA");
            });

            modelBuilder.Entity<PatientAllergy>(entity =>
            {
                entity.HasKey(e => new { e.PatientId, e.AllergyId })
                    .HasName("PK__PatientA__AD472882E2625BBB");

                entity.ToTable("PatientAllergy", "Clinic");

                entity.Property(e => e.Note).HasMaxLength(300);

                entity.HasOne(d => d.Allergy)
                    .WithMany(p => p.PatientAllergies)
                    .HasForeignKey(d => d.AllergyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientAl__Aller__6B24EA82");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientAllergies)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientAl__Patie__6A30C649");
            });

            modelBuilder.Entity<PatientConsentForm>(entity =>
            {
                entity.HasKey(e => e.FormId)
                    .HasName("PK__PatientC__FB05B7DD58170DFB");

                entity.ToTable("PatientConsentForm", "Doc");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.FilePath).HasMaxLength(500);

                entity.Property(e => e.SignedDate).HasColumnType("date");

                entity.HasOne(d => d.ConsentFormType)
                    .WithMany(p => p.PatientConsentForms)
                    .HasForeignKey(d => d.ConsentFormTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientCo__Conse__2A164134");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientConsentForms)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientCo__Patie__29221CFB");

                entity.HasOne(d => d.UploadedByUser)
                    .WithMany(p => p.PatientConsentForms)
                    .HasForeignKey(d => d.UploadedByUserId)
                    .HasConstraintName("FK__PatientCo__Uploa__2B0A656D");
            });

            modelBuilder.Entity<PatientDisease>(entity =>
            {
                entity.HasKey(e => new { e.PatientId, e.DiseaseId })
                    .HasName("PK__PatientD__1195905EF5B72409");

                entity.ToTable("PatientDisease", "Clinic");

                entity.Property(e => e.Note).HasMaxLength(300);

                entity.HasOne(d => d.Disease)
                    .WithMany(p => p.PatientDiseases)
                    .HasForeignKey(d => d.DiseaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientDi__Disea__6EF57B66");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientDiseases)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientDi__Patie__6E01572D");
            });

            modelBuilder.Entity<PatientImage>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK__PatientI__7516F70C792CE5F8");

                entity.ToTable("PatientImage", "Clinic");

                entity.Property(e => e.AfterImagePath).HasMaxLength(500);

                entity.Property(e => e.BeforeImagePath).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasOne(d => d.Visit)
                    .WithMany(p => p.PatientImages)
                    .HasForeignKey(d => d.VisitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientIm__Visit__797309D9");
            });

            modelBuilder.Entity<PatientVisit>(entity =>
            {
                entity.HasKey(e => e.VisitId)
                    .HasName("PK__PatientV__4D3AA1DED81C3C42");

                entity.ToTable("PatientVisit", "Clinic");

                entity.HasIndex(e => e.PatientId, "IX_PatientVisit_PatientId");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.VisitDate).HasColumnType("date");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.PatientVisits)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientVi__Branc__72C60C4A");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientVisits)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientVi__Patie__71D1E811");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.PatientVisits)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientVi__Staff__74AE54BC");

                entity.HasOne(d => d.Treatment)
                    .WithMany(p => p.PatientVisits)
                    .HasForeignKey(d => d.TreatmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientVi__Treat__73BA3083");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission", "Security");

                entity.HasIndex(e => e.Code, "UQ__Permissi__A25C5AA7700B8C3E")
                    .IsUnique();

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.ModuleName).HasMaxLength(100);

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<Profession>(entity =>
            {
                entity.ToTable("Profession", "Core");

                entity.Property(e => e.Title).HasMaxLength(150);
            });

            modelBuilder.Entity<ReferralSource>(entity =>
            {
                entity.ToTable("ReferralSource", "Core");

                entity.Property(e => e.Title).HasMaxLength(150);
            });

            modelBuilder.Entity<ReservationStatus>(entity =>
            {
                entity.ToTable("ReservationStatus", "Core");

                entity.Property(e => e.ColorHex)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "Security");

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasMany(d => d.Permissions)
                    .WithMany(p => p.Roles)
                    .UsingEntity<Dictionary<string, object>>(
                        "RolePermission",
                        l => l.HasOne<Permission>().WithMany().HasForeignKey("PermissionId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__RolePermi__Permi__208CD6FA"),
                        r => r.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__RolePermi__RoleI__1F98B2C1"),
                        j =>
                        {
                            j.HasKey("RoleId", "PermissionId").HasName("PK__RolePerm__6400A1A8A90C75C9");

                            j.ToTable("RolePermission", "Security");
                        });
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room", "Scheduling");

                entity.Property(e => e.Area).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Room__BranchId__7E37BEF6");

                entity.HasOne(d => d.RoomUsageType)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.RoomUsageTypeId)
                    .HasConstraintName("FK__Room__RoomUsageT__7F2BE32F");
            });

            modelBuilder.Entity<RoomReservation>(entity =>
            {
                entity.HasKey(e => e.ReservationId)
                    .HasName("PK__RoomRese__B7EE5F24DFF9A87E");

                entity.ToTable("RoomReservation", "Scheduling");

                entity.HasIndex(e => new { e.ReservationDate, e.RoomId }, "IX_RoomReservation_Date_Room");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.ReservationDate).HasColumnType("date");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.RoomReservations)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RoomReser__Branc__04E4BC85");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.RoomReservations)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("FK_RoomReservation_CreatedByUser");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.RoomReservations)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RoomReser__Patie__05D8E0BE");

                entity.HasOne(d => d.ReservationStatus)
                    .WithMany(p => p.RoomReservations)
                    .HasForeignKey(d => d.ReservationStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RoomReser__Reser__09A971A2");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomReservations)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RoomReser__RoomI__07C12930");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.RoomReservations)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RoomReser__Staff__06CD04F7");

                entity.HasOne(d => d.Treatment)
                    .WithMany(p => p.RoomReservations)
                    .HasForeignKey(d => d.TreatmentId)
                    .HasConstraintName("FK__RoomReser__Treat__08B54D69");

                entity.HasOne(d => d.Visit)
                    .WithMany(p => p.RoomReservations)
                    .HasForeignKey(d => d.VisitId)
                    .HasConstraintName("FK__RoomReser__Visit__0A9D95DB");
            });

            modelBuilder.Entity<RoomUsageType>(entity =>
            {
                entity.ToTable("RoomUsageType", "Core");

                entity.Property(e => e.Title).HasMaxLength(150);
            });

            modelBuilder.Entity<Treatment>(entity =>
            {
                entity.ToTable("Treatment", "Clinic");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Title).HasMaxLength(150);

                entity.HasOne(d => d.TreatmentGroup)
                    .WithMany(p => p.Treatments)
                    .HasForeignKey(d => d.TreatmentGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Treatment__Treat__52593CB8");
            });

            modelBuilder.Entity<TreatmentGroup>(entity =>
            {
                entity.ToTable("TreatmentGroup", "Clinic");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Title).HasMaxLength(150);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "Security");

                entity.HasIndex(e => e.Username, "UQ__User__536C85E4A73DCB5C")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PasswordHash).HasMaxLength(256);

                entity.Property(e => e.Username).HasMaxLength(100);

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK__User__StaffId__10566F31");

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__UserRole__RoleId__1CBC4616"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__UserRole__UserId__1BC821DD"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF2760AD21CD050F");

                            j.ToTable("UserRole", "Security");
                        });
            });

            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.PermissionId })
                    .HasName("PK__UserPerm__F972A3FE893F1D22");

                entity.ToTable("UserPermission", "Security");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.UserPermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserPermi__Permi__245D67DE");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPermissions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserPermi__UserI__236943A5");
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.ToTable("Staff", "Clinic");

                entity.HasIndex(e => e.NationalCode, "UQ__Staff__3DFA41063A979FD8")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.Degree).HasMaxLength(150);

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.FullName).HasMaxLength(150);

                entity.Property(e => e.HireDate).HasColumnType("date");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Mobile).HasMaxLength(20);

                entity.Property(e => e.NationalCode).HasMaxLength(15);

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.staff)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Staff__BranchId__59FA5E80");

                entity.HasOne(d => d.Profession)
                    .WithMany(p => p.staff)
                    .HasForeignKey(d => d.ProfessionId)
                    .HasConstraintName("FK__Staff__Professio__5AEE82B9");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
