using System.Collections.Generic;
using UnityEngine;

namespace JanZinch.Services.Common
{
    public class ViewDistributor : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> _views;

        public TView GetView<TView>() where TView : MonoBehaviour
        {
            foreach (var view in _views)
            {
                if (view is TView castedView)
                {
                    return castedView;
                }
            }

            return null;
        }
    }
}