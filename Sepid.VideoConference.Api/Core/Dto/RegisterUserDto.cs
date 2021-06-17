using System.ComponentModel.DataAnnotations;

namespace Sepid.VideoConference.Api.Core.Dto
{
    public class RegisterUserDto
    {
        [Required]
        public string Name { get; set; }
    }
}