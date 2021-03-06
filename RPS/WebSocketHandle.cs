﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Net.WebSockets;
using RPS.GameLogic;

namespace RPS
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class WebSocketHandle
    {
        private readonly RequestDelegate _next;

        public WebSocketHandle(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var ws = await httpContext.WebSockets.AcceptWebSocketAsync();
                var player = new Player(ws);
                await player.HandleWebsocket();
            }
            else
            {
                await _next(httpContext);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class WebSocketHandleExtensions
    {
        public static IApplicationBuilder UseWebSocketHandle(this IApplicationBuilder builder)
        {
            builder.UseWebSockets();
            return builder.UseMiddleware<WebSocketHandle>();
        }
    }
}
