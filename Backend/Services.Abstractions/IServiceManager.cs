namespace Payments.Services.Abstractions
{
    public interface IServiceManager
    {
        IBillerService BillerService { get; }
        ICustomerAddressService CustomerAddressService { get; }
    }
}
