using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
   
        public record VerifyOtpRequest(string CountryCode, string PhoneNumber, string Code);
    
}
