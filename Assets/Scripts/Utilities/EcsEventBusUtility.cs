﻿using Leopotam.Ecs;
using Management;

namespace Utilities
{
    public static class EcsEventBusUtility
    {
        public static void FireOneFrameComponent<T>(T component) where T : struct
        {
            EcsEntity entity = EcsStartup.Instance.World.NewEntity();
            entity.Replace(component);
        }
        
        // TODO Make Listener System for GameStateMachine and ScoreService
    }
}