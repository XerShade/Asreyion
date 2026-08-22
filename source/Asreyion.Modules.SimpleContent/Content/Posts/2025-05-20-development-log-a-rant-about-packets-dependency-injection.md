---
title: "Rant: The Struggles of Networking Code and Circular Dependencies in My Game Engine"
date: 2025-05-20
categories: 
  - "coding"
tags: 
  - "a"
  - "about"
  - "dependency"
  - "development"
  - "injection"
  - "log"
  - "pakcets"
  - "rant"
  - "reoria"
---

Hey everyone,

Just wanted to vent a bit about a complication I ran into this morning while working on my game engine and networking code. Specifically, I hit a wall while trying to implement a feature that allows the server to gracefully close a client’s connection with an error message — instead of just abruptly terminating the SSL and TCP connections with no details or way to handle the disconnection without crashing the game loop.

### The Problem

My initial idea was to create a `CloseSecureSocketPacket` class to handle this gracefully, like so:

using LiteNetLib.Utils; using Microsoft.Extensions.Logging; using Asreyion.Networking.Packets.Interfaces; using Asreyion.Networking.Sockets.Interfaces; <div></div> namespace Asreyion.Networking.Sockets.Packets; <div></div> public class CloseSecureSocketPacket(ILogger<CloseSecureSocketPacket> logger, ISecureSocket secureSocket) : IPacket { protected readonly ILogger<CloseSecureSocketPacket> Logger = logger; protected readonly ISecureSocket SecureSocket = secureSocket; <div></div> public string PacketIdentifier => "Sockets.CloseSecureSocketPacket"; <div></div> public void Deserialize(NetDataReader reader) { string message = reader.GetString(); <div></div> \_ = this.SecureSocket.DisconnectAsync(); <div></div> this.Logger.LogInformation("Secure socket was closed by remote host, reason: {message}", message); } <div></div> public void Serialize(NetDataWriter writer) => writer.Put("Connection has been closed by the server."); }

```
using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Asreyion.Networking.Packets.Interfaces;
using Asreyion.Networking.Sockets.Interfaces;

namespace Asreyion.Networking.Sockets.Packets;

public class CloseSecureSocketPacket(ILogger<CloseSecureSocketPacket> logger, ISecureSocket secureSocket) : IPacket
{
    protected readonly ILogger<CloseSecureSocketPacket> Logger = logger;
    protected readonly ISecureSocket SecureSocket = secureSocket;

    public string PacketIdentifier => "Sockets.CloseSecureSocketPacket";

    public void Deserialize(NetDataReader reader)
    {
        string message = reader.GetString();

        _ = this.SecureSocket.DisconnectAsync();

        this.Logger.LogInformation("Secure socket was closed by remote host, reason: {message}", message);
    }

    public void Serialize(NetDataWriter writer) 
        => writer.Put("Connection has been closed by the server.");
}
```

The idea was simple: the server would send this packet to gracefully close the connection, logging the reason for the disconnection and handling it cleanly.

But here’s where things got tricky: while the packet system fully supports dependency injection, my `ISecureSocket` class is responsible for handling calls to the `IPacketRegistry`, which requires an `IEnumerable<IPacket>` from the DI container. This creates a **circular dependency** — `ISecureSocket` needs `IPacketRegistry`, which needs `CloseSecureSocketPacket` (an `IPacket`), which needs `ISecureSocket`. And it loops on and on.

### The Temporary Fix

For now, I worked around this by creating a simple "flag" class that holds a boolean value to signal whether the socket should be closed. The game loop then checks this class at the end of each update cycle to see if the connection needs to be terminated. While this solution works for now, it’s not ideal long-term.

I’m not thrilled with this fix, as it feels like a temporary band-aid rather than a proper solution, but it works for the time being.

### Why I Chose This Path

One of the reasons I decided to tackle game development this way — mostly from scratch using MonoGame and some additional libraries — is because of the challenges like these. I wanted the opportunity to run into these little nightmares and figure out how to solve them on my own, while still keeping the game’s codebase clean and maintainable. It's part of the fun and reason I didn’t want to use engines like Godot, Unity, or RPG Maker — I wanted to build something that I could shape and optimize according to my own needs.

That said, I’ve made progress! I now have a basic timeout mechanism in place on the server that sends the `CloseSecureSocketPacket` after 5 seconds in the game loop. Before this, the only way to close a connection was to abruptly kill the socket mid-game loop, which obviously comes with its own set of problems.

### What's Next

For now, I’ll be focusing on improving the core networking code. But I’m happy to have a functioning workaround in place for the time being. It’s not perfect, but it’s a start. I’ll keep pushing forward and share more updates as the networking system develops.

Below are some screenshots of the server and client running the mock code. I’ll post another update once I have more progress to show.

![](images/image.png)

![](images/image-1.png)

### Follow the Project

If you want to stay up to date with the progress of this game and its networking code, you can track my commits on GitHub:

- [Game Project Repository](https://github.com/XerShade/Reoria)

- [Networking Code Repository](https://github.com/XerShade/Asreyion.Networking)

Thanks for reading,  
\-XerShade
