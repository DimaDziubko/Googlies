using System;

namespace _Game.Core._FeatureUnlockSystem.Scripts
{
    public interface IFeatureUnlockSystem
    {
        bool IsFeatureUnlocked(IFeature feature);
        bool IsFeatureUnlocked(Feature feature);
        event Action<Feature> FeatureUnlocked;
    }
}