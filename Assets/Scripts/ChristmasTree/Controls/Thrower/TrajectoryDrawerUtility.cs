using UnityEngine;

namespace ChristmasTree.Controls.Thrower
{
    public static class TrajectoryDrawerUtility
    {
        public static void RemoveTrajectory(in TrajectoryDrawerComponent drawerComponent) {

            drawerComponent.LineRenderer.positionCount = default;            
        }
        
        public static void UpdateTrajectory(in TrajectoryDrawerComponent drawerComponent, Rigidbody projectile, Vector3 startPoint, Vector3 force) {

            Vector3 velocity = force / projectile.mass * Time.fixedDeltaTime;
            float flightDuration = 2 * velocity.y / Physics.gravity.y;
            float stepTime = flightDuration / drawerComponent.SegmentCount;

            drawerComponent.Points.Clear();

            for (int i = 0; i < drawerComponent.PointsCount; i++) {

                float stepTimePassed = stepTime * i;

                Vector3 movement = new Vector3()
                {
                    x = velocity.x * stepTimePassed,
                    y = velocity.y * stepTimePassed - 0.5f * Physics.gravity.y * Mathf.Pow(stepTimePassed, 2),
                    z = velocity.z * stepTimePassed
                };

                if (Physics.Raycast(startPoint, -movement, out RaycastHit hit, movement.magnitude)) break;
                                     
                drawerComponent.Points.Add(-movement + startPoint);
            }

            drawerComponent.LineRenderer.positionCount = drawerComponent.Points.Count;
            drawerComponent.LineRenderer.SetPositions(drawerComponent.Points.ToArray());
        }
        
    }
}