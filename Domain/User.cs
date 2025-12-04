using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public  string CountryCode { get; set; }
        public  string PhoneNumber { get; set; }
        public string Email { get; set; }
        public  string PasswordHash { get; set; }
        public bool IsPhoneVerified { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
