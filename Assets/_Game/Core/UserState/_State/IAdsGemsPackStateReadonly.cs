using System;

namespace _Game.Core.UserState._State
{
    public interface IAdsGemsPackStateReadonly
    {
        DateTime LastAdsGemPackDay { get; }
        int AdsGemPackCount { get; }
    }
}