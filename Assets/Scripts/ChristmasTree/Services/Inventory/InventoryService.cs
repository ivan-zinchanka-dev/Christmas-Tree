using System;
using System.Collections.Generic;
using System.Linq;
using ChristmasTree.Services.Projectiles;
using JanZinch.Services.ObjectPooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ChristmasTree.Services.Inventory
{
    public class InventoryService
    {
        private readonly InventoryConfig _inventoryConfig;
        private readonly Dictionary<DecorationType, int> _inventory = new();
        private readonly Dictionary<DecorationType, ObjectPool> _objectPools = new();

        private Transform _defaultProjectilesParent;
        
        public bool IsEmpty => _inventory.Count == 0;
        public bool StarSpent { get; private set; }
        public event Action OnEmpty;
        
        public InventoryService(InventoryConfig inventoryConfig)
        {
            _inventoryConfig = inventoryConfig;
            _defaultProjectilesParent = new GameObject("inventory").transform;
            
            foreach (DecorationData data in _inventoryConfig.Inventory)
            {
                _inventory.Add(data.Type, data.Amount);
                _objectPools.Add(data.Type, new ObjectPool(data.Amount, data.Prefab, _defaultProjectilesParent));
            }

            DecorationData star = _inventoryConfig.Star;
            _objectPools.Add(star.Type, new ObjectPool(1, star.Prefab, _defaultProjectilesParent));
        }

        public InventoryService Refresh()
        {
            _inventory.Clear();
            
            foreach (DecorationData data in _inventoryConfig.Inventory)
            {
                _inventory.Add(data.Type, data.Amount);
            }

            StarSpent = false;
            return this;
        }

        public bool TryGetProjectile(Vector3 position, Quaternion rotation, out Projectile projectile)
        {
            projectile = GetProjectile(position, rotation);
            return projectile != null;
        }

        public Projectile GetProjectile(Vector3 position, Quaternion rotation)
        {
            if (IsEmpty && StarSpent)
            {
                return null;
            }

            DecorationType decorationType = GetDecorationType();
            Projectile result = (Projectile) _objectPools[decorationType].GetObject();
            
            if (result != null)
            {
                result.transform.position = position;
                result.transform.rotation = rotation;
                
                if (_inventoryConfig.IsMassive(decorationType)) {

                    result.IsMassive = true;
                }
                
                return result;
            }
            else {

                Debug.LogException(new Exception("Bad projectile"));
                return null;
            }
        }

        private DecorationType GetDecorationType()
        {
            if (IsEmpty)
            {
                StarSpent = true;
                return _inventoryConfig.Star.Type;
            }
            else
            {
                var pair = _inventory.ElementAt(Random.Range(0, _inventory.Count));

                if (pair.Value > 1)
                {
                    _inventory[pair.Key] -= 1;
                }
                else
                {
                    _inventory.Remove(pair.Key);

                    if (IsEmpty)
                    {
                        OnEmpty?.Invoke();
                    }
                }

                return pair.Key;
            }
        }
    }
}