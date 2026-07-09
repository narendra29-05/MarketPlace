namespace Findly.Domain.ValueObjects;

public class PhoneNumber
{
    public string Value { get; private set; } = null!;

    protected PhoneNumber() { }

    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty.");

        var digits = new string(value.Where(char.IsDigit).ToArray());

        if (digits.Length < 7 || digits.Length > 15)
            throw new ArgumentException("Phone number must be between 7 and 15 digits.");

        return new PhoneNumber { Value = value.Trim() };
    }
}
