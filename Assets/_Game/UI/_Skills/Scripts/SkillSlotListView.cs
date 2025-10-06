using System.Collections.Generic;
using UnityEngine;

namespace _Game.UI._Skills.Scripts
{
    public class SkillSlotListView : MonoBehaviour
    {
        [SerializeField] private List<SkillSlotView> _skillSlotView;
        
        public List<SkillSlotView> SkillSlotView => _skillSlotView;
    }
}