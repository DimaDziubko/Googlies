using System.Collections.Generic;
using System.Linq;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core._GameListenerComposite
{
    public class UIListenerComposite :
        MonoBehaviour,
        IGameScreenListener<IGameScreenWithInfo>,
        IGameScreenListener<IStartBattleScreen>,
        IGameScreenListener<IShopScreen>,
        IGameScreenListener<IUpgradesScreen>,
        IGameScreenListener<ICardsScreen>,
        IGameScreenListener<ISkillsScreen>,
        IGameScreenListener<IGeneralCardsScreen>,
        IGameScreenListener<ITravelScreen>,
        IGameScreenListener<IMenuScreen>,
        IGameScreenListener<IDungeonScreen>,
        IGameScreenListener<IEvolveScreen>
    {
        [ShowInInspector] [Inject] private IUINotifier _uiNotifier;

        [ShowInInspector] [Inject] private List<IGameScreenListener> _listeners;

        private void Start() => _uiNotifier.Register(this);
        private void OnDestroy() => _uiNotifier.Unregister(this);

        
        private void NotifyDisposed<TScreen>(TScreen screen) where TScreen : IGameScreen
        {
            var listeners = _listeners.OfType<IGameScreenListener<TScreen>>();
            foreach (var listener in listeners)
            {
                listener.OnScreenDisposed(screen);
            }
        }
        
        private void NotifyActiveChanged<TScreen>(TScreen screen, bool isActive) where TScreen : IGameScreen
        {
            var listeners = _listeners.OfType<IGameScreenListener<TScreen>>();
            foreach (var listener in listeners)
            {
                listener.OnScreenActiveChanged(screen, isActive);
            }
        }
        
        private void NotifyListeners<TScreen>(TScreen screen, bool isOpened) where TScreen : IGameScreen
        {
            var listeners = _listeners.OfType<IGameScreenListener<TScreen>>();

            foreach (var listener in listeners)
            {
                if (isOpened)
                    listener.OnScreenOpened(screen);
                else
                    listener.OnScreenClosed(screen);
            }
        }
        
        private void NotifyInfoChanged<TScreen>(TScreen screen) where TScreen : IGameScreen
        {
            var listeners = _listeners.OfType<IGameScreenListener<TScreen>>();

            foreach (var listener in listeners)
            {
                listener.OnInfoChanged(screen);
            }
        }
        
        private void NotifyRequiresAttention<TScreen>(TScreen screen) where TScreen : IGameScreen
        {
            var listeners = _listeners.OfType<IGameScreenListener<TScreen>>();

            foreach (var listener in listeners)
            {
                listener.OnRequiresAttention(screen);
            }
        }

        void IGameScreenListener<IGameScreenWithInfo>.OnScreenOpened(IGameScreenWithInfo screen) => NotifyListeners(screen, true);
        void IGameScreenListener<IGameScreenWithInfo>.OnInfoChanged(IGameScreenWithInfo screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<IGameScreenWithInfo>.OnRequiresAttention(IGameScreenWithInfo screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<IGameScreenWithInfo>.OnScreenClosed(IGameScreenWithInfo screen) => NotifyListeners(screen, false);
        void IGameScreenListener<IGameScreenWithInfo>.OnScreenActiveChanged(IGameScreenWithInfo screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<IGameScreenWithInfo>.OnScreenDisposed(IGameScreenWithInfo screen) => NotifyDisposed(screen);


        void IGameScreenListener<IStartBattleScreen>.OnScreenOpened(IStartBattleScreen screen) => NotifyListeners(screen, true);
        void IGameScreenListener<IStartBattleScreen>.OnInfoChanged(IStartBattleScreen screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<IStartBattleScreen>.OnRequiresAttention(IStartBattleScreen screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<IStartBattleScreen>.OnScreenClosed(IStartBattleScreen screen) => NotifyListeners(screen, false);
        void IGameScreenListener<IStartBattleScreen>.OnScreenActiveChanged(IStartBattleScreen screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<IStartBattleScreen>.OnScreenDisposed(IStartBattleScreen screen) => NotifyDisposed(screen);
        
 
        void IGameScreenListener<IShopScreen>.OnScreenOpened(IShopScreen screen) => NotifyListeners(screen, true);
        void IGameScreenListener<IShopScreen>.OnInfoChanged(IShopScreen screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<IShopScreen>.OnRequiresAttention(IShopScreen screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<IShopScreen>.OnScreenClosed(IShopScreen screen) => NotifyListeners(screen, false);
        void IGameScreenListener<IShopScreen>.OnScreenActiveChanged(IShopScreen screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<IShopScreen>.OnScreenDisposed(IShopScreen screen) => NotifyDisposed(screen);
        

        void IGameScreenListener<IUpgradesScreen>.OnScreenOpened(IUpgradesScreen screen) => NotifyListeners(screen, true);
        void IGameScreenListener<IUpgradesScreen>.OnInfoChanged(IUpgradesScreen screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<IUpgradesScreen>.OnRequiresAttention(IUpgradesScreen screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<IUpgradesScreen>.OnScreenClosed(IUpgradesScreen screen) => NotifyListeners(screen, false);
        void IGameScreenListener<IUpgradesScreen>.OnScreenActiveChanged(IUpgradesScreen screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<IUpgradesScreen>.OnScreenDisposed(IUpgradesScreen screen) => NotifyDisposed(screen);


        void IGameScreenListener<ICardsScreen>.OnScreenOpened(ICardsScreen screen) => NotifyListeners(screen, true);
        void IGameScreenListener<ICardsScreen>.OnInfoChanged(ICardsScreen screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<ICardsScreen>.OnRequiresAttention(ICardsScreen screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<ICardsScreen>.OnScreenClosed(ICardsScreen screen) => NotifyListeners(screen, false);
        void IGameScreenListener<ICardsScreen>.OnScreenActiveChanged(ICardsScreen screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<ICardsScreen>.OnScreenDisposed(ICardsScreen screen) => NotifyDisposed(screen);
   

        void IGameScreenListener<IGeneralCardsScreen>.OnScreenOpened(IGeneralCardsScreen screen) => NotifyListeners(screen, true);
        void IGameScreenListener<IGeneralCardsScreen>.OnInfoChanged(IGeneralCardsScreen screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<IGeneralCardsScreen>.OnRequiresAttention(IGeneralCardsScreen screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<IGeneralCardsScreen>.OnScreenClosed(IGeneralCardsScreen screen) => NotifyListeners(screen, false);
        void IGameScreenListener<IGeneralCardsScreen>.OnScreenActiveChanged(IGeneralCardsScreen screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<IGeneralCardsScreen>.OnScreenDisposed(IGeneralCardsScreen screen) => NotifyDisposed(screen);
   

        void IGameScreenListener<ITravelScreen>.OnScreenOpened(ITravelScreen screen) => NotifyListeners(screen, true);
        void IGameScreenListener<ITravelScreen>.OnInfoChanged(ITravelScreen screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<ITravelScreen>.OnRequiresAttention(ITravelScreen screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<ITravelScreen>.OnScreenClosed(ITravelScreen screen) => NotifyListeners(screen, false);
        void IGameScreenListener<ITravelScreen>.OnScreenActiveChanged(ITravelScreen screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<ITravelScreen>.OnScreenDisposed(ITravelScreen screen) => NotifyDisposed(screen);
 

        void IGameScreenListener<IMenuScreen>.OnScreenOpened(IMenuScreen screen) => NotifyListeners(screen, true);
        void IGameScreenListener<IMenuScreen>.OnInfoChanged(IMenuScreen screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<IMenuScreen>.OnRequiresAttention(IMenuScreen screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<IMenuScreen>.OnScreenClosed(IMenuScreen screen) => NotifyListeners(screen, false);
        void IGameScreenListener<IMenuScreen>.OnScreenActiveChanged(IMenuScreen screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<IMenuScreen>.OnScreenDisposed(IMenuScreen screen) => NotifyDisposed(screen);

        
        void IGameScreenListener<ISkillsScreen>.OnScreenOpened(ISkillsScreen screen) => NotifyListeners(screen, true);
        void IGameScreenListener<ISkillsScreen>.OnInfoChanged(ISkillsScreen screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<ISkillsScreen>.OnRequiresAttention(ISkillsScreen screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<ISkillsScreen>.OnScreenClosed(ISkillsScreen screen) => NotifyListeners(screen, false);
        void IGameScreenListener<ISkillsScreen>.OnScreenActiveChanged(ISkillsScreen screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<ISkillsScreen>.OnScreenDisposed(ISkillsScreen screen) => NotifyDisposed(screen);

        void IGameScreenListener<IDungeonScreen>.OnScreenOpened(IDungeonScreen screen) => NotifyListeners(screen, true);
        void IGameScreenListener<IDungeonScreen>.OnInfoChanged(IDungeonScreen screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<IDungeonScreen>.OnRequiresAttention(IDungeonScreen screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<IDungeonScreen>.OnScreenClosed(IDungeonScreen screen) => NotifyListeners(screen, false);
        void IGameScreenListener<IDungeonScreen>.OnScreenActiveChanged(IDungeonScreen screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<IDungeonScreen>.OnScreenDisposed(IDungeonScreen screen) => NotifyDisposed(screen);


        void IGameScreenListener<IEvolveScreen>.OnScreenOpened(IEvolveScreen screen) => NotifyListeners(screen, true);
        void IGameScreenListener<IEvolveScreen>.OnInfoChanged(IEvolveScreen screen) => NotifyInfoChanged(screen);
        void IGameScreenListener<IEvolveScreen>.OnRequiresAttention(IEvolveScreen screen) => NotifyRequiresAttention(screen);
        void IGameScreenListener<IEvolveScreen>.OnScreenClosed(IEvolveScreen screen) => NotifyListeners(screen, false);
        void IGameScreenListener<IEvolveScreen>.OnScreenActiveChanged(IEvolveScreen screen, bool isActive) => NotifyActiveChanged(screen, isActive);
        void IGameScreenListener<IEvolveScreen>.OnScreenDisposed(IEvolveScreen screen) => NotifyDisposed(screen);

    }
}