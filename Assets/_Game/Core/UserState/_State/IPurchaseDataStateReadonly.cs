using System;
using System.Collections.Generic;
using _Game.Core.Services.IAP;

namespace _Game.Core.UserState._State
{
    public interface IPurchaseDataStateReadonly
    {
        event Action Changed;
        List<BoughtIAP> BoughtIAPs { get; }
        bool Restored { get; }
        List<PendingIAP> PendingIAPs { get; }
    }
}