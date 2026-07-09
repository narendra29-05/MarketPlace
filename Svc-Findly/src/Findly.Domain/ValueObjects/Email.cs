namespace Findly.Domain.ValueObjects;

public class Email
{
    public string Value { get; private set; } = null!;

    protected Email() { }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");

        if (!value.Contains('@') || !value.Contains('.'))
            throw new ArgumentException("Invalid email format.");

        return new Email { Value = value.Trim().ToLower() };
    }
}
