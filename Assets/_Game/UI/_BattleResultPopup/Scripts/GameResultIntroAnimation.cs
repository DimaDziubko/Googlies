using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace _Game.UI.BattleResultPopup.Scripts
{
    [RequireComponent(typeof(PlayableDirector))]
    public class GameResultIntroAnimation : MonoBehaviour
    {
        [SerializeField] private List<GameResultSetting> _settings;
        
        [SerializeField] private PlayableDirector _director;
        
        private UniTaskCompletionSource<bool> _playAwaiter;
        public async UniTask Play(GameResultType result)
        {
            
            
            _playAwaiter = new UniTaskCompletionSource<bool>();
            _director.stopped -= OnTimelineFinished;
            _director.stopped += OnTimelineFinished;
            
            _director.Play();
            await _playAwaiter.Task;
        }

        private void OnTimelineFinished(PlayableDirector _)
        {
            _playAwaiter.TrySetResult(true);
        }
        
        [Serializable]
        private class GameResultSetting
        {
            public GameResultType Type;
            //public GameObject Object;
        }
    }
    
}