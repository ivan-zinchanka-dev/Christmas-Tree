using System;
using Controls.ChristmasTree;
using Services.ObjectPooling;
using Services.Score;
using UnityEngine;
using Utilities;

namespace Services
{
    public class Projectile : FloatingObject
    {
        [SerializeField] protected Rigidbody _rigidbody = null;
        [SerializeField] protected int _price = default;
        
        public ProjectileState State { get; protected set; } = ProjectileState.Launched;
        public bool IsMassive { get; set; } = false;

        public Rigidbody Rigidbody => _rigidbody;
        public Action<ProjectileState> OnResult { get; private set; }

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.TryGetComponent<Projectile>(out Projectile otherProjectile))
            {
                if (State == ProjectileState.Failed || 
                    otherProjectile.State == ProjectileState.Failed) return;
                
                if (State == ProjectileState.Attached)
                {
                    EcsEventBusUtility.FireOneFrameComponent(new OnScoreChangedComponent(-1 * _price));
                }
                else if (otherProjectile.State == ProjectileState.Attached)
                {
                    EcsEventBusUtility.FireOneFrameComponent(new OnScoreChangedComponent(-1 * otherProjectile._price));
                }
                
                _rigidbody.isKinematic = false;
                State = ProjectileState.Failed;

                otherProjectile.Rigidbody.isKinematic = false;
                otherProjectile.State = ProjectileState.Failed;

            }
            else if (State != ProjectileState.Attached && collision.transform
                         .TryGetComponent<RotatingPlatformTag>(out RotatingPlatformTag rotatingPlatform)) {
                
                if (IsMassive)
                {
                    _rigidbody.isKinematic = true;
                    State = ProjectileState.Attached;
                    transform.SetParent(rotatingPlatform.CaughtDecorationsParent, true);
                    EcsEventBusUtility.FireOneFrameComponent(new OnScoreChangedComponent(_price));
                }
                else
                {
                    State = ProjectileState.Failed;
                }
            }
            
        }

        protected virtual void OnCollisionExit(Collision collision){

            if (State != ProjectileState.Attached && collision.transform
                    .TryGetComponent<RotatingPlatformTag>(out RotatingPlatformTag rotatingPlatform))
            {
                transform.SetParent(null, true);
                _rigidbody.isKinematic = false;
            }
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (State != ProjectileState.Attached && State != ProjectileState.Failed && 
                other.transform.TryGetComponent<ChristmasTreeTag>(out ChristmasTreeTag christmasTree))
            {
                if (!IsMassive) {
             
                    _rigidbody.isKinematic = true;
                    State = ProjectileState.Attached;
                    transform.SetParent(christmasTree.CaughtDecorationsParent, true);
                    
                    EcsEventBusUtility.FireOneFrameComponent(new OnScoreChangedComponent(_price));
                }       
            }
        }

        protected override void OnReturnToPool()
        {
            State = ProjectileState.Launched;
        }
    }
}