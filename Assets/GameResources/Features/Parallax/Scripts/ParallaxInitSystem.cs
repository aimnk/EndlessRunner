namespace Game.Features.Parallax
{
    using DG.Tweening;
    using AssetProvider;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using UnityEngine;
    using Configs;
    using Extensions;
    using DG.Tweening.Core;
    using DG.Tweening.Plugins.Options;
    using States;
    
    /// <summary>
    /// Система параллакс эффекта для фона
    /// </summary>
    public class ParallaxInitSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsWorldInject _world = default;
        
        private readonly EcsCustomInject<IAssetProvider> _assetProvider;
        
        private readonly EcsCustomInject<GameConfig> _gameConfig;
        
        private EcsCustomInject<IStateMachine> _stateMachine;
        
        private EcsFilterInject<Inc<LevelComponent>> _filter;
        
        private ParallaxData _parallaxData;

        private EcsPool<LevelComponent> _levelComponentPool;

        private Transform _transformParallax;

        private SpriteRenderer _renderer;

        private GameObject _parallaxGameObject;

        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenerCore;
        
        public void Init(IEcsSystems systems)
        {
            var entity = _world.Value.NewEntity();
            _world.Value.AddComponent<LevelComponent>(entity);

            _levelComponentPool = _world.Value.GetPool<LevelComponent>();
             
            ParallaxData parallaxInitSystem = _assetProvider.Value.LoadAsset<ParallaxData>(AssetsDataPath.PARALLAX_DATA_PATH);
            
            _parallaxGameObject =
                Object.Instantiate(parallaxInitSystem.PrefabParallax, Vector3.zero, Quaternion.identity);

            _transformParallax = _parallaxGameObject.transform;
            
            _renderer = _parallaxGameObject.GetComponent<SpriteRenderer>();
        }

        private void SetCurrentHeight()
        { 
            foreach (var entity in _filter.Value)
            {
                ref float currentHeight = ref _levelComponentPool.Get(entity).CurrentHeight;
                currentHeight = _renderer.size.y/2 -_transformParallax.transform.position.y;
            }
        }

        public void Run(IEcsSystems systems)
        {
            if (_stateMachine.Value.CurrentState() != State.Game)
            {
                _parallaxGameObject.transform.position = new Vector3(0, _renderer.size.y/2, 0);
                _tweenerCore.Kill();
                return;
            }

            if (_tweenerCore != null)
            {
                return;
            }
            
            _tweenerCore = _parallaxGameObject.transform.DOMoveY(-_renderer.size.y / 2,
                    _renderer.size.y / _gameConfig.Value.DifficultData.SpeedBalloon)
                .OnUpdate(SetCurrentHeight)
                .SetEase(Ease.Linear);
        }

        public void Destroy(IEcsSystems systems) 
            => _tweenerCore.Kill();
    }
}
