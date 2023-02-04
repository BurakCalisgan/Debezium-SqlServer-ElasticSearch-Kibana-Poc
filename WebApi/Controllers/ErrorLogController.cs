using Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorLogController : Controller
    {
        private readonly IErrorLogService _errorLogService;

        public ErrorLogController(IErrorLogService errorLogService)
        {
            _errorLogService = errorLogService;
        }

        [HttpPost("sample")]
        public async Task<IActionResult> PostSampleData()
        {
            await _errorLogService.InsertManyAsync();

            return Ok(new { Result = "Data successfully registered with Elasticsearch" });
        }

        [HttpPost("exception")]
        public IActionResult PostException()
        {
            throw new Exception("Generate sample exception");
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _errorLogService.GetAllAsync();

            return Json(result);
        }

        [HttpGet("errortitle-match")]
        public async Task<IActionResult> GetByErrorTitleWithMatch([FromQuery] string errorTitle)
        {
            var result = await _errorLogService.GetByErrorTitleWithMatch(errorTitle);

            return Json(result);
        }

        [HttpGet("errortitle-multimatch")]
        public async Task<IActionResult> GetByErrorTitleAndDescriptionMultiMatch([FromQuery] string term)
        {
            var result = await _errorLogService.GetByErrorTitleAndDescriptionMultiMatch(term);

            return Json(result);
        }

        [HttpGet("errortitle-matchphrase")]
        public async Task<IActionResult> GetByErrorTitleWithMatchPhrase([FromQuery] string errorTitle)
        {
            var result = await _errorLogService.GetByErrorTitleWithMatchPhrase(errorTitle);

            return Json(result);
        }

        [HttpGet("errortitle-matchphraseprefix")]
        public async Task<IActionResult> GetByErrorTitleWithMatchPhrasePrefix([FromQuery] string errorTitle)
        {
            var result = await _errorLogService.GetByErrorTitleWithMatchPhrasePrefix(errorTitle);

            return Json(result);
        }

        [HttpGet("errortitle-term")]
        public async Task<IActionResult> GetByErrorTitleWithTerm([FromQuery] string errorTitle)
        {
            var result = await _errorLogService.GetByErrorTitleWithTerm(errorTitle);

            return Json(result);
        }

        [HttpGet("errortitle-wildcard")]
        public async Task<IActionResult> GetByErrorTitleWithWildcard([FromQuery] string errorTitle)
        {
            var result = await _errorLogService.GetByErrorTitleWithWildcard(errorTitle);

            return Json(result);
        }

        [HttpGet("errortitle-fuzzy")]
        public async Task<IActionResult> GetByErrorTitleWithFuzzy([FromQuery] string errorTitle)
        {
            var result = await _errorLogService.GetByErrorTitleWithFuzzy(errorTitle);

            return Json(result);
        }

        [HttpGet("description-match")]
        public async Task<IActionResult> GetByDescriptionMatch([FromQuery] string description)
        {
            var result = await _errorLogService.GetByDescriptionMatch(description);

            return Json(result);
        }

        [HttpGet("all-fields")]
        public async Task<IActionResult> SearchAllProperties([FromQuery] string term)
        {
            var result = await _errorLogService.SearchInAllFiels(term);

            return Json(result);
        }

        [HttpGet("condiction")]
        public async Task<IActionResult> GetByCondictions([FromQuery] string errorTitle, [FromQuery] string description, [FromQuery] DateTime? errorDate)
        {
            var result = await _errorLogService.GetErrorLogsCondition(errorTitle, description, errorDate);

            return Json(result);
        }

        [HttpGet("term")]
        public async Task<IActionResult> GetByAllCondictions([FromQuery] string term)
        {
            var result = await _errorLogService.GetErrorLogsAllCondition(term);

            return Json(result);
        }

        [HttpGet("aggregation")]
        public async Task<IActionResult> GetActorsAggregation()
        {
            var result = await _errorLogService.GetErrorLogsAggregation();

            return Json(result);
        }
    }
}
