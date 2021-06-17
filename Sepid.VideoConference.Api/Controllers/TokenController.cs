using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepid.VideoConference.Api.Core.Dto;
using Sepid.VideoConference.Api.Core.Services;

namespace Sepid.VideoConference.Api.Controllers
{
    [Route("[Controller]")]
    [EnableCors]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly ITokenGenerator _tokenGenerator;

        public TokenController(ITokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(RegisterUserDto registerUserDto,
            CancellationToken cancellationToken)
        {
            return Ok(await _tokenGenerator.Generate(registerUserDto.Name, cancellationToken));
        }
    }
}