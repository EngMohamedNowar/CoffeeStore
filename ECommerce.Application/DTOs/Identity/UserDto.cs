using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Identity
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string DisplayName { get; set; }
    }
}
