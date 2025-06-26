using Contracts.Enums;

namespace Payments.Contracts
{
    public class CustomerAddressForUpdateDto
    {
        public long? CustomerId { get; set; }

        public AddressType? AddressType { get; set; }

        public string StreetAddress1 { get; set; }

        public string StreetAddress2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public bool? IsDefault { get; set; }
    }
} 