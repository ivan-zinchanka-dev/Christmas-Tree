using ChristmasTree.Management;
using Leopotam.Ecs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ChristmasTree.Controls.Platform
{
    public class RotatingPlatformSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private static readonly Vector2 TiltRange = new Vector2(0.0f, 3.5f);
        private const float Eps = 0.0001f;
        private const float Acceleration = 35.0f;
        
        private readonly EcsFilter<RotatingPlatformComponent> _rotatingPlatformFilter = null;
        private readonly GameStateMachine _gameStateMachine;
        private Vector3 _targetEulerAngles;
        
        private static int GetRandomDirection()
        {
            return Random.Range(0, 2) == 0 ? -1 : 1;
        }
        
        private static float GetRandomTilt()
        {
            return Random.Range(TiltRange.x, TiltRange.y);
        }
        
        private static bool ApproximatelyEquals(float a, float b) {
            
            return Mathf.Abs(a - b) < Eps;
        }
        
        public void Init()
        {
            _gameStateMachine.OnNewStage += OnNewGameStage;
            
            foreach (int i in _rotatingPlatformFilter)
            {
                ref RotatingPlatformComponent rotatingPlatformComponent = ref _rotatingPlatformFilter.Get1(i);

                rotatingPlatformComponent.CurrentRotationSpeed = rotatingPlatformComponent.TargetRotationSpeed 
                    = rotatingPlatformComponent.DefaultRotationSpeed;
            }
        }

        public void Run()
        {
            foreach (int i in _rotatingPlatformFilter)
            {
                ref RotatingPlatformComponent rotatingPlatformComponent = ref _rotatingPlatformFilter.Get1(i);
                
                TiltIfNeed(rotatingPlatformComponent.Transform, _targetEulerAngles);
                
                rotatingPlatformComponent.Transform
                    .Rotate(Vector3.up, rotatingPlatformComponent.CurrentRotationSpeed.y * Time.deltaTime, Space.World);

                AccelerateRotationIfNeed(ref rotatingPlatformComponent);
            }
        }
        
        private void OnNewGameStage(GameStage gameStage)
        {
            foreach (int i in _rotatingPlatformFilter)
            {
                ref RotatingPlatformComponent rotatingPlatformComponent = ref _rotatingPlatformFilter.Get1(i);
                
                rotatingPlatformComponent.TargetRotationSpeed = 
                    rotatingPlatformComponent.DefaultRotationSpeed 
                    * Random.Range(gameStage.MinPlatformSpeedMultiplier, gameStage.MaxPlatformSpeedMultiplier)
                    * GetRandomDirection();

                if (gameStage.IsDefault)
                {
                    _targetEulerAngles = new Vector3(0.0f, rotatingPlatformComponent.Transform.eulerAngles.y, 0.0f);
                }
                else
                {
                    _targetEulerAngles = new Vector3(
                        GetRandomTilt(), 
                        rotatingPlatformComponent.Transform.eulerAngles.y, 
                        GetRandomTilt());
                }
            }
        }
        
        private static void TiltIfNeed(Transform rotatingPlatformTransform, Vector3 targetEulerAngles)
        {
            Vector3 startEulerAngles = rotatingPlatformTransform.eulerAngles;
            
            if (!ApproximatelyEquals(rotatingPlatformTransform.eulerAngles.x, targetEulerAngles.x))
            {            
                float angleX = Mathf.LerpAngle(startEulerAngles.x, targetEulerAngles.x, Time.deltaTime);
                float angleZ = Mathf.LerpAngle(startEulerAngles.z, targetEulerAngles.z, Time.deltaTime);

                rotatingPlatformTransform.eulerAngles = 
                    new Vector3(angleX, rotatingPlatformTransform.eulerAngles.y, angleZ);
            }
        }
        
        private static void AccelerateRotationIfNeed(ref RotatingPlatformComponent platformComponent) {
            
            if (!ApproximatelyEquals(platformComponent.CurrentRotationSpeed.y, platformComponent.TargetRotationSpeed.y)) {

                float acceleration = Acceleration;
            
                if (platformComponent.CurrentRotationSpeed.y > platformComponent.TargetRotationSpeed.y) {

                    acceleration *= -1;
                }
                
                platformComponent.CurrentRotationSpeed.y += acceleration * Time.deltaTime;
            }
        }
        
        public void Destroy()
        {
            _gameStateMachine.OnNewStage -= OnNewGameStage;
        }
    }
}