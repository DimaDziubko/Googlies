using System;
using Zenject;

namespace _Game.UI.ClassicOffers.Scripts
{
    public interface IClassicOfferPresenter : IInitializable, IDisposable
    {
        void ShowPopup();
        void ClosePopup();
    }
}
