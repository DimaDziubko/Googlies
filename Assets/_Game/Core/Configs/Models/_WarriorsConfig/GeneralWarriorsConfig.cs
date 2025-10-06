using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _Game.Core.Configs.Models._WarriorsConfig
{
    [CreateAssetMenu(fileName = "GeneralWarriorsConfig", menuName = "Configs/Warriors")]
    public class GeneralWarriorsConfig : ScriptableObject
    {
        public List<WarriorConfig> WarriorConfigs;

        [Button("Validate Unique IDs")]
        public void ValidateUniqueIds()
        {
            HashSet<int> uniqueIds = new HashSet<int>();

            foreach (var warrior in WarriorConfigs)
            {
                if (!uniqueIds.Add(warrior.Id))
                {
                    Debug.LogError($"Duplicate ID found: {warrior.Id}");
                }
            }
        }

        [Header("Utils")] public bool SameForZombie;
        public int SearchId;
        public Skin Skin;
        public int Timeline;
        public string PathPart1 = "Assets/_Game/Bundles/Timeline_";
        public string PathPart2 = "/Units/Prefabs/";
        public string PathPart2ForT3 = "/Units/";

        [Header("Batch Extract Settings")] public int StartId = 1;
        public int EndId = 1;

        [Button("Extract Range From Prefabs")]
        public void ExtractFromPrefabsByRange()
        {
#if UNITY_EDITOR
            for (int id = StartId; id <= EndId; id++)
            {
                ExtractSinglePrefab(id);
            }
#endif
        }

        [Button]
        public void SetDefaultScales()
        {
            foreach (var warrior in WarriorConfigs)
            {
                warrior.WarriorPositionSettings.HealthBarSettings.Scale = Vector3.one;
                warrior.WarriorPositionSettings.DamageTextPointSettings.Scale = Vector3.one;
                warrior.WarriorPositionSettings.SkillEffectParentSettings.Scale = Vector3.one;

                warrior.ZombiePositionSettings.HealthBarSettings.Scale = Vector3.one;
                warrior.ZombiePositionSettings.DamageTextPointSettings.Scale = Vector3.one;
                warrior.ZombiePositionSettings.SkillEffectParentSettings.Scale = Vector3.one;
            }
        }

        [Button]
        public void SetHealthBarPosition(Vector3 position)
        {
            var config = WarriorConfigs.Find(warrior => warrior.Id == SearchId);
            if (config != null)
            {
                config.WarriorPositionSettings.HealthBarSettings.Position = position;

                if (SameForZombie)
                {
                    SetZombieHealthBarPosition(position);
                }
            }
        }

        [Button]
        public void SetDamageTestPosition(Vector3 position)
        {
            var config = WarriorConfigs.Find(warrior => warrior.Id == SearchId);
            if (config != null)
            {
                config.WarriorPositionSettings.DamageTextPointSettings.Position = position;

                if (SameForZombie)
                {
                    SetZombieDamageTestPosition(position);
                }
            }
        }

        [Button]
        public void SetSkillEffectParentPosition(Vector3 position)
        {
            var config = WarriorConfigs.Find(warrior => warrior.Id == SearchId);
            if (config != null)
            {
                config.WarriorPositionSettings.SkillEffectParentSettings.Position = position;

                if (SameForZombie)
                {
                    SetZombieSkillEffectParentPosition(position);
                }
            }
        }

        [Button]
        public void SetSkillEffectParentScale(Vector3 scale)
        {
            var config = WarriorConfigs.Find(warrior => warrior.Id == SearchId);
            if (config != null)
            {
                config.WarriorPositionSettings.SkillEffectParentSettings.Scale = scale;
                if (SameForZombie)
                {
                    SetZombieSkillEffectParentScale(scale);
                }
            }
        }


        [Button]
        public void SetZombieHealthBarPosition(Vector3 position)
        {
            var config = WarriorConfigs.Find(warrior => warrior.Id == SearchId);
            if (config != null)
            {
                config.ZombiePositionSettings.HealthBarSettings.Position = position;
            }
        }

        [Button]
        public void SetZombieDamageTestPosition(Vector3 position)
        {
            var config = WarriorConfigs.Find(warrior => warrior.Id == SearchId);
            if (config != null)
            {
                config.ZombiePositionSettings.DamageTextPointSettings.Position = position;
            }
        }


        [Button]
        public void SetZombieSkillEffectParentPosition(Vector3 position)
        {
            var config = WarriorConfigs.Find(warrior => warrior.Id == SearchId);
            if (config != null)
            {
                config.ZombiePositionSettings.SkillEffectParentSettings.Position = position;
            }
        }


        [Button]
        public void SetZombieSkillEffectParentScale(Vector3 scale)
        {
            var config = WarriorConfigs.Find(warrior => warrior.Id == SearchId);
            if (config != null)
            {
                config.ZombiePositionSettings.SkillEffectParentSettings.Scale = scale;
            }
        }

#if UNITY_EDITOR
        private void ExtractSinglePrefab(int id)
        {
            string assetPath = "";
            string pathPart3 = "";
            string pathPart4 = ".prefab";

            int warriorIndex = id - 1;

            switch (Skin)
            {
                case Skin.Ally:
                    pathPart3 = $"Unit_{warriorIndex}";
                    break;
                case Skin.Hostile:
                    pathPart3 = $"Enemy_{warriorIndex}";
                    break;
                case Skin.Zombie:
                    pathPart3 = $"Zombie_{warriorIndex}";
                    break;
                default:
                    Debug.LogWarning($"ID {id}: Невірний тип Skin: {Skin}");
                    return;
            }

            if (Timeline == 3)
            {
                assetPath = string.Format("{0}{1}{2}{3}/{3}{4}", PathPart1, Timeline, PathPart2ForT3, pathPart3,
                    pathPart4);
            }
            else
            {
                assetPath = $"{PathPart1}{Timeline}{PathPart2}{pathPart3}{pathPart4}";
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null)
            {
                Debug.LogWarning($"ID {id}: Префаб не знайдено за шляхом: {assetPath}");
                return;
            }

            Transform pivot = prefab.transform.Find("Pivot");
            if (pivot == null)
            {
                Debug.LogWarning($"ID {id}: Pivot не знайдено в префабі.");
                return;
            }

            Transform health = pivot.Find("HealthBar");
            Transform damage = pivot.Find("DamageTextPoint");
            Transform skill = pivot.Find("SkillEffectParent");

            if (health == null || damage == null || skill == null)
            {
                Debug.LogWarning(
                    $"ID {id}: Один із дочірніх елементів не знайдено (HealthBar, DamageTextPoint, SkillEffectParent).");
                return;
            }

            var config = WarriorConfigs.Find(w => w.Id == id);
            if (config == null)
            {
                Debug.LogWarning($"ID {id}: WarriorConfig не знайдено.");
                return;
            }

            if (Skin != Skin.Zombie)
            {
                config.WarriorPositionSettings.HealthBarSettings = new ObjectPositionSettings
                {
                    Position = health.localPosition,
                    Rotation = health.localEulerAngles,
                    Scale = health.localScale
                };

                config.WarriorPositionSettings.DamageTextPointSettings = new ObjectPositionSettings
                {
                    Position = damage.localPosition,
                    Rotation = damage.localEulerAngles,
                    Scale = damage.localScale
                };
                config.WarriorPositionSettings.SkillEffectParentSettings = new ObjectPositionSettings
                {
                    Position = skill.localPosition,
                    Rotation = skill.localEulerAngles,
                    Scale = skill.localScale
                };
            }

            if (SameForZombie || Skin == Skin.Zombie)
            {
                config.ZombiePositionSettings.HealthBarSettings = new ObjectPositionSettings
                {
                    Position = health.localPosition,
                    Rotation = health.localEulerAngles,
                    Scale = health.localScale
                };

                config.ZombiePositionSettings.DamageTextPointSettings = new ObjectPositionSettings
                {
                    Position = damage.localPosition,
                    Rotation = damage.localEulerAngles,
                    Scale = damage.localScale
                };
                config.ZombiePositionSettings.SkillEffectParentSettings = new ObjectPositionSettings
                {
                    Position = skill.localPosition,
                    Rotation = skill.localEulerAngles,
                    Scale = skill.localScale
                };
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
#endif

            Debug.Log($"ID {id}: Дані з префабу {prefab.name} зчитані успішно.");
        }
#endif
    }
}