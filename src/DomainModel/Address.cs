using CSharpFunctionalExtensions;

namespace DomainModel;

public class Address : Entity
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }

    public Address()
    {
        
    }

    private Address(string street, string city, string state, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
    }

    public static Result<Address, Error> Create(string street, string city, string state, string zipCode)
    {
        street = (street ?? "").Trim();
        city = (city ?? "").Trim();
        state = (state ?? "").Trim();
        zipCode = (zipCode ?? "").Trim();

        if (street.Length < 1 || street.Length > 100)
            return Errors.General.InvalidLength("street");

        if (city.Length < 1 || city.Length > 40)
            return Errors.General.InvalidLength("city");

        if (zipCode.Length < 1 || zipCode.Length > 5)
            return Errors.General.InvalidLength("zip code");

        return new Address(street, city, state, zipCode);
    }
}