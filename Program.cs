using RealTimeCommunication.Hubs;
using RealTimeCommunication.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddHostedService<SensorBroadcastService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var ws = await context.WebSockets.AcceptWebSocketAsync();
        var i = 0;
        while (ws.State == System.Net.WebSockets.WebSocketState.Open)
        {
            var latest = SensorBroadcastService.History.LastOrDefault();
            var msg = latest != null
                ? $"[{DateTime.Now:HH:mm:ss}] T:{latest.Temperature}°C H:{latest.Humidity}% P:{latest.Pressure}hPa"
                : $"[{DateTime.Now:HH:mm:ss}] Очікування даних...";
            var bytes = System.Text.Encoding.UTF8.GetBytes(msg);
            await ws.SendAsync(bytes, System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
            await Task.Delay(1000);
            i++;
        }
    }
    else context.Response.StatusCode = 400;
});

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapHub<SensorHub>("/sensorHub");
app.Run();