using Findly.Domain.Entities;

namespace Findly.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) Generate(User user);
}
