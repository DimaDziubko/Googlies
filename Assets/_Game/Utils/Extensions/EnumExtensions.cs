using System;
using System.Text.RegularExpressions;
using _Game.Core.Boosts;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay.Vfx.Factory;
using _Game.UI._ParticleAttractorSystem;

namespace _Game.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static AttractableParticleType ToAttractableVfxRenderedOverlay(this CurrencyType type)
        {
            return type switch
            {
                CurrencyType.Coins => AttractableParticleType.CoinsOverlay,
                CurrencyType.Gems => AttractableParticleType.GemsOverlay,
                CurrencyType.SkillPotion => AttractableParticleType.CardTabOverlay,
                _ => AttractableParticleType.CoinsOverlay
            };
        }

        public static AttractableParticleType ToAttractableVfxRenderedCamera(this CurrencyType type)
        {
            return type switch
            {
                CurrencyType.Coins => AttractableParticleType.CoinsCamera,
                CurrencyType.Gems => AttractableParticleType.GemsCamera,
                CurrencyType.SkillPotion => AttractableParticleType.CardTabCamera,
                _ => AttractableParticleType.CoinsCamera
            };
        }

        public static string ToName(this BoostSource boostSource) => 
            $"{boostSource} Boosts";

        public static string ToName(this BoostType boostSource)
        {
            return Regex.Replace(boostSource.ToString(), "([a-z])([A-Z])", "$1 $2");
        }
        
        public static string ToSpineSkin(this Skin skin)
        {
            return skin switch
            {
                Skin.Ally => "Ally",
                Skin.Hostile => "Hostile",
                Skin.Zombie => "Zombie",
                
                Skin.GreenGhost => "GreenGhost",
                Skin.BlueGhost => "BlueGhost",
                Skin.MagentaGhost => "MagentaGhost",
                
                Skin.None => "Ally",
                _ => throw new ArgumentOutOfRangeException($"Unsupported Skin: {skin}")
            };
        }
    }
}