using System;
using Payments.Domain.Repositories;
using Payments.Services.Abstractions;

namespace Payments.Services
{
    public sealed class ServiceManager : IServiceManager
    {
        public ServiceManager(IBillerService billerService, ICustomerAddressService customerAddressService)
        {
            BillerService = billerService;
            CustomerAddressService = customerAddressService;
        }

        public IBillerService BillerService { get; }
        public ICustomerAddressService CustomerAddressService { get; }
    }
}
