using System;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts._Animation
{
    public class AnimationTrackPlayer
    {
        private readonly SkeletonAnimation _skeletonAnimation;
        
        [ShowInInspector, ReadOnly]
        private TrackEntry _trackEntry;
    
        private Action _onComplete;
        private int _trackIndex;
        
        [ShowInInspector, ReadOnly]
        private bool _isPlaying;

        [ShowInInspector, ReadOnly] 
        private string AnimationName => _trackEntry?.Animation?.Name;
        
        public AnimationTrackPlayer(SkeletonAnimation skeletonAnimation)
        {
            _skeletonAnimation = skeletonAnimation;
        }

        public bool IsPlaying => _isPlaying;
        public bool IsTrackEntry => _trackEntry != null;

        public void Play(AnimationReferenceAsset animation, int trackIndex = 0)
        {
            InternalPlay(animation, trackIndex,  false, null);
        }
        
        public void PlayLoop(AnimationReferenceAsset animation, int trackIndex = 0)
        {
            InternalPlay(animation, trackIndex,  true, null);
        }
        
        public void Play(AnimationReferenceAsset animation, Action onComplete, int trackIndex = 0)
        {
            InternalPlay(animation, trackIndex, false, onComplete);
        }
        
        public void TryPlay(AnimationReferenceAsset animation, int trackIndex = 0)
        {
            if (_isPlaying) return;
            InternalPlay(animation, trackIndex,  false,null);
        }
        
        public void TryPlayThen(AnimationReferenceAsset animation,  Action onComplete, int trackIndex = 0)
        {
            if (_isPlaying) return;
            InternalPlay(animation, trackIndex, false, onComplete);
        }

        public void Stop(int trackIndex, float mixDuration = 0.2f)
        {
            if (_skeletonAnimation == null) return;

            if (_trackEntry != null)
            {
                Unsubscribe();
                _trackEntry = _skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndex, mixDuration);
                _isPlaying = false;
                _onComplete = null;
            }
        }
        
        public void SetTimeScale(float timeScale)
        {
            if(_trackEntry == null) return;
            _trackEntry.TimeScale = timeScale;
        }

        public void AddAnimation(AnimationReferenceAsset animation, int trackIndex = 0,  bool loop = false, float delay = 0, Action onComplete = null)
        {
            if (animation == null) return;

            var entry = _skeletonAnimation.AnimationState.AddAnimation(trackIndex, animation, loop, delay);

            if (onComplete != null && !loop)
            {
                entry.Complete += (e) => onComplete.Invoke();
                entry.Interrupt += (e) => onComplete.Invoke();
                entry.Dispose += (e) => onComplete.Invoke();
            }
        }
        
        private void InternalPlay(AnimationReferenceAsset animation, int trackIndex, bool loop, Action onComplete)
        {
            if (animation == null) return;

            Unsubscribe();
            
            _trackEntry = _skeletonAnimation.AnimationState.SetAnimation(trackIndex, animation, loop);
            _trackIndex = trackIndex;
            _isPlaying = true;

            _onComplete = onComplete;
            
            if(!loop)
                Subscribe();
        }

        private void Subscribe()
        {
            if (_trackEntry == null) return;

            _trackEntry.Complete += OnComplete;
            _trackEntry.Interrupt += OnInterrupt;
            _trackEntry.Dispose += OnDispose;
        }

        private void Unsubscribe()
        {
            if (_trackEntry == null) return;

            _trackEntry.Complete -= OnComplete;
            _trackEntry.Interrupt -= OnInterrupt;
            _trackEntry.Dispose -= OnDispose;
        }

        private void OnComplete(TrackEntry entry)
        {
            Finish();
        }

        private void OnInterrupt(TrackEntry entry)
        {
            Finish();
        }

        private void OnDispose(TrackEntry entry)
        {
            Finish();
        }

        private void Finish()
        {
            Unsubscribe();

            _onComplete?.Invoke();
            _onComplete = null;
            
            _trackEntry = null;
            _isPlaying = false;
        }

        public void Cleanup()
        {
            Unsubscribe();
            _onComplete = null;
            _trackEntry = null;
            _isPlaying = false;
        }

        public float GetCurrentTrackDuration()
        {
            if(_trackEntry?.Animation == null) return 0;
            return _trackEntry.Animation.Duration;
        }
    }
}