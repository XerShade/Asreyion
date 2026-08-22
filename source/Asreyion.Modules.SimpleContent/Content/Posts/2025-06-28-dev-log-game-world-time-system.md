---
title: "Dev Log: Game World Time System"
date: 2025-06-28
categories: 
  - "coding"
tags: 
  - "game"
  - "system"
  - "time"
  - "world"
---

So I wanted to make a quick little post to show and talk about the time scale system I made for my game project today. Nothing too fancy just a system I can plug into my MonoGame.Extended world which will calculate and emit signals (events) based on how in-game time passes that other game systems can listen for.

```csharp
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS.Systems;
using Asreyion.Common;
using Asreyion.Signals.Interfaces;
using GameWorld = MonoGame.Extended.ECS.World;

namespace Reoria.Game.World.Systems;

/// <summary>
/// Simulates in-game time progression, emitting events for minute, hour, and day ticks.
/// Time progresses based on a configurable real-time to game-time ratio.
/// </summary>
public class WorldTimeSystem(ISignalBus signalBus) : Disposable, IUpdateSystem
{
    /// <summary>
    /// Number of in-game minutes in an hour.
    /// </summary>
    public const float MINUTES_PER_HOUR = 60.0f;

    /// <summary>
    /// Number of in-game hours in a day.
    /// </summary>
    public const float HOURS_PER_DAY = 24.0f;

    /// <summary>
    /// Real-world minutes it takes for a full in-game day to pass.
    /// </summary>
    public const float REAL_MINUTES_PER_GAME_DAY = 70.0f;

    /// <summary>
    /// Signal bus for emitting world time-related events.
    /// </summary>
    protected readonly ISignalBus SignalBus = signalBus;

    /// <summary>
    /// The rate at which in-game time progresses relative to real-world time.
    /// </summary>
    public readonly float Timescale = MINUTES_PER_HOUR * HOURS_PER_DAY / REAL_MINUTES_PER_GAME_DAY;

    /// <summary>
    /// The world time recorded at the last update tick.
    /// </summary>
    /// <remarks>Used to detect time unit changes.</remarks>
    protected TimeSpan LastWorldTime { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// The current total elapsed in-game time.
    /// </summary>
    public TimeSpan CurrentWorldTime { get; protected set; } = TimeSpan.Zero;

    /// <summary>
    /// Initializes the system and emits an initialization signal.
    /// </summary>
    /// <param name="world">The ECS world instance.</param>
    public void Initialize(GameWorld world) 
        => this.SignalBus.Emit("WorldTimeSystem.OnInitialize", this);

    /// <summary>
    /// Updates the world time, calculates time progression, and emits signals for time unit ticks.
    /// </summary>
    /// <param name="gameTime">Snapshot of elapsed game time.</param>
    public void Update(GameTime gameTime)
    {
        // Progress world time scaled to the defined timescale
        this.CurrentWorldTime += gameTime.ElapsedGameTime * this.Timescale;
        this.SignalBus.Emit("WorldTimeSystem.OnUpdate", this);

        // Check to see if the number of minutes has changed.
        if (this.LastWorldTime.Minutes != this.CurrentWorldTime.Minutes)
        {
            // It has, please emit a signal to listen for.
            this.SignalBus.Emit("WorldTimeSystem.OnMinuteTick", this);
        }

        // Check to see if the number of hours has changed.
        if (this.LastWorldTime.Hours != this.CurrentWorldTime.Hours)
        {
            // It has, please emit a signal to listen for.
            this.SignalBus.Emit("WorldTimeSystem.OnHourTick", this);
        }

        // Check to see if the number of days has changed.
        if (this.LastWorldTime.Days != this.CurrentWorldTime.Days)
        {
            // It has, please emit a signal to listen for.
            this.SignalBus.Emit("WorldTimeSystem.OnDayTick", this);
        }

        // Update the tracker for the last game tick.
        this.LastWorldTime = this.CurrentWorldTime;
    }
}
```

Out of the box I have it configured to a 20.5714285714 timescale, which is about 70 minutes equals one in-game day. If you wanted to use this without my engine's signal bus interface you can just replace the calls to `this.SignalBus.Emit` with actual events or some other system. Also I defined `Timescale` as a readonly float instead of a constant so if someone made an override for this class they could change it in the constructor.

I might add support to sync it up with real time later by scaling real time to the game world timescale, this would be helpful in a multi-server setup for scaling, but that's a feature I'll implement later I just wanted to get this working so I could work on things that need it for now. I may also add a version that isn't synced to real time, but saves and loads its timespans so they persist between restarts.

The full and current code is available in my game's GitHub repository at: [https://github.com/XerShade/Reoria/blob/0.1.0/source/Reoria.Game/World/Systems/WorldTimeSystem.cs](https://github.com/XerShade/Reoria/blob/0.1.0/source/Reoria.Game/World/Systems/WorldTimeSystem.cs)
