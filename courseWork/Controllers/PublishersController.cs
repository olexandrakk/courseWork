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
    }
}