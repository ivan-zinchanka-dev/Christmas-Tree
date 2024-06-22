using System;
using System.Collections.Generic;
using System.Threading;
using Controls.Events;
using Cysharp.Threading.Tasks;
using Leopotam.Ecs;
using Management;
using Services;
using Services.Inventory;
using UnityEngine;

namespace Controls.Thrower
{
    public class ThrowerSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilter<ThrowerComponent> _throwerFilter = null;
        
        private readonly EcsFilter<OnMouseDownComponent> _mouseDownFilter = null;
        private readonly EcsFilter<OnMouseDragComponent> _mouseDragFilter = null;
        private readonly EcsFilter<OnMouseUpComponent> _mouseUpFilter = null;
        private readonly EcsFilter<OnProjectileDroppedComponent> _projectileDroppedFilter = null;
        
        private Vector3 _startMousePoint;
        private Vector3 _currentMousePoint;
        private Projectile _heldProjectile;
        private readonly Stack<Projectile> _threwProjectiles = new Stack<Projectile>();
        
        private ThrowerComponent _throwerComponent;
        private CancellationTokenSource _rechargeCancellationToken = new CancellationTokenSource();
        
        private readonly GameStateMachine _gameStateMachine;
        private readonly InventoryService _inventoryService;
        
        public ThrowerSystem(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        
        public void Init()
        {
            foreach (int i in _throwerFilter)
            {
                _throwerComponent = _throwerFilter.Get1(i);
                break;
            }
            
            SetProjectileToSceneIfNeed();
        }
        
        private void SetProjectileToSceneIfNeed() {

            if (_inventoryService.TryGetProjectile(_throwerComponent.StartPoint.position, 
                    Quaternion.identity, out Projectile projectile))
            {
                _heldProjectile = projectile;
                _heldProjectile.Rigidbody.isKinematic = true;
            }
        }

        private async UniTaskVoid Recharge()
        {
            _rechargeCancellationToken = new CancellationTokenSource();
            
            await UniTask.Delay(
                TimeSpan.FromSeconds(_throwerComponent.Recharge), 
                DelayType.DeltaTime, 
                PlayerLoopTiming.FixedUpdate, 
                _rechargeCancellationToken.Token);
            
            SetProjectileToSceneIfNeed();
        }

        private void ReInit()
        {
            while (_threwProjectiles.Count > 0)
            {
                _threwProjectiles.Pop().ReturnToPool();
            }

            if (_heldProjectile != null)
            {
                _heldProjectile.ReturnToPool();
                _heldProjectile = null;
            }

            _gameStateMachine.Restart();
            SetProjectileToSceneIfNeed();
        }

        private void OnMouseDown()
        {
            if (_gameStateMachine.ActiveState == GameState.WaitingForRestart)
            {
                return;
            }

            if (_gameStateMachine.ActiveState == GameState.Preparing)
            {
                _gameStateMachine.Play();
            }

            if (_heldProjectile == null)
            {
                return;
            }

            if (_gameStateMachine.ActiveState == GameState.Preparing || 
                _gameStateMachine.ActiveState == GameState.Playing)
            {
                _startMousePoint = _currentMousePoint = Input.mousePosition;
                    
                TrajectoryDrawerUtility.UpdateTrajectory(
                    _throwerComponent.TrajectoryDrawerComponent, 
                    _heldProjectile.Rigidbody, 
                    _heldProjectile.transform.position, 
                    MakeForceFromScreen(_throwerComponent.ForceMultiplier));
            }
            
        }
        
        private void OnMouseDrag()
        {
            if (_gameStateMachine.ActiveState == GameState.WaitingForRestart || _heldProjectile == null)
            {
                return;
            }
            
            if (_gameStateMachine.ActiveState == GameState.Playing)
            {
                _currentMousePoint = Input.mousePosition;
            
                TrajectoryDrawerUtility.UpdateTrajectory(
                    _throwerComponent.TrajectoryDrawerComponent, 
                    _heldProjectile.Rigidbody, 
                    _heldProjectile.transform.position, 
                    MakeForceFromScreen(_throwerComponent.ForceMultiplier));
            }
            else
            {
                _heldProjectile.Rigidbody.isKinematic = false;
                _heldProjectile.Rigidbody.AddForce(Vector3.zero);
            }
        }
        
        private void OnMouseUp()
        {
            if (_gameStateMachine.ActiveState == GameState.WaitingForRestart)
            {
                ReInit();
                return;
            }
            
            if (_heldProjectile == null)
            {
                return;
            }
            
            _heldProjectile.Rigidbody.isKinematic = false;

            Vector3 force = _gameStateMachine.ActiveState == GameState.Playing
                ? MakeForceFromScreen(_throwerComponent.ForceMultiplier)
                : Vector3.zero;

            _heldProjectile.Rigidbody.AddForce(force);

            _threwProjectiles.Push(_heldProjectile);
            
            TrajectoryDrawerUtility.RemoveTrajectory(_throwerComponent.TrajectoryDrawerComponent);
            _heldProjectile = null;

            if (_gameStateMachine.ActiveState == GameState.Playing)
            {
                Recharge().Forget();
            }
        }
        
        private async UniTaskVoid TakeBackProjectileAsync(Projectile projectile, Vector3 targetPoint, float returningSpeed)
        {
            _rechargeCancellationToken?.Cancel();
            
            projectile.Rigidbody.isKinematic = true;
            projectile.transform.rotation = Quaternion.identity;

            _heldProjectile = projectile;
            
            while (projectile.transform.position != targetPoint) {

                projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, targetPoint, 
                    returningSpeed * Time.deltaTime);

                await UniTask.Yield();
            }

            await UniTask.Yield();   
        }
        
        public void Run()
        {
            if (_mouseDownFilter.GetEntitiesCount() > 0)
            {
                OnMouseDown();
            }
            
            if (_mouseDragFilter.GetEntitiesCount() > 0)
            {
                OnMouseDrag();
            }
            
            if (_mouseUpFilter.GetEntitiesCount() > 0)
            {
                OnMouseUp();
            }
            
            foreach (int i in _projectileDroppedFilter)
            {
                ref OnProjectileDroppedComponent projectileDroppedEvent = ref _projectileDroppedFilter.Get1(i);
                
                TakeBackProjectileAsync(
                    projectileDroppedEvent.Projectile, 
                    _throwerComponent.StartPoint.position, 
                    _throwerComponent.ReturningSpeed)
                    .Forget();
                
                break;
            }
            
        }
        
        private Vector3 MakeForceFromScreen(Vector3 forceMultiplier) {

            Vector3 difference = _startMousePoint - _currentMousePoint;

            difference.x *= forceMultiplier.x;
            difference.y *= forceMultiplier.y;
            difference.z = difference.y;
            
            if (difference.y < 0.0f) {

                difference.y = Mathf.Abs(difference.y);
                difference.z = Mathf.Abs(difference.z);
                difference.x *= -1;
            }

            return difference;
        }
        
    }
}