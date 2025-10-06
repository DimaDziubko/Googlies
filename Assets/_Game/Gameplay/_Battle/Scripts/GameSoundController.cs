using _Game.Core._GameListenerComposite;
using _Game.Core.Services.Audio;

namespace _Game.Gameplay._Battle.Scripts
{
    public class GameSoundController : IStartGameListener
    {
        private readonly IAudioService _audioService;

        public GameSoundController(IAudioService audioService)
        {
            _audioService = audioService;
        }


        void IStartGameListener.OnStartBattle() => 
            _audioService.PlayStartBattleSound();
    }
}