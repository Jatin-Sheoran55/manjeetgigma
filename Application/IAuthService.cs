using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
//using Microsoft.AspNetCore.Identity.Data;


namespace Application
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequestDto request );
        Task SendOtpAsync(OtpRequest request );
        Task verifyOtpAsync(VerifyOtpRequest request );
        Task<string> LoginAsync(LoginRequestDto request);
        Task ResetPasswordAsync(VerifyOtpRequest verifyRequest, string newPassword );
    }
}
