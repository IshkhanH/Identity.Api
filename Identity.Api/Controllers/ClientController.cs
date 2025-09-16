using Identity.Core.Interfaces;
using Identity.Core.Models.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] ClientCreate request
            )
        {
            var res = await _clientService.CreateAsync(request);
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] ClientUpdate request
            )
        {
            await _clientService.UpdateAsync(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id
            )
        {
            await _clientService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(
            [FromRoute] int id
            )
        {
            var data = await _clientService.GetAsync(id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync()
        {
            var data = await _clientService.GetListAsync();
            return Ok(data);
        }
    }
}
