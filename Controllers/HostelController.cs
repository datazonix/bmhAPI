using Microsoft.AspNetCore.Mvc;
using bmhAPI.Models;
using bmhAPI.Services;

namespace bmhAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostelController : ControllerBase
    {
        private readonly HostelService _hostelService;

        public HostelController(HostelService hostelService)
        {
            _hostelService = hostelService;
        }

        [HttpPost("list")]
        public IActionResult GetHostels([FromBody] HostelListRequest request)
        {
            try
            {
                var data = _hostelService.GetHostelList(request.HostelCode, request.CityName);
                return Ok(data);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = "Database error occurred", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message });
            }
        }
    }
}
