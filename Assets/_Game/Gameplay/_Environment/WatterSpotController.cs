using System.Collections.Generic;
using _Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Environment
{
    public class WatterSpotController : MonoBehaviour
    {
        [SerializeField] private WatterSpot _watterSpotPrefab;
        [SerializeField] private RectTransform _fieldTransform;

        private float MinLocalX => _fieldTransform.rect.xMin;
        private float MaxLocalX => _fieldTransform.rect.xMax;
        private float MinLocalY => _fieldTransform.rect.yMin;
        private float MaxLocalY => _fieldTransform.rect.yMax;
        
        [SerializeField]
        [FloatRangeSlider(5, 100f)] 
        private FloatRange _sizeY = new(5, 100f);
        
        [SerializeField]
        [FloatRangeSlider(1, 3f)] 
        private FloatRange _sizeXCoeficient = new(1, 3f);
        
        [SerializeField]
        [FloatRangeSlider(1, 5f)] 
        private FloatRange _lifetime = new(1, 5f);
        
        [SerializeField]
        [FloatRangeSlider(0f, 100f)] 
        private FloatRange _speed = new(0f, 100f);
        
        
        [SerializeField] private int _amount;
        
        [ShowInInspector]
        [SerializeField] private List<WatterSpot> _watterSpots = new();

        [Button("Spawn Watter Spots")]
        private void SpawnWatterSpots()
        {
            foreach (var spot in _watterSpots)
            {
                DestroyImmediate(spot.gameObject);
            }
        
            _watterSpots.Clear();

            for (int i = 0; i < _amount; i++)
            {
                var spot = Instantiate(_watterSpotPrefab, _fieldTransform);
                
                GenerateNewSpotDate(spot);

                spot.SetCurrentLifeTime(Random.Range(0, spot.Lifetime));
                
                _watterSpots.Add(spot);
            }
        }

        private void Update()
        {
            Tick(Time.deltaTime);
        }
        
        public void Tick(float deltaTime)
        {
            foreach (var spot in _watterSpots)
            {
                if (spot.IsActive)
                {
                    spot.Tick(deltaTime);
                }
                else
                {
                    GenerateNewSpotDate(spot);
                }
            }
        }

        private void GenerateNewSpotDate(WatterSpot spot)
        {
            var position = GenerateRandomPosition();  
            spot.Position = position;
            var size = GenerateRandomSize();
            spot.SetSize(size.x, size.y);  
            spot.SetSetLifetime(_lifetime.RandomValueInRange);
            spot.SetSpeed(_speed.RandomValueInRange);
            spot.SetDirection(new Vector3(Random.value > 0.5f ? 1 : -1, 0, 0));
            spot.SetCurrentLifeTime(0);
            spot.SetActive(true);
        }

        private Vector3 GenerateRandomPosition()
        {
            float randomX = Random.Range(MinLocalX, MaxLocalX);
            float randomY = Random.Range(MinLocalY, MaxLocalY); 

            return new Vector3(randomX, randomY, 0);
        }
        
        private Vector2 GenerateRandomSize()
        {
            float randomY = _sizeY.RandomValueInRange;
            
            float randomMultiplier = _sizeXCoeficient.RandomValueInRange;
            
            float width = randomY * randomMultiplier;

            return new Vector2(width, randomY);
        }
    }
}