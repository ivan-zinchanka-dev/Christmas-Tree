using System.Collections.Generic;
using UnityEngine;

namespace ChristmasTree.Services.Inventory
{
    [CreateAssetMenu(fileName = "inventory_config", menuName = "Configs/InventoryConfig", order = 0)]
    public class InventoryConfig : ScriptableObject
    {
        [SerializeField] 
        private List<DecorationData> _inventory;
        [SerializeField] 
        private DecorationData _star;
        [SerializeField] 
        private List<DecorationType> _massiveDecorations;
        
        public bool IsMassive(DecorationType decorationType)
        {
            return _massiveDecorations.Contains(decorationType);
        }

        public Dictionary<DecorationType, int> GetInventory()
        {
            Dictionary<DecorationType, int> result = new Dictionary<DecorationType, int>(_inventory.Capacity);
            
            foreach (var pair in _inventory)
            {
                result.Add(pair.Type, pair.Amount);
            }

            return result;
        }

        public DecorationData GetDecorationData(DecorationType type)
        {
            return _inventory.Find(data => data.Type == type);
        }

        public DecorationData Star => _star;
        public IReadOnlyList<DecorationData> Inventory => _inventory;
    }
}