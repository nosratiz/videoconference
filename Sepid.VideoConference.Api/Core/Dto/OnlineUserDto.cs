using System.Collections.Generic;

namespace Sepid.VideoConference.Api.Core.Dto
{
    public class OnlineUserDto
    {
        public List<string> ConnectionIds { get; set; }
        public string FullName { get; set; }

        public string UserId { get; set; }
    }


}