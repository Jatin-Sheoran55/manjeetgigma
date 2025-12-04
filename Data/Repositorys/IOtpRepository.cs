using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Data.Repositorys
{
    public interface IOtpRepository
    {
        Task AddAsync(Otp otp);
        Task<Otp?> GetValidOtpAsync(Guid userId, string code);
        Task MakeUsedAsync(Otp otp);
    }
}
