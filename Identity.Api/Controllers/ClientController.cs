using Identity.Core.Interfaces;
using Identity.Core.Models.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Identity.Core.Enums;
using Identity.Core.Validators.Client;
using System.Security.Claims;


namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ClientCreate request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Օգտատերը ճանաչված չէ:" });

            request.UserId = int.Parse(userIdClaim);

            var validator = new ClientCreateValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
            try
            {
                var id = await _clientService.CreateAsync(request);
                return Ok(new { ClientId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Errors = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] ClientUpdate request
            )
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Օգտատերը ճանաչված չէ:" });

            request.UserId = int.Parse(userIdClaim);
            try
            {
                var status = await _clientService.UpdateAsync(id, request, User);

                return status switch
                {
                    UpdateStatus.NotFound => NotFound(new { message = $"{id} Id ունեցող հաճախորդ չի գտնվել:" }),
                    UpdateStatus.Deleted => BadRequest(new { message = $"{id} Id ունեցող հաճախորդը ջնջված է և չի կարող փոփոխվել:" }),
                    UpdateStatus.Updated => Ok(new { message = $"{id} Id ունեցող հաճախորդը հաջողությամբ թարմացվել է:" }),
                    _ => StatusCode(500)
                };
            }
            catch(InvalidDataException ex)
            {
                return BadRequest(new { Errors = ex.Message });
            }       

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue("nameid");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Օգտատերը ճանաչված չէ։" });

            
            var status = await _clientService.DeleteAsync(id, userId);

            return status switch
            {
                DeleteStatus.NotFound => NotFound(new { message = $"{id} Id ունեցող հաճախորդ չի գտնվել:" }),
                DeleteStatus.AlreadyDeleted => BadRequest(new { message = $"{id} Id ունեցող հաճախորդը արդեն ջնջված է:" }),
                DeleteStatus.Deleted => Ok(new { message = $"{id} Id ունեցող հաճախորդը հաջողությամբ ջնջվել է:" }),
                _ => StatusCode(500)
            };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] int id)
        {
            var data = await _clientService.GetAsync(id);

            if(data == null)
            {
                return NotFound(new {massage = $"{id} Id ունեցող հաճախորդ չի գտնվել:"});
            }
            if(data.IsDeleted)
            {
                return BadRequest(new { message = $"{id} ունեցող հաճախորդի տվյալները ջնջված են:" });
            }
            
            return Ok(data);
        }

        [HttpGet("AllClients")]
        public async Task<IActionResult> GetListAsync()
        {           
            
            var data = await _clientService.GetListAsync();
                if (data == null || !data.Any())
                {
                    return Ok(new { message = "Ոչ մի հաճախորդ չի գտնվել:", 
                                    data = new List<ClientDetails>() });
                }
            return Ok(data);                
                                  
        }

        [HttpGet("GetClientsByUser")]
        public async Task<IActionResult> GetByUserIdAsync([FromQuery] int userId)
        {
            try
            {
                var data = await _clientService.GetByUserIdAsync(userId);

             if (data == null || !data.Any())
                return Ok(new { message = "Ոչ մի հաճախորդ չի գտնվել", data = new List<ClientDetails>() });

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Նման օգտատեր գոյություն չունի:" });
            }            
        }
        
        [HttpGet("export")]
        public async Task<IActionResult> ExportClients()
        {
            var fileBytes = await _clientService.ExportExcelClientsAsync();
            var fileName = $"Clients_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
