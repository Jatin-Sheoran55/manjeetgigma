using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositorys
{
    public class OtpRepository:IOtpRepository
    {
        private readonly ProjectContext _db;
        public OtpRepository(ProjectContext db) => _db = db;

        public async Task AddAsync(Otp otp)
        {
            _db.Otps.Add(otp);
            await _db.SaveChangesAsync();
        }

        public async Task<Otp?> GetValidOtpAsync(Guid userId, string code)
        {
            return await _db.Otps
                .Where(o => o.UserId == userId && o.Code == code && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(o => o.ExpiresAt)
                .FirstOrDefaultAsync();
        }

        public async Task MakeUsedAsync(Otp otp)
        {
            otp.IsUsed = true;
            _db.Otps.Update(otp);
            await _db.SaveChangesAsync();
        }
    }
}
