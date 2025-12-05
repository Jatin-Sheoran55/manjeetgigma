using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Data.Repositorys;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IOtpRepository _otps;
        private readonly PasswordHasher<User> _hasher;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository users, IOtpRepository otps, IConfiguration config)
        {
            _users = users;
            _otps = otps;
            _hasher = new PasswordHasher<User>();
            _config = config;
        }


        public async Task RegisterAsync(RegisterRequestDto request)
        {
            var existing = await _users.GetByPhoneAsync(request.CountryCode, request.PhoneNumber);
            if (existing != null) throw new Exception("User already exists");

            var user = new User
            {
                CountryCode = request.CountryCode,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            };

            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            await _users.AddAsync(user);

            await SendOtpToUserAsync(user);
        }

        public async Task SendOtpAsync(OtpRequest request)
        {
            var user = await _users.GetByPhoneAsync(request.CountryCode, request.PhoneNumber)
                ?? throw new Exception("user not found");

            await SendOtpToUserAsync(user);
        }

        private async Task SendOtpToUserAsync(User user)
        {
            var code = GenerateNumericCode(4);
            var otp = new Otp
            {
                UserId = user.Id,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            };
            await _otps.AddAsync(otp);
          

            Console.WriteLine($"[OTP] to {user.CountryCode}{user.PhoneNumber}: {code}");
            await Task.CompletedTask;
            //Console.WriteLine($"[OTP] to {user.CountryCode}{user.PhoneNumber}:{code} ");
        }
       
        public async Task verifyOtpAsync(VerifyOtpRequest request)
        
        {
            var user = await _users.GetByPhoneAsync(request.CountryCode, request.PhoneNumber)
                ?? throw new Exception("User not found");

            var otp = await _otps.GetValidOtpAsync(user.Id, request.Code)
                ?? throw new Exception("Invalid/expired otp");

            await _otps.MakeUsedAsync(otp);
            user.IsPhoneVerified = true;
            await _users.UpdateAsync(user);
        }
        public async Task<string> LoginAsync(LoginRequestDto request)
        {
            var user = await _users.GetByPhoneAsync(request.CountryCode, request.PhoneNumber)
                ?? throw new Exception("User Not found");

            var res = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (res == PasswordVerificationResult.Failed) throw new Exception("Invalid Cdetails");
            if (!user.IsPhoneVerified) throw new Exception("Phone not verified");

            return GenerateJwtToken(user);
        }


        public async Task ResetPasswordAsync(VerifyOtpRequest verifyRequest, string newPassword)
        {
            var user = await _users.GetByPhoneAsync(verifyRequest.CountryCode, verifyRequest.PhoneNumber)
                ?? throw new Exception("User not found");

            var otp = await _otps.GetValidOtpAsync(user.Id, verifyRequest.Code)
                ?? throw new Exception("Invalid.expire otp");

            await _otps.MakeUsedAsync(otp);
            user.PasswordHash = _hasher.HashPassword(user, newPassword);
            await _users.UpdateAsync(user);
        }

        private string GenerateNumericCode(int len)
        {
            var rng = new Random();
            var s = "";
            for (int i = 0; i < len; i++) s += rng.Next(0, 10).ToString();
            return s;
        }
        private string GenerateJwtToken(User user)
        {
            var secret = _config["Jwt:key"] ?? throw new Exception("JWT secret missing");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("phone", $"{user.CountryCode}{user.PhoneNumber}"),
            new Claim("email", user.Email ?? "")
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);



        }
    }
}
