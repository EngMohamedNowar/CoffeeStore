using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Common.Models;

public sealed record AuthUserSnapshot(Guid Id, string Email, string? DisplayName);
