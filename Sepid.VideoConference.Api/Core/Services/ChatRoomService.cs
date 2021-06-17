using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sepid.VideoConference.Api.Core.Dto;

namespace Sepid.VideoConference.Api.Core.Services
{
    public class ChatRoomService : IChatRoomServices
    {
        private static readonly Dictionary<Guid, OnlineUserDto> OnlineUser = new();
        private static readonly Dictionary<Guid, ChatRoomDto> ChatRoom = new();


        public Task<List<OnlineUserDto>> AddOnlineUsersAsync(string userId, string fullName, string connectionId)
        {
            var user = OnlineUser.FirstOrDefault(x => x.Value.UserId == userId);

            if (user.Key == Guid.Empty)
            {
                OnlineUser.TryAdd(Guid.NewGuid(), new OnlineUserDto
                {
                    UserId = userId,
                    FullName = fullName,
                    ConnectionIds = new List<string> { connectionId }
                });
            }


            return Task.FromResult(OnlineUser.Values.ToList());
        }

        public Task<List<OnlineUserDto>> RemoveUserAsync(string userId, string connectionId)
        {
            var user = OnlineUser.FirstOrDefault(x => x.Value.UserId == userId);

            if (user.Key != Guid.Empty)
            {
                OnlineUser.Remove(user.Key);
            }

            return Task.FromResult(OnlineUser.Values.ToList());
        }




        public Task<OnlineUserDto> GetUserAsync(string userId)
        {
            var onlineUser = OnlineUser.FirstOrDefault(x => x.Value.UserId == userId);

            if (onlineUser.Key == Guid.Empty)
            {
                throw new ArgumentException("user not found");
            }

            return Task.FromResult(onlineUser.Value);
        }

        public Task AddRoomAsync(ChatRoomDto chatRoom)
        {
            var id = Guid.NewGuid();

            ChatRoom.TryAdd(id, chatRoom);

            return Task.CompletedTask;
        }

        public Task<ChatRoomDto> GetChatRoomAsync(string name)
        {
            var chatroom = ChatRoom.FirstOrDefault(x => x.Value.OwnerName == name || x.Value.GuestName == name);

            if (chatroom.Key == Guid.Empty)
            {
                throw new ArgumentException("chatroom not found");
            }

            return Task.FromResult(chatroom.Value);
        }

        public Task<ChatRoomDto> AcceptVideoCallAsync(string name, bool accept)
        {
            var chatroom = ChatRoom.FirstOrDefault(x => x.Value.GuestName == name);

            if (chatroom.Key == Guid.Empty)
            {
                throw new ArgumentException("chatroom not found");
            }

            if (accept == false)
            {
                ChatRoom.Remove(chatroom.Key);
            }

            return Task.FromResult(chatroom.Value);
        }

        public Task DeleteChatRoom(string name)
        {
            var chatroom =
                ChatRoom.FirstOrDefault(x => x.Value.GuestName == name || x.Value.OwnerName == name);

            if (chatroom.Key != Guid.Empty)
            {
                ChatRoom.Remove(chatroom.Key);
            }


            return Task.CompletedTask;
        }
    }
}