using System;
using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._DailyTasks.Scripts.Strategies;
using _Game.Gameplay._GameEventRouter;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._Hud;
using _Game.UI._Hud._DailyTaskView;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public class DailyTaskStrategyFactory
    {
        private readonly DailyTaskView _view;
        private readonly IConfigRepository _config;
        private readonly IUserContainer _userContainer;
        private readonly FoodGenerator _foodGenerator;
        private readonly IBattleNavigator _battleNavigator;
        private readonly IAgeNavigator _ageNavigator;
        private readonly BattleField _battleField;
        private readonly DailyTaskPresenter.Factory _factory;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly TemporaryCurrencyBank _temporaryBank;
        private readonly GameEventRouter _router;

        public DailyTaskStrategyFactory(
            IUserContainer userContainer,
            BattleHud battleHud,
            IConfigRepository config,
            FoodGenerator foodGenerator,
            TemporaryCurrencyBank temporaryBank,
            IBattleNavigator battleNavigator,
            IAgeNavigator ageNavigator,
            BattleField battleField,
            DailyTaskPresenter.Factory factory,
            ITimelineNavigator timelineNavigator,
            GameEventRouter router)
        {
            _router = router;
            _view = battleHud.DailyTaskView;
            _config = config;
            _userContainer = userContainer;
            _foodGenerator = foodGenerator;
            _temporaryBank = temporaryBank;
            _battleNavigator = battleNavigator;
            _battleField = battleField;
            _ageNavigator = ageNavigator;
            _factory = factory;
            _timelineNavigator = timelineNavigator;
        }

        public IDailyTaskStrategy GetStrategy(DailyTaskModel model)
        {
            return model.Type switch
            {
                DailyTaskType.AdsWatch => new AdsWatchStrategy(model, _view, _userContainer, _timelineNavigator, _factory),
                DailyTaskType.EarnCoins => new EarnCoinsStrategy(model, _view, _userContainer, _battleNavigator, _timelineNavigator, _temporaryBank, _ageNavigator, _factory),
                DailyTaskType.ProduceFood => new ProduceFoodStrategy(model, _view, _foodGenerator, _ageNavigator, _timelineNavigator, _userContainer, _factory),
                
                DailyTaskType.DefeatEnemy => new DefeatEnemyStrategy(model, _view, _ageNavigator, _timelineNavigator, _battleField, _userContainer, _factory, _router),
                DailyTaskType.DefeatLightEnemy => new DefeatLightEnemyStrategy(model, _view, _config, _battleNavigator, _ageNavigator, _timelineNavigator, _battleField, _userContainer, _factory, _router),
                DailyTaskType.DefeatMediumEnemy => new DefeatMediumEnemyStrategy(model, _view, _config, _battleNavigator, _ageNavigator, _timelineNavigator, _battleField, _userContainer, _factory, _router),
                DailyTaskType.DefeatHeavyEnemy => new DefeatHeavyEnemyStrategy(model, _view, _config, _battleNavigator, _ageNavigator, _timelineNavigator, _battleField, _userContainer, _factory, _router),
                
                DailyTaskType.SpawnUnit => new SpawnUnitStrategy(model, _view, _ageNavigator, _timelineNavigator, _battleField, _userContainer, _factory),
                DailyTaskType.SpawnLightUnit => new SpawnLightUnitStrategy(model, _view, _config, _ageNavigator, _timelineNavigator, _battleField, _userContainer, _factory),
                DailyTaskType.SpawnMediumUnit => new SpawnMediumUnitStrategy(model, _view, _config, _ageNavigator, _timelineNavigator, _battleField, _userContainer, _factory),
                DailyTaskType.SpawnHeavyUnit => new SpawnHeavyUnitStrategy(model, _view, _config, _ageNavigator, _timelineNavigator, _battleField, _userContainer, _factory),

                _ => throw new ArgumentOutOfRangeException($"Unsupported Type: {model.Type}")
            };
        }
    }
}