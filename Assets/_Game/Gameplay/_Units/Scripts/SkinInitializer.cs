using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Utils.Extensions;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class SkinInitializer : MonoBehaviour
    {
        [SerializeField, Required] private SkeletonAnimation _skeletonAnimation;

        public void Initialize(Skin skin)
        {
            var skinToUse = skin.ToSpineSkin();

            if (_skeletonAnimation != null && _skeletonAnimation.skeleton != null)
            {
                _skeletonAnimation.skeleton.SetSlotsToSetupPose();

                var skinToCheck = _skeletonAnimation.skeleton.Data.FindSkin(skinToUse);
                if (skinToCheck != null)
                    _skeletonAnimation.skeleton.SetSkin(skinToUse);
                else
                    Debug.LogError($"Skin '{skinToUse}' not found in skeleton data.", this);
            }
        }


#if UNITY_EDITOR
        [Button]
        private void ManualInit()
        {
            _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        }
#endif

    }
}