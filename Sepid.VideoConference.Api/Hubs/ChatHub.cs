using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using Sepid.VideoConference.Api.Core.Dto;
using Sepid.VideoConference.Api.Core.Services;

namespace Sepid.VideoConference.Api.Hubs
{
    [EnableCors]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private readonly IChatRoomServices _chatRoomServices;

        public ChatHub(IChatRoomServices chatRoomServices)
        {
            _chatRoomServices = chatRoomServices;
        }

        public override async Task OnConnectedAsync()
        {

            try
            {
                var onlineUsers = await _chatRoomServices.AddOnlineUsersAsync(
                    Context.User.FindFirstValue("Id"),
                    Context.User.FindFirstValue("fullName"),
                    Context.ConnectionId);
                
                
                await Clients.All.SendAsync("OnlineUsers", onlineUsers);
            }
            catch (Exception)
            {
                //
            }
          


            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var onlineUsers =
                    await _chatRoomServices.RemoveUserAsync(Context.User.FindFirstValue("Id"), Context.ConnectionId);

                await Clients.All.SendAsync("OnlineUsers", onlineUsers);
            }
            catch (Exception)
            {
                //
            }
    

            await base.OnDisconnectedAsync(exception);
        }


        public async Task RequestVideoCall(string userId)
        {
            var onlineUser = await _chatRoomServices.GetUserAsync(userId);

            if (onlineUser is null)
            {
                throw new ArgumentException("user not found");
            }

            var groupName = Guid.NewGuid().ToString();

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await _chatRoomServices.AddRoomAsync(new ChatRoomDto
            {
                GroupName = groupName,
                OwnerConnectionId = Context.ConnectionId,
                GuestName = onlineUser.FullName,
                OwnerName = Context.User?.Identity?.Name,
                GuestConnectionId = onlineUser.ConnectionIds.FirstOrDefault()
            });

            await Clients.Client(onlineUser.ConnectionIds.FirstOrDefault()!).SendAsync("VideoCall");
        }


        public async Task ResponseToVideoCall(bool accept)
        {
            var chatRoom = await _chatRoomServices.AcceptVideoCallAsync(Context.User?.Identity?.Name, accept);

            if (accept == false)
            {
                await Clients.Client(chatRoom.OwnerConnectionId).SendAsync("Response", new {response = false});
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoom.GroupName);

            await Clients.Client(chatRoom.OwnerConnectionId).SendAsync("Response", new {response = true});
        }


        public async Task UploadStream(ChannelReader<string> stream)
        {
            var chatroom = await _chatRoomServices.GetChatRoomAsync(Context.User?.Identity?.Name);

            while (await stream.WaitToReadAsync())
            {
                while (stream.TryRead(out var item))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var dataStream = item.Split('|');
                        if (!string.IsNullOrEmpty(dataStream[0]))
                        {
                            await Clients.Client(chatroom.GuestConnectionId).SendAsync("StreamVideo", dataStream[1]);
                        }
                    }
                }
            }
        }

        public ChannelReader<string> DownloadStream(int delay, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<string>();
            _ = WriteItemsAsync(channel.Writer, DateTime.Now.Millisecond.ToString(), delay, cancellationToken);

            return channel.Reader;
        }


        private static async Task WriteItemsAsync(ChannelWriter<string> writer, string data, int delay,
            CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                await writer.WriteAsync(data, cancellationToken);
                await Task.Delay(delay, cancellationToken);
            }
            catch (Exception ex)
            {
                localException = ex;
            }

            writer.Complete(localException);
        }

        public async Task SendData(string data)
        {
            var chatroom = await _chatRoomServices.GetChatRoomAsync(Context.User?.Identity?.Name);


            if (chatroom.GuestName == Context.User?.Identity?.Name)
            {
                await Clients.Client(chatroom.OwnerConnectionId).SendAsync("ReceiveData", data);
            }
            else
            {
                await Clients.Client(chatroom.GuestConnectionId).SendAsync("ReceiveData", data);
            }
        }


        public async Task EndCall() => await _chatRoomServices.DeleteChatRoom(Context.User?.Identity?.Name);

    }
}