namespace Game.Features.Parallax
{
    using DG.Tweening;
    using AssetProvider;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using UnityEngine;
    using Configs;
    using Extensions;
    
    /// <summary>
    /// Система параллакс эффекта для фона
    /// </summary>
    public class ParallaxInitSystem : IEcsInitSystem
    {
        private const string PARALLAX_DATA_PATH = "ParallaxData";
        
        private readonly EcsWorldInject _world = default;
        
        private readonly EcsCustomInject<IAssetProvider> _assetProvider;
        
        private readonly EcsCustomInject<GameConfig> _gameConfig;
        
        private EcsFilterInject<Inc<LevelComponent>> _filter;
        
        private ParallaxData _parallaxData;

        private EcsPool<LevelComponent> _levelComponentPool;

        private Transform _transformParallax;

        private SpriteRenderer _renderer;
        
        public void Init(IEcsSystems systems)
        {
            var entity = _world.Value.NewEntity();
            _world.Value.AddComponent<LevelComponent>(entity);

            _levelComponentPool = _world.Value.GetPool<LevelComponent>();
             
            ParallaxData parallaxInitSystem = _assetProvider.Value.LoadAsset<ParallaxData>(PARALLAX_DATA_PATH);
            
            var parallaxGameObject =
                Object.Instantiate(parallaxInitSystem.PrefabParallax, Vector3.zero, Quaternion.identity);

            _transformParallax = parallaxGameObject.transform;
            
            _renderer = parallaxGameObject.GetComponent<SpriteRenderer>();

            parallaxGameObject.transform.position = new Vector3(0, _renderer.size.y/2, 0);

            parallaxGameObject.transform.DOMoveY(-_renderer.size.y / 2,
                    _renderer.size.y / _gameConfig.Value.DifficultData.SpeedBalloon)
                .OnUpdate(SetCurrentHeight)
                .SetEase(Ease.Linear)
                .SetAutoKill();
        }

        private void SetCurrentHeight()
        { 
            foreach (var entity in _filter.Value)
            {
                ref float currentHeight = ref _levelComponentPool.Get(entity).CurrentHeight;
                currentHeight = _renderer.size.y/2 -_transformParallax.transform.position.y;
            }
        }
    }
}
