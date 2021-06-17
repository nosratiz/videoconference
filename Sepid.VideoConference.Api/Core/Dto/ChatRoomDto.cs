using System;
using System.Collections.Generic;

namespace Sepid.VideoConference.Api.Core.Dto
{
    public class ChatRoomDto
    {
        public string OwnerConnectionId { get; set; }

        public string OwnerName { get; set; }
        public string GuestConnectionId { get; set; }

        public string GuestName { get; set; }
        public string GroupName { get; set; }

        public bool IsEnded { get; set; }
    }

    public class ChatRoomHandler
    {
        private static ChatRoomHandler _instance;

        private readonly object _locker = new();

        public static ChatRoomHandler Instance => _instance ??= new ChatRoomHandler();

        public IDictionary<Guid, ChatRoomDto> chatrooms;


        public ChatRoomHandler()
        {
            chatrooms = new Dictionary<Guid, ChatRoomDto>();
        }


        public void AddChatRoom(ChatRoomDto chatRoomDto)
        {
            try
            {
                chatrooms.Add(Guid.NewGuid(), chatRoomDto);
            }
            catch (Exception)
            {
                //
            }
        }


      
    }
}