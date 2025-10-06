using System;
using System.Collections.Generic;
using _Game.Core.Services.IAP;

namespace _Game.Core.UserState._State
{
    public class PurchaseDataState : IPurchaseDataStateReadonly
    {
        public List<BoughtIAP> BoudhtIAPs = new();
        public List<PendingIAP> PendingIAPs = new();
        public bool Restored;

        public event Action Changed;

        List<BoughtIAP> IPurchaseDataStateReadonly.BoughtIAPs => BoudhtIAPs;
        List<PendingIAP> IPurchaseDataStateReadonly.PendingIAPs => PendingIAPs;
        bool IPurchaseDataStateReadonly.Restored => Restored;

        public PurchaseDataState()
        {
            BoudhtIAPs = new();
            PendingIAPs = new();
        }

        public void AddPurchase(string id)
        {
            BoughtIAP boughtIap = Product(id);

            if (boughtIap != null)
            {
                boughtIap.Count++;
            }
            else
            {
                BoudhtIAPs.Add(new BoughtIAP { IAPId = id, Count = 1 });
            }

            Changed?.Invoke();
        }

        private BoughtIAP Product(string id) =>
            BoudhtIAPs.Find(x => x.IAPId == id);

        public void Restore() => Restored = true;

        public void AddPending(string iapId)
        {
            PendingIAPs.Add(new PendingIAP
            {
                IAPId = iapId,
            });

            if (PendingIAPs.Count > 5)
            {
                int countToRemove = PendingIAPs.Count - 5;
                PendingIAPs.RemoveRange(0, countToRemove);
            }

            Changed?.Invoke();
        }

        public void RemovePending(string iapId)
        {
            var item = PendingIAPs.Find(x => x.IAPId == iapId);
            if (item != null)
            {
                PendingIAPs.Remove(item);
                Changed?.Invoke();
            }
        }
    }
    
    public class PendingIAP
    {
        public string IAPId;
    }
}