using System.Collections.Generic;
using System.Threading.Tasks;
using Sepid.VideoConference.Api.Core.Dto;

namespace Sepid.VideoConference.Api.Core.Services
{
    public interface IChatRoomServices
    {

        Task<List<OnlineUserDto>> AddOnlineUsersAsync(string userId, string fullName, string connectionId);

        Task<List<OnlineUserDto>> RemoveUserAsync(string userId,string connectionId);

        Task<OnlineUserDto> GetUserAsync(string userId);

        Task AddRoomAsync(ChatRoomDto chatRoom);

        Task<ChatRoomDto> GetChatRoomAsync(string ownerName);

        Task<ChatRoomDto> AcceptVideoCallAsync(string name,bool accept);

        Task DeleteChatRoom(string name);


    }
}