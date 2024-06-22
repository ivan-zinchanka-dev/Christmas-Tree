using Controls.Events;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Controls
{
    public class ThrowerEventsAdaptor : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent OnMouseInput { get; private set; }

        private void OnMouseDown()
        {
            EcsEventBusUtility.FireOneFrameComponent(new OnMouseDownComponent());
            OnMouseInput.Invoke();
        }

        private void OnMouseDrag()
        {
            EcsEventBusUtility.FireOneFrameComponent(new OnMouseDragComponent());
            OnMouseInput.Invoke();
        }
        
        private void OnMouseUp()
        {
            EcsEventBusUtility.FireOneFrameComponent(new OnMouseUpComponent());
            OnMouseInput.Invoke();
        }
    }
}