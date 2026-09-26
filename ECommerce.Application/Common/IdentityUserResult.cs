using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Common
{
    public class IdentityUserResult
    {
        public IdentityUserResult(string id, string displayName, string email, string userName)
        {
            Id = id;
            DisplayName = displayName;
            Email = email;
            UserName = userName;
        }

        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }

    }
}
