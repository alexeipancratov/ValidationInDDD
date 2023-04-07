using CSharpFunctionalExtensions;
using DomainModel.ValueObjects;

namespace DomainModel;

public class Address : Entity
{
    public string Street { get; set; }
    public string City { get; set; }
    public State State { get; set; }
    public string ZipCode { get; set; }

    public Address()
    {
        
    }

    private Address(string street, string city, State state, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
    }

    public static Result<Address, Error> Create(string street, string city, string state, string zipCode,
        string[] allStates)
    {
        // NOTE: We assume that the state has been already validated.
        // We don't want to validate it here, because we don't want to report on errors of this object.
        // State object should take care of this.
        var stateObject = State.Create(state, allStates).Value;
        
        street = (street ?? "").Trim();
        city = (city ?? "").Trim();
        zipCode = (zipCode ?? "").Trim();

        if (street.Length < 1 || street.Length > 100)
            // indicate specific field because we're not using Value Objects for all Address props.
            // so Fluent Validation cannot figure out the proper path on its own.
            // It works for State though.ß
            return Errors.General.InvalidLength("street");

        if (city.Length < 1 || city.Length > 40)
            return Errors.General.InvalidLength("city");

        if (zipCode.Length < 1 || zipCode.Length > 5)
            return Errors.General.InvalidLength("zip code");

        return new Address(street, city, stateObject, zipCode);
    }
}