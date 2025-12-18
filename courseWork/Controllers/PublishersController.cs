using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courseWork.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var publishers = await _publisherService.GetAllPublishersAsync();
            return Ok(publishers);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherRequest request)
        {
            var createdPublisher = await _publisherService.CreatePublisherAsync(request);
            return Ok(createdPublisher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublisher(int id, [FromBody] CreatePublisherRequest request)
        {
            try
            {
                var updatedPublisher = await _publisherService.UpdatePublisherAsync(id, request);
                return Ok(updatedPublisher);
            }
            catch (Exception ex) when (ex.Message.Contains("doesn't exist"))
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            try
            {
                await _publisherService.DeletePublisherAsync(id);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message.Contains("doesn't exist"))
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}