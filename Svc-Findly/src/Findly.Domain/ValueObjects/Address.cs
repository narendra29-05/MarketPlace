namespace Findly.Domain.ValueObjects;

public class Address
{
    protected Address() { }

    public string AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string PostalCode { get; private set; }

    public static Address Create(
        string addressLine1,
        string addressLine2,
        string city,
        string state,
        string country,
        string postalCode)
    {
        return new Address
        {
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            City = city,
            State = state,
            Country = country,
            PostalCode = postalCode
        };
    }
}
