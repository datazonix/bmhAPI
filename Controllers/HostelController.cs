using Microsoft.AspNetCore.Mvc;
using bmhAPI.Models;
using bmhAPI.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace bmhAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostelController : ControllerBase
    {
        private readonly DBExecutor _dbExecutor;
        private readonly DBNonQueryExecutor _dbNonQueryExecutor;
        public HostelController(DBExecutor dbExecutor, DBNonQueryExecutor dbNonQueryExecutor)
        {
            _dbExecutor = dbExecutor;
            _dbNonQueryExecutor = dbNonQueryExecutor;
        }


        [HttpPost("hostel/list")]
        public IActionResult GetHostels([FromBody] HostelListRequest request)
        {
            try
            {
                // Validate input
                if (request == null || string.IsNullOrWhiteSpace(request.HostelCode))
                {
                    return BadRequest(new { message = "HostelCode is required" });
                }

                // Prepare parameters
                string[] paramNames = { "@hostel_code" };
                object?[] paramValues = { request.HostelCode };

                // Execute SP directly from controller
                var data = _dbExecutor.ExecuteReader("sp_hostel_list", paramNames, paramValues);

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

        

        [HttpPost("create")]

        public IActionResult CreateHostel([FromBody] HostelCreateRequest request)
        {
            try
            {
                var inputParams = new Dictionary<string, object?>
        {
            { "@name", request.Name },
            { "@id", request.Id }
        };

                var outputParams = new string[] { "@val" };

                var (tableResult, outputValues) = _dbNonQueryExecutor.ExecuteProcedure("sp_hostel_create", inputParams, outputParams);

                object? result = tableResult ?? (outputValues != null && outputValues.ContainsKey("@val") ? outputValues["@val"] : null);

                return Ok(new
                {
                    result
                });
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
