using System;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class IAPRestoreProcessor
    {
        private readonly IAPProvider _iapProvider;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;

        public IAPRestoreProcessor(
            IAPProvider iapProvider,
            IMyLogger logger,
            IUserContainer userContainer)
        {
            _iapProvider = iapProvider;
            _logger = logger;
            _userContainer = userContainer;
        }
        
        public void RestorePurchasesAndroid(Action onRestoreComplete = null)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return;
            }

            if (!_iapProvider.IsInitialized)
            {
                _logger.LogError("IAP provider is not initialized.");
                return;
            }

            IGooglePlayStoreExtensions googlePlayExtensions = _iapProvider.Extensions.GetExtension<IGooglePlayStoreExtensions>();

            if (googlePlayExtensions != null)
            {
                googlePlayExtensions.RestoreTransactions((result) =>
                {
                    if (result)
                    {
                        _logger.Log("Restore transactions completed successfully.");
                        
                        onRestoreComplete?.Invoke();
                    }
                    else
                    {
                        _logger.LogWarning("Restore transactions failed.");
                    }
                });
            }
            else
            {
                _logger.LogError("Google Play Store Extension is not available.");
            }
        }

        public void RestorePurchasesIOS(Action onRestoreComplete = null)
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.OSXPlayer)
            {
                return;
            }

            if (!_iapProvider.IsInitialized)
            {
                _logger.LogError("IAP provider is not initialized.");
                return;
            }


            IAppleExtensions apple = _iapProvider.Extensions.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) =>
            {
                if (result)
                {
                    onRestoreComplete?.Invoke();

                    _logger.Log("Restore success.", DebugStatus.Success);
                }
                else
                {
                    _logger.Log("Restore failed.", DebugStatus.Warning);
                }
            });
        }
        
        //TODO: Delete later
        private void OnIOSRestoreComplete()
        {
            _userContainer.State.PurchaseDataState.Restore();
        }
    }
}