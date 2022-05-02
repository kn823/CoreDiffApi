using Microsoft.AspNetCore.Mvc;
using CoreDiffApi.Services;
using CoreDiffApi.Models;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreDiffApi.Controllers
{
    [ApiController]
    public class DiffController : ControllerBase
    {
        private readonly IDiffOp _diffOp;

        /// <summary>
        /// Inject DiffOp service at the start of this controller.
        /// DiffOp saves away left/right Base64 encode or provides diff-ing between left & right encode.
        /// </summary>
        /// <param name="diffOp"></param>
        public DiffController(IDiffOp diffOp)
        {
            _diffOp = diffOp;
        }

        /// <summary>
        /// In response to a GET request, controller calls DiffOp service to do the diff-ing.
        /// </summary>
        /// <param name="id">Input Id for the two Base64 encodes to be diff-ed.</param>
        /// <returns>A JSON formatted string as a response.</returns>
        [HttpGet("v1/diff/{id}")]
        public async Task<string> Get(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode =
                    Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound;
            }
            string result = await _diffOp.Diff(id);

            this.Response.StatusCode =
                Microsoft.AspNetCore.Http.StatusCodes.Status200OK;
            if (String.IsNullOrEmpty(result))
            {
                Response.StatusCode =
                    Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound;
            }

            return result;
        }

        /// <summary>
        /// In response to a POST request, controller calls DiffOp service to save away the left Base64 encode.
        /// </summary>
        /// <param name="id">Input Id for the Base64 encode to be saved away.</param>
        /// <param name="diffEncode64Data">Input Base64 encode to be saved away</param>
        /// <returns></returns>
        [HttpPost("v1/diff/{id}/left")]
        public async Task<IActionResult> PostLeft(string id, [FromBody] DiffEncode64Data diffEncode64Data)
        {
            string input = diffEncode64Data.Data;
            if (false == _diffOp.IsDataValid(id, input))
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }

            DiffInput diffInput = new DiffInput() { Id = id, DataLeft = input, DataRight = "" };
            try
            {
                await _diffOp.Save(diffInput);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// In response to a POST request, controller calls DiffOp service to save away the right Base64 encode.
        /// </summary>
        /// <param name="id">Input Id for the Base64 encode to be saved away.</param>
        /// <param name="diffEncode64Data"></param>
        /// <returns>Input Base64 encode to be saved away</returns>
        [HttpPost("v1/diff/{id}/right")]
        public async Task<IActionResult> PostRight(string id, [FromBody] DiffEncode64Data diffEncode64Data)
        {
            string input = diffEncode64Data.Data;
            if (false == _diffOp.IsDataValid(id, input))
            { 
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }

            DiffInput diffInput = new DiffInput() { Id = id, DataLeft = "", DataRight = input };
            try
            {
                await _diffOp.Save(diffInput);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
            }
        }
    }
}
