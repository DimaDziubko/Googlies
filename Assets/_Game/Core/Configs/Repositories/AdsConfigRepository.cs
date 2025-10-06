using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._AdsConfig;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories
{
    public interface IAdsConfigRepository
    {
        AdsConfig GetConfig();
    }
    public class AdsConfigRepository : IAdsConfigRepository
    {
        private readonly IUserContainer _userContainer;

        public AdsConfigRepository(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public AdsConfig GetConfig() => 
            _userContainer.GameConfig.AdsConfig;
    }
}