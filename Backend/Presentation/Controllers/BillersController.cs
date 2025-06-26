using System;
using System.Threading;
using System.Threading.Tasks;
using Payments.Contracts;
using Microsoft.AspNetCore.Mvc;
using Payments.Services.Abstractions;

namespace Payments.Presentation.Controllers
{
    [ApiController]
    [Route("api/billers")]
    public class BillersController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public BillersController(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [HttpGet]
        public async Task<IActionResult> GetBillers(CancellationToken cancellationToken)
        {
            var billers = await _serviceManager.BillerService.GetAllAsync(cancellationToken);

            return Ok(billers);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetBillerById(long id, CancellationToken cancellationToken)
        {
            var billerDto = await _serviceManager.BillerService.GetByIdAsync(id, cancellationToken);

            return Ok(billerDto);
        }

        [HttpGet("public/{publicId:guid}")]
        public async Task<IActionResult> GetBillerByPublicId(Guid publicId, CancellationToken cancellationToken)
        {
            var billerDto = await _serviceManager.BillerService.GetByPublicIdAsync(publicId, cancellationToken);

            return Ok(billerDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBiller([FromBody] BillerForCreationDto billerForCreationDto, CancellationToken cancellationToken)
        {
            var billerDto = await _serviceManager.BillerService.CreateAsync(billerForCreationDto, cancellationToken);

            return CreatedAtAction(nameof(GetBillerById), new { id = billerDto.Id }, billerDto);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateBiller(long id, [FromBody] BillerForUpdateDto billerForUpdateDto, CancellationToken cancellationToken)
        {
            await _serviceManager.BillerService.UpdateAsync(id, billerForUpdateDto, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteBiller(long id, CancellationToken cancellationToken)
        {
            await _serviceManager.BillerService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
} 