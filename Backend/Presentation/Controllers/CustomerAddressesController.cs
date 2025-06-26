using System;
using System.Threading;
using System.Threading.Tasks;
using Payments.Contracts;
using Microsoft.AspNetCore.Mvc;
using Payments.Services.Abstractions;

namespace Payments.Presentation.Controllers
{
    [ApiController]
    [Route("api/customer-addresses")]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CustomerAddressesController(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [HttpGet]
        public async Task<IActionResult> GetCustomerAddresses(CancellationToken cancellationToken)
        {
            var addresses = await _serviceManager.CustomerAddressService.GetAllAsync(cancellationToken);

            return Ok(addresses);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetCustomerAddressById(long id, CancellationToken cancellationToken)
        {
            var addressDto = await _serviceManager.CustomerAddressService.GetByIdAsync(id, cancellationToken);

            return Ok(addressDto);
        }

        [HttpGet("public/{publicId:guid}")]
        public async Task<IActionResult> GetCustomerAddressByPublicId(Guid publicId, CancellationToken cancellationToken)
        {
            var addressDto = await _serviceManager.CustomerAddressService.GetByPublicIdAsync(publicId, cancellationToken);

            return Ok(addressDto);
        }

        [HttpGet("customers/{customerId:long}")]
        public async Task<IActionResult> GetCustomerAddressesByCustomerId(long customerId, CancellationToken cancellationToken)
        {
            var addresses = await _serviceManager.CustomerAddressService.GetByCustomerIdAsync(customerId, cancellationToken);

            return Ok(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerAddress([FromBody] CustomerAddressForCreationDto customerAddressForCreationDto, CancellationToken cancellationToken)
        {
            var addressDto = await _serviceManager.CustomerAddressService.CreateAsync(customerAddressForCreationDto, cancellationToken);

            return CreatedAtAction(nameof(GetCustomerAddressById), new { id = addressDto.Id }, addressDto);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateCustomerAddress(long id, [FromBody] CustomerAddressForUpdateDto customerAddressForUpdateDto, CancellationToken cancellationToken)
        {
            await _serviceManager.CustomerAddressService.UpdateAsync(id, customerAddressForUpdateDto, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteCustomerAddress(long id, CancellationToken cancellationToken)
        {
            await _serviceManager.CustomerAddressService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
} 