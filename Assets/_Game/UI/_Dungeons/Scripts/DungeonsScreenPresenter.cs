using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._GameInitializer;
using _Game.Core.Navigation.Timeline;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Sirenix.OdinInspector;
using UnityUtils;


namespace _Game.UI._Dungeons.Scripts
{
    public class DungeonsScreenPresenter :
        IDungeonsScreenPresenter,
        IDungeonScreen,
        IGameScreenEvents<IDungeonScreen>,
        IGameScreenListener<IMenuScreen>,
        IDisposable
    {
        private const int REQUEST_ATTENTION_LIMIT = 1;
        public event Action<IDungeonScreen> ScreenOpened;
        public event Action<IDungeonScreen> InfoChanged;
        public event Action<IDungeonScreen> RequiresAttention;
        public event Action<IDungeonScreen> ScreenClosed;
        public event Action<IDungeonScreen, bool> ActiveChanged;
        public event Action<IDungeonScreen> ScreenDisposed;

        [ShowInInspector, ReadOnly]
        public bool IsReviewed { get; private set; }

        [ShowInInspector, ReadOnly]
        public bool NeedAttention =>
            _dungeonModelFactory.GetModels().Any(x => x.KeysCount > 0 || x.VideosCount > 0) &&
            _reviewCounter < REQUEST_ATTENTION_LIMIT;
        
        private int _reviewCounter;
        
        public string Info => "Dungeons";

        public DungeonsScreen Screen { get; set; }

        private readonly DungeonPresenter.Factory _factory;
        private readonly IDungeonModelFactory _dungeonModelFactory;
        private readonly IGameInitializer _gameInitializer;

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<IDungeonModel, DungeonPresenter> _presenters = new();

        private readonly List<DungeonView> _fakeViews = new();

        private readonly IUINotifier _uiNotifier;

        public DungeonsScreenPresenter(
            DungeonPresenter.Factory factory,
            IDungeonModelFactory dungeonModelFactory,
            IGameInitializer gameInitializer,
            IUINotifier uiNotifier)
        {
            _factory = factory;
            _dungeonModelFactory = dungeonModelFactory;
            _gameInitializer = gameInitializer;
            _gameInitializer.OnPostInitialization += Init;
            _uiNotifier = uiNotifier;

            uiNotifier.RegisterScreen(this, this);
        }

        private void Init()
        {
            IsReviewed = !NeedAttention;
        }

        void IDungeonsScreenPresenter.OnScreenOpened()
        {
            if (Screen.OrNull() != null)
            {
                UpdateDungeonViews();
                SpawnFakeViews();
                IsReviewed = true;
                ScreenOpened?.Invoke(this);
                _reviewCounter++;
            }
        }

        void IDungeonsScreenPresenter.OnScreenClosed()
        {
            if (Screen.OrNull() != null)
            {
                Cleanup();
                ScreenClosed?.Invoke(this);
            }
        }

        private void Cleanup()
        {
            foreach (var view in _fakeViews) 
                Screen.DungeonListView.DestroyElement(view); 
            _fakeViews.Clear();
            
            foreach (var presenter in _presenters.Values)
            {
                presenter.Dispose();
            }
        }

        void IDungeonsScreenPresenter.OnScreeDispose()
        {
            Cleanup();
            ScreenDisposed?.Invoke(this);
        }

        void IDungeonsScreenPresenter.OnScreeActiveChanged(bool isActive)
        {
            //foreach (var presenter in _presenters.Values) 
            //    presenter.OnScreeActiveChanged(isActive);
            ActiveChanged?.Invoke(this, isActive);
        }

        void IDisposable.Dispose()
        {
            _presenters.Clear();
            _uiNotifier.UnregisterScreen(this);
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void UpdateDungeonViews()
        {
            foreach (IDungeonModel dungeonModel in _dungeonModelFactory.GetModels())
            {
                if (dungeonModel != null)
                {
                    if (_presenters.TryGetValue(dungeonModel, out var presenter))
                    {
                        presenter.SetModel(dungeonModel);

                        if (presenter.View == null)
                        {
                            var view = Screen.DungeonListView.SpawnElement();
                            presenter.SetView(view);
                        }
                    }
                    else
                    {
                        var view = Screen.DungeonListView.SpawnElement();
                        presenter = _factory.Create(dungeonModel, view);
                        _presenters.Add(dungeonModel, presenter);
                    }

                    presenter.Initialize();
                }
            }
        }

        private void SpawnFakeViews()
        {
            for (int i = 0; i < 2; i++)
            {
                var fakeView = Screen.DungeonListView.SpawnElement();
                fakeView.SetLocked(true);
                _fakeViews.Add(fakeView);
            }
        }

        [Button]
        void IGameScreenListener<IMenuScreen>.OnScreenOpened(IMenuScreen screen)
        {
            if (NeedAttention)
            {
                IsReviewed = false;
                RequiresAttention?.Invoke(this);
            }
        }

        void IGameScreenListener<IMenuScreen>.OnInfoChanged(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnRequiresAttention(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenClosed(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenActiveChanged(IMenuScreen screen, bool isActive) { }
        void IGameScreenListener<IMenuScreen>.OnScreenDisposed(IMenuScreen screen) { }
    }
}