using ChristmasTree.Controls.Platform;
using ChristmasTree.Management;
using ChristmasTree.Services.Score;
using ChristmasTree.Utilities;
using UnityEngine;

namespace ChristmasTree.Services.Projectiles
{
    public class Star : Projectile
    {
        protected override void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<StarCatcherTag>(out StarCatcherTag catcher))
            {
                _rigidbody.isKinematic = true;
                State = ProjectileState.Attached;
                
                transform.position = catcher.Point.position;
                transform.SetParent(catcher.TreeTag.CaughtDecorationsParent, true);
                
                EcsEventBusUtility.FireOneFrameComponent(new OnScoreChangedComponent(_price));
                EcsEventBusUtility.FireOneFrameComponent(new OnStarSpent(true));
            }
            else if (State == ProjectileState.Launched && other
                         .TryGetComponent<ChristmasTreeTag>(out ChristmasTreeTag christmasTree)) {

                _rigidbody.velocity = Vector3.zero;
                /*Vector3 resist = Vector3.Normalize(Thrower.StartPoint.position - this.transform.position) * 
                                 christmasTree.BranchesForce;
                resist.y = 0.0f;
                _rigidbody.AddForce(resist);*/

                State = ProjectileState.Failed;
                EcsEventBusUtility.FireOneFrameComponent(new OnStarSpent(false));
            }
            else
            {
                State = ProjectileState.Failed;
                EcsEventBusUtility.FireOneFrameComponent(new OnStarSpent(false));
            }
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
            
            State = ProjectileState.Failed;
            EcsEventBusUtility.FireOneFrameComponent(new OnStarSpent(false));
        }
    }
}