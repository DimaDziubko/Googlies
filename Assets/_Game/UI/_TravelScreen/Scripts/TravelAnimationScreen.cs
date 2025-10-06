using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace _Game.UI._TravelScreen.Scripts
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TravelAnimationScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        public void Construct(Camera camera)
        {
            _canvas.worldCamera = camera;
        }

        [SerializeField] private PlayableDirector _director;

        private UniTaskCompletionSource<bool> _playAwaiter;

        public async UniTask<bool> Play()
        {
            _playAwaiter = new UniTaskCompletionSource<bool>();
            _director.stopped -= OnTimelineFinished;
            _director.stopped += OnTimelineFinished;
            
            _director.Play();
            return await _playAwaiter.Task;
        }

        private void OnTimelineFinished(PlayableDirector _) => _playAwaiter.TrySetResult(true);
    }
}
