using Identity.Core.Interfaces;
using Identity.Core.Models.Client;
using Identity.Core.Models.User;
using Identity.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(
        [FromBody] UserCreate request
            )
        {
            var res = await _userService.CreateAsync(request);
            return Ok(res);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] UserUpdate request)
        {
            await _userService.UpdateAsync(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id)
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetAsync(
            [FromRoute] int id)
        {
            var data = await _userService.GetAsync(id);
            return Ok(data);
        }

        [HttpGet]

        public async Task<IActionResult> GetListAsync()
        {
            var data = await _userService.GetListAsync();
            return Ok(data);
        }

    }
}
