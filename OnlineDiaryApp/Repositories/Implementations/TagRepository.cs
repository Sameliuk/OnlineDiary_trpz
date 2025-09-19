using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineDiaryApp.Data;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;
using System;

namespace OnlineDiaryApp.Repositories.Implementation
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task AddAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
        }

        public async Task DeleteAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
                _context.Tags.Remove(tag);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

