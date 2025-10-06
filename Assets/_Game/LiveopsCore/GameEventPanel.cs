using System.Collections.Generic;
using _Game.LiveopsCore._Enums;
using UnityEngine;

namespace _Game.LiveopsCore
{
    public class GameEventPanel : MonoBehaviour
    {
        [SerializeField]
        private List<SlotBinding> _slotBindings = new();

        private readonly Dictionary<GameEventSlotType, GameEventListView> _slotMap = new();

        private void Awake()
        {
            foreach (var binding in _slotBindings)
            {
                if (!_slotMap.ContainsKey(binding.SlotType))
                {
                    _slotMap.Add(binding.SlotType, binding.ListView);
                }
            }
        }

        public GameEventView SpawnElement(GameEventSlotType slot)
        {
            if (_slotMap.TryGetValue(slot, out var listView))
            {
                return listView.SpawnElement();
            }

            return GetFirstListView()?.SpawnElement();
        }

        public void RemoveElement(GameEventSlotType slot, GameEventView view)
        {
            if (_slotMap.TryGetValue(slot, out var listView))
            {
                listView.DestroyElement(view);
            }
        }

        public void Cleanup()
        {
            foreach (var listView in _slotMap.Values)
            {
                listView.Clear();
            }
        }

        private GameEventListView GetFirstListView()
        {
            foreach (var listView in _slotMap.Values)
            {
                return listView;
            }

            return null;
        }
    }
}