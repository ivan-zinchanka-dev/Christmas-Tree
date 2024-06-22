using System.Collections;
using UnityEngine;

namespace Services.ObjectPooling
{
    public class FloatingObject : MonoBehaviour
    {      
        private Transform _defaultParent;

        public FloatingObject SetDefaultParent(Transform defaultParent)
        {
            _defaultParent = defaultParent;
            return this;
        }

        public IEnumerator ReturnToPool(float time)
        {
            yield return new WaitForSeconds(time);
            ReturnToPool();
        }

        public void ReturnToPool()
        {
            gameObject.transform.SetParent(_defaultParent, true);
            gameObject.SetActive(false);
            OnReturnToPool();
        }

        protected virtual void OnReturnToPool()
        {
            
        }

    }
}