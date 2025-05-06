using ChristmasTree.Management;
using Leopotam.Ecs;

namespace ChristmasTree.Utilities
{
    public static class EcsEventBusUtility
    {
        public static void FireOneFrameComponent<T>(T component) where T : struct
        {
            EcsEntity entity = EcsStartup.Instance.World.NewEntity();
            entity.Replace(component);
        }
    }
}