using System.Security.Claims;
using Identity.Core.Enums;
using Identity.Core.Interfaces;
using Identity.Core.Models.User;
using Identity.Core.Validators.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Identity.Core.Helpers;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var validator = new LoginValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });

            }
            try
            {
                var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
                var result = await _identityService.LoginAsync(request, ipAddress);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new {message = ex.Message });
            }
            
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public async Task<IActionResult> RegistrationAsync([FromBody] RegistrationRequest request)
        {
            var validator = new RegistrationValidator();
            var validationResult = await validator.ValidateAsync(request);
            if(!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });

            }
            try
            {
                var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
                var result = await _identityService.RegistrationAsync(request, ipAddress);
                return Ok(result);
            }

            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }           

        }
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var ipAddress = IpHelper.GetClientIpAddress(HttpContext);
                var response = await _identityService.RefreshTokenAsync(request.RefreshToken, ipAddress);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            try
            {
                await _identityService.RevokeTokenAsync(request.RefreshToken);
                return Ok(new { message = "Token-ը դարձավ անվավեր" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [Authorize]
        [HttpPost("revoke-all-tokens/{userId}")]
        public async Task<IActionResult> RevokeAllTokens(int userId)
        {
            try
            {
                await _identityService.RevokeAllUserTokensAsync(userId);
                return Ok(new { message = "Բոլոր token-ները դարձան անվավեր" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("nameid");
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Օգտատերը ճանաչված չէ։" });

            var status = await _identityService.DeleteAsync(id, userId);

            return status switch
            {
                DeleteStatus.NotFound => NotFound(new { message = $"{id} Id ունեցող օգտատեր չի գտնվել:" }),
                DeleteStatus.AlreadyDeleted => BadRequest(new { message = $"{id} Id ունեցող օգտատերը արդեն ջնջված է:" }),
                DeleteStatus.Deleted => Ok(new { message = $"{id} Id ունեցող օգտատերը հաջողությամբ ջնջվել է:" }),
                _ => StatusCode(500)
            };
        }

        [Authorize]
        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetListAsync()
        {
            var data = await _identityService.GetListAsync();
            if (data == null || !data.Any())
            {
                return Ok(new
                {
                    message = "Ոչ մի օգտատեր չի գտնվել:",
                    data = new List<UserDetails>()
                });
            }
            return Ok(data);
        }

    }
}
