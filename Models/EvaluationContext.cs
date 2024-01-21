using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Evaluation.Models;

public partial class EvaluationContext : DbContext
{
    public EvaluationContext()
    {
    }

    public EvaluationContext(DbContextOptions<EvaluationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<FormDt> FormDts { get; set; }

    public virtual DbSet<FormMst> FormMsts { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\;Database=Evaluation;TrustServerCertificate=True;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmpId);

            entity.ToTable("Employee");

            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.EmpDep)
                .HasMaxLength(250)
                .HasColumnName("emp_dep");
            entity.Property(e => e.EmpName)
                .HasMaxLength(250)
                .HasColumnName("emp_name");
            entity.Property(e => e.EmpStep)
                .HasMaxLength(100)
                .HasColumnName("emp_step");
            entity.Property(e => e.EmpTitle)
                .HasMaxLength(100)
                .HasColumnName("emp_title");
        });

        modelBuilder.Entity<FormDt>(entity =>
        {
            entity.HasKey(e => e.FormDtsId);

            entity.ToTable("Form_dts");

            entity.Property(e => e.FormDtsId).HasColumnName("form_dts_id");
            entity.Property(e => e.FormId).HasColumnName("form_id");
            entity.Property(e => e.SkillDegree)
                .HasMaxLength(100)
                .HasColumnName("skill_degree");
            entity.Property(e => e.SkillId).HasColumnName("skill_id");
        });

        modelBuilder.Entity<FormMst>(entity =>
        {
            entity.HasKey(e => e.FormId);

            entity.ToTable("Form_mst");

            entity.Property(e => e.FormId).HasColumnName("form_id");
            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.FormDate)
                .HasColumnType("date")
                .HasColumnName("form_date");
            entity.Property(e => e.KnowledgeType)
                .HasMaxLength(250)
                .HasColumnName("knowledge_type");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.ToTable("Skill");

            entity.Property(e => e.SkillId).HasColumnName("skill_id");
            entity.Property(e => e.SkillTitle)
                .HasMaxLength(100)
                .HasColumnName("skill_title");
            entity.Property(e => e.SkillType)
                .HasMaxLength(100)
                .HasColumnName("skill_type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
