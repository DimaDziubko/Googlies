using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class BattlePassLootPoint : VfxEntity
    {
        [SerializeField, Required] private Transform _transform;
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private Image _iconHolder;
        
        [SerializeField] private float _duration = 2f;
        
        private float _age;

        [SerializeField] private DOTweenAnimation[] _animations;
        
        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }
        
        public void Initialize(Vector3 position)
        {
            _canvas.worldCamera = _cameraService.MainCamera;
            Position = position;
            _age = 0;
        }

        public void Play()
        {
            foreach (var anim in _animations)
            {
                anim.RecreateTween();
                anim.DOPlay();
            }
        }
        
        public override bool GameUpdate(float deltaTime)
        {
            _age += deltaTime;
            if (_age >= _duration)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            return true;
        }

        public override void Recycle()
        {
            foreach (var anim in _animations)
            {
                anim.DOKill();
            }

            base.Recycle();
        }
    }
}