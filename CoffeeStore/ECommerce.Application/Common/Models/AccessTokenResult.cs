using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Common.Models;

public record AccessTokenResult(string Token, DateTimeOffset ExpireAtUtc);
