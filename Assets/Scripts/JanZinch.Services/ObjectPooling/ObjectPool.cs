using System.Collections.Generic;
using UnityEngine;

namespace JanZinch.Services.ObjectPooling
{
    public class ObjectPool
    {
        private readonly List<FloatingObject> _objects;
        private readonly Transform _defaultParent;
        
        public ObjectPool(int count, FloatingObject source, Transform defaultParent = null) {

            _objects = new List<FloatingObject>();
            _defaultParent = defaultParent;
            
            for (int i = 0; i < count; i++)
            {
                AddObject(source);
            }
        }
        
        private void AddObject(FloatingObject source) {

            FloatingObject newObject = Object.Instantiate<FloatingObject>(source, _defaultParent)
                .SetDefaultParent(_defaultParent);
            
            newObject.name = source.name;
            _objects.Add(newObject);
            newObject.gameObject.SetActive(false);
        }

        public FloatingObject GetObject() {

            for (int i = 0; i < _objects.Count; i++) {

                if (_objects[i].gameObject.activeInHierarchy == false) {

                    _objects[i].gameObject.SetActive(true);
                    return _objects[i];
                }
            }

            AddObject(_objects[0]);
            return _objects[_objects.Count - 1];
        }
   
    }
}