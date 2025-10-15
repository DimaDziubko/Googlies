using System;
using _Game.Core._Logger;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace _Game.Gameplay._Units.Scripts._Animation
{
    public class SpineUnitAnimator : BaseUnitAnimator
    {
        private const int BASE_LAYER = 0;
        private const int COMBAT_LAYER = 1;
        private const int ADDITIVE_LAYER = 2;

        private const float WALK_SPEED_THRESHOLD = 0.01f;

        enum AnimationState
        {
            None = 0,
            Ilde = 1,
            Walk = 2,
            AggroIntro = 3,
            Aggro = 4,
            Attack = 5,
            Death = 6
        }

        [SerializeField, Required] private AnimationReferenceAsset _idleBottom;
        [SerializeField, Required] private AnimationReferenceAsset _idleTop;

        [SerializeField, Required] private AnimationReferenceAsset _walkBottom;
        [SerializeField, Required] private AnimationReferenceAsset _walkTop;

        [ShowIf(nameof(_isAttack))]
        [SerializeField] private AnimationReferenceAsset[] _attacks;

        [SerializeField] private bool _isAttack = true;

        [ShowIf(nameof(_isAggroIntro))]
        [SerializeField] private AnimationReferenceAsset _aggroIntro;

        [ShowIf(nameof(_isAggro))]
        [SerializeField] private AnimationReferenceAsset _aggro;

        [ShowIf(nameof(_isDeath))]
        [SerializeField] private AnimationReferenceAsset _death;

        [SerializeField] bool _isAggroIntro = false;
        [SerializeField] bool _isAggro = false;
        [SerializeField] bool _isDeath = false;

        [ShowIf(nameof(_isAiming))]
        [SerializeField] private AnimationReferenceAsset _aimingIn;
        [ShowIf(nameof(_isAiming))]
        [SerializeField] private AnimationReferenceAsset _aimingOut;

        [SerializeField, Required] SkeletonAnimation _skeletonAnimation;

        [SerializeField] bool _isAiming = false;

        [ShowInInspector, ReadOnly]
        private float _attackPerSecond = 1;

        //[ShowInInspector, ReadOnly]
        //private float _testSpeed = 2;

        private float _defaultSpeed;
        private float _cachedSpeed;
        private float _tempAnimationSpeed;

        private TrackEntry _aimEntry;

        [ShowIf(nameof(_isAiming))]
        [SerializeField] private AimingReferences _settings;
        [ShowIf(nameof(_isAiming))]
        [SerializeField] private AimingSettings _refferences;

        [SerializeField] private float _locomotionBaseSpeed = 0.05f;

        [ShowInInspector, ReadOnly]
        private AnimationTrackPlayer _bottomTrackPlayer;
        [ShowInInspector, ReadOnly]
        private AnimationTrackPlayer _topTrackPlayer;
        [ShowInInspector, ReadOnly]
        private AnimationTrackPlayer _additiveTrackPlayer;


        [ShowInInspector, ReadOnly]
        private AnimationState _bottomLayerAnimationState = AnimationState.None;
        [ShowInInspector, ReadOnly]
        private AnimationState _topLayerAnimationState = AnimationState.None;

        [ShowInInspector, ReadOnly]
        private bool _isAggressive = false;

        [ShowInInspector, ReadOnly]
        private bool _isAimingActive = false;

        protected override void OnInitialize()
        {
            _defaultSpeed = _skeletonAnimation.timeScale;

            //New
            _bottomTrackPlayer = new AnimationTrackPlayer(_skeletonAnimation);
            _topTrackPlayer = new AnimationTrackPlayer(_skeletonAnimation);
            _additiveTrackPlayer = new AnimationTrackPlayer(_skeletonAnimation);
        }

        public override void SetAttackSpeed(float attackPerSecond)
        {
            _attackPerSecond = attackPerSecond;
            if (_topTrackPlayer == null) return;
            float requiredTimeScale = _topTrackPlayer.GetCurrentTrackDuration() / (1 / _attackPerSecond);
            _topTrackPlayer.SetTimeScale(requiredTimeScale);
        }

        public override void GameUpdate(float deltaTime) =>
            _skeletonAnimation.Update(deltaTime);

        public override void LateGameUpdate(float deltaTime)
        {

        }


        private void OnDrawGizmosSelected()
        {
            //             if (!_isAiming || _targetTransform.OrNull() == null)
            //                 return;
            //
            //             Vector3 center = _aimBoneTransform.position;
            //             bool facingRight = true;
            //
            // #if UNITY_EDITOR
            //             if (_originTransform != null)
            //                 facingRight = _originTransform.position.x <= TargetPosition.x;
            // #endif
            //
            //             Vector3 forward = facingRight ? Vector3.right : Vector3.left;
            //             
            //             Gizmos.color = Color.yellow;
            //             Gizmos.DrawWireSphere(center, _maxAimDistance);
            //             
            //             Vector3 minDir = Quaternion.AngleAxis(_angleRange.Min, Vector3.forward) * forward;
            //             Vector3 maxDir = Quaternion.AngleAxis(_angleRange.Max, Vector3.forward) * forward;
            //
            //             Gizmos.color = Color.red;
            //             Gizmos.DrawLine(center, center + minDir * _maxAimDistance);
            //             Gizmos.DrawLine(center, center + maxDir * _maxAimDistance);
        }

        public override void PlayAttack()
        {
            if (!_isAttack || _attacks.Length == 0)
                return;

            if (_topLayerAnimationState != AnimationState.Attack ||
                (_topLayerAnimationState == AnimationState.Attack && !_topTrackPlayer.IsPlaying))
            {
                _topTrackPlayer.Play(SelectRandomAttack(), COMBAT_LAYER);
            }

            _topLayerAnimationState = AnimationState.Attack;
        }

        private AnimationReferenceAsset SelectRandomAttack()
        {
            int randomIndex = UnityEngine.Random.Range(0, _attacks.Length);
            return _attacks[randomIndex];
        }

        public override void TryPlayAggro(Action onComplete = null)
        {
            if (_isAggroIntro && !_isAggressive)
            {
                _topTrackPlayer.Play(_aggroIntro, () =>
                {
                    onComplete?.Invoke();
                    _isAggressive = true;
                    _topLayerAnimationState = AnimationState.Aggro;
                    _logger.Log("AGGRO INTRO COMPLETE", DebugStatus.Info);
                }, COMBAT_LAYER);

                _topLayerAnimationState = AnimationState.AggroIntro;
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        private void TryPlayAggroLoop(float currentSpeed)
        {
            float requiredTimeScale = currentSpeed / _locomotionBaseSpeed;

            if (_topLayerAnimationState != AnimationState.Aggro ||
               (_topLayerAnimationState == AnimationState.Aggro && !_topTrackPlayer.IsPlaying))
                _topTrackPlayer.PlayLoop(_aggro, COMBAT_LAYER);

            _topTrackPlayer.SetTimeScale(requiredTimeScale);

            _logger.Log("PLAY AGGRO LOOP", DebugStatus.Info);

            _topLayerAnimationState = AnimationState.Aggro;
        }

        public override void TryStartAiming()
        {
            if (_topLayerAnimationState == AnimationState.AggroIntro) return;

            _logger.Log("PLAY START AIMING", DebugStatus.Info);

            if (_isAiming && !_isAimingActive)
            {
                _additiveTrackPlayer.Play(_aimingIn, ADDITIVE_LAYER);
                _isAimingActive = true;
            }
        }

        public override void TryStopAiming()
        {
            _logger.Log("PLAY STOP AIMING", DebugStatus.Info);

            if (_isAiming && _isAimingActive)
            {
                _additiveTrackPlayer.Play(_aimingOut, ADDITIVE_LAYER);
                _isAimingActive = false;
            }
        }

        public override void SetTarget(Transform targetTransform)
        {
            //if (_isAiming) _additiveTrackPlayer.Play(_aimingIn, ADDITIVE_LAYER);
        }

        public override void StopAttack()
        {

        }

        public override void PlayDeath(Action onComplete = null)
        {
            if (_isDeath)
            {
                _topTrackPlayer.Play(_death, onComplete, COMBAT_LAYER);
                _topLayerAnimationState = AnimationState.Death;
            }
            else onComplete?.Invoke();
        }

        public override void SetSpeedFactor(float speedFactor) =>
            _skeletonAnimation.timeScale = speedFactor * _defaultSpeed;

        public override void ResetSpeed() =>
            _skeletonAnimation.timeScale = _defaultSpeed;

        public override void SetPaused(bool isPaused)
        {
            if (isPaused)
            {
                _tempAnimationSpeed = _skeletonAnimation.timeScale;
            }

            SetSpeed(isPaused ? 0 : _tempAnimationSpeed);
        }

        public override void CleanUp()
        {
            _logger.Log("SPINE UNIT ANIMATOR CLEANUP", DebugStatus.Success);

            _bottomTrackPlayer?.Cleanup();
            _topTrackPlayer?.Cleanup();
            _additiveTrackPlayer?.Cleanup();

            _bottomLayerAnimationState = AnimationState.None;
            _topLayerAnimationState = AnimationState.None;
        }

        public override void SetTangent(Vector3 tangent)
        {

        }

        public override void PlayBottomLocomotion(float currentSpeed)
        {
            if (_bottomTrackPlayer == null) return;

            var currentTrack = _skeletonAnimation.AnimationState.GetCurrent(BASE_LAYER);

            if (currentTrack == null || currentTrack.Animation == null || !_bottomTrackPlayer.IsPlaying || !_bottomTrackPlayer.IsTrackEntry)
            {
                _bottomLayerAnimationState = AnimationState.None;
            }

            float requiredTimeScale = currentSpeed / _locomotionBaseSpeed;

            if (currentSpeed >= WALK_SPEED_THRESHOLD)
            {
                if (_bottomLayerAnimationState != AnimationState.Walk)
                {
                    _bottomTrackPlayer.PlayLoop(_walkBottom);
                    _bottomLayerAnimationState = AnimationState.Walk;
                    _logger.Log("PLAY_BOTTOM_LOCOMOTION WALK", DebugStatus.Info);
                }

                _bottomTrackPlayer.SetTimeScale(requiredTimeScale);
                return;
            }

            if (_bottomLayerAnimationState != AnimationState.Ilde)
                _bottomTrackPlayer.PlayLoop(_idleBottom);

            _bottomTrackPlayer.SetTimeScale(requiredTimeScale);

            _logger.Log("PLAY_BOTTOM_LOCOMOTION IDLE", DebugStatus.Info);

            _bottomLayerAnimationState = AnimationState.Ilde;

        }

        public override void PlayTopLocomotion(float currentSpeed)
        {
            if (_topTrackPlayer == null) return;

            var currentTrack = _skeletonAnimation.AnimationState.GetCurrent(COMBAT_LAYER);

            if (currentTrack == null || currentTrack.Animation == null || !_topTrackPlayer.IsPlaying | !_bottomTrackPlayer.IsTrackEntry)
            {
                _topLayerAnimationState = AnimationState.None;
            }

            float requiredTimeScale = currentSpeed / _locomotionBaseSpeed;

            if (currentSpeed >= WALK_SPEED_THRESHOLD)
            {
                if (_topLayerAnimationState != AnimationState.Walk)
                {
                    _topTrackPlayer.PlayLoop(_walkTop, COMBAT_LAYER);
                    _topLayerAnimationState = AnimationState.Walk;

                    _logger.Log("PLAY_TOP_LOCOMOTION WALK", DebugStatus.Info);
                }

                _topTrackPlayer.SetTimeScale(requiredTimeScale);
                return;
            }

            if (_topLayerAnimationState != AnimationState.Ilde)
                _topTrackPlayer.PlayLoop(_idleTop, COMBAT_LAYER);

            _topTrackPlayer.SetTimeScale(requiredTimeScale);

            _logger.Log("PLAY_BOTTOM_LOCOMOTION IDLE", DebugStatus.Info);

            _topLayerAnimationState = AnimationState.Ilde;
        }

        public override void TryPlayAggroLocomotion(float currentSpeed)
        {
            _logger.Log("PLAY_AGGRO_LOCOMOTION", DebugStatus.Info);

            if (_topLayerAnimationState == AnimationState.AggroIntro) return;

            if (_isAggressive)
            {
                TryPlayAggroLoop(currentSpeed);
                return;
            }

            if (!_isAggro)
                PlayTopLocomotion(currentSpeed);
        }

        public override void UpdateLocomotionSpeed(float currentSpeed)
        {

        }

        private void SetSpeed(float speed) => _skeletonAnimation.timeScale = speed;

        public override void ResetPose()
        {
            _logger.Log("RESET POSE", DebugStatus.Success);

            _skeletonAnimation.AnimationState.ClearTracks();
            _skeletonAnimation.Skeleton.SetToSetupPose();
            _bottomLayerAnimationState = AnimationState.None;
            _topLayerAnimationState = AnimationState.None;
        }

        //Delete later
        public override void PlayWalk()
        {

        }

        //Delete later
        public override void PlayIdle()
        {

        }

        public override void TryCalmDown()
        {
            _isAggressive = false;
            TryStopAiming();
        }


#if UNITY_EDITOR
        [Space, Header("------------------------ INIT ------------------------ ")]
        [FolderPath]
        [SerializeField] private string _animationFolderPath = "Assets/_Game/Bundles/Timeline_1/Units/Spine/Warrior_6";
#endif
        [Button("AUTO INIT PARAMS"), PropertyOrder(0)]
        private void LoadAnimations()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(_animationFolderPath))
            {
                Debug.LogError("Animation folder path is empty!");
                return;
            }
            // Загружаем все AnimationReferenceAsset из папки
            string[] guids = AssetDatabase.FindAssets("t:AnimationReferenceAsset", new[] { _animationFolderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AnimationReferenceAsset asset = AssetDatabase.LoadAssetAtPath<AnimationReferenceAsset>(path);
                if (asset == null) continue;

                // Нормализуем имя - убираем _ и делаем lowercase для сравнения
                string animName = asset.name.Replace("_", "").ToLower();

                // Автоматическое назначение по имени
                if (animName.Contains("idlebottom"))
                    _idleBottom = asset;
                else if (animName.Contains("idletop"))
                    _idleTop = asset;
                else if (animName.Contains("walkbottom"))
                    _walkBottom = asset;
                else if (animName.Contains("walktop"))
                    _walkTop = asset;
                else if (animName.Contains("attack"))
                {
                    if (_attacks == null) _attacks = new AnimationReferenceAsset[0];
                    var list = _attacks.ToList();
                    if (!list.Contains(asset))
                        list.Add(asset);
                    _attacks = list.ToArray();
                }
                else if (animName.Contains("aimingin") || animName.Contains("aimingout"))
                {
                    _isAiming = true;
                    if (animName.Contains("aimingin"))
                        _aimingIn = asset;
                    else
                        _aimingOut = asset;
                }
                else if (animName.Contains("aggrointro"))
                {
                    _isAggroIntro = true;
                    _aggroIntro = asset;
                }
                else if (animName.Contains("aggro") && !animName.Contains("intro"))
                {
                    _isAggro = true;
                    _aggro = asset;
                }
                else if (animName.Contains("death"))
                {
                    _isDeath = true;
                    _death = asset;
                }
            }
            EditorUtility.SetDirty(this);
            Debug.Log($"Loaded {guids.Length} animations from {_animationFolderPath}");
#endif
        }
    }
}