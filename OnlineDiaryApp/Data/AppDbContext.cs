using Microsoft.EntityFrameworkCore;
using OnlineDiaryApp.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace OnlineDiaryApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<NoteFile> NoteFiles { get; set; }
        public DbSet<Notebook> Notebooks { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Note>()
                .HasMany(n => n.Tags)
                .WithMany(t => t.Notes);

            modelBuilder.Entity<NoteFile>()
                .HasOne(f => f.Note)
                .WithMany(n => n.Files)
                .HasForeignKey(f => f.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Note>()
                .HasOne(n => n.Notebook)
                .WithMany(nb => nb.Notes)
                .HasForeignKey(n => n.NotebookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tag>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tags)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notebook>()
                .HasOne(nb => nb.User)
                .WithMany(u => u.Notebooks) 
                .HasForeignKey(nb => nb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Note>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Note>()
                .Property(n => n.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            base.OnModelCreating(modelBuilder);
        }

    }
}