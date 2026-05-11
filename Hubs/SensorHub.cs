using Microsoft.AspNetCore.SignalR;

namespace RealTimeCommunication.Hubs
{
    public class SensorHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}