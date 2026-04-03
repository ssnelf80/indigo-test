using IndigoTestTask.Adapters.Sources.Servers.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace IndigoTestTask.Host.Controllers;

public class SourceWebSocketController(
    AliceSourceServerHandler aliceSourceServerHandler,
    BobSourceServerHandler bobSourceServerHandler,
    ChloeSourceServerHandler chloeSourceServerHandler
    ) : ControllerBase
{
    [Route("/ws-alice")]
    public async Task GetAliceWs(CancellationToken cancellationToken)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await aliceSourceServerHandler.Handle(webSocket, cancellationToken);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    
    [Route("/ws-bob")]
    public async Task GetBobWs(CancellationToken cancellationToken)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await bobSourceServerHandler.Handle(webSocket, cancellationToken);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    
    [Route("/ws-chloe")]
    public async Task GetChloeWs(CancellationToken cancellationToken)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await chloeSourceServerHandler.Handle(webSocket, cancellationToken);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}