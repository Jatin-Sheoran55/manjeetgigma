using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositorys
{
    public class UserRepository:IUserRepository
    {
        private readonly ProjectContext _db;
        public UserRepository(ProjectContext db) => _db = db;

        public async Task<User?> GetByPhoneAsync(string countryCode, string phone) =>
            await _db.Users.FirstOrDefaultAsync(u => u.CountryCode == countryCode && u.PhoneNumber == phone);

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _db.Users.FindAsync(id);

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}
