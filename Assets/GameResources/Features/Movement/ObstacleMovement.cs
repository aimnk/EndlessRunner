namespace Game.Features.Obstacles
{
    using System.Collections.Generic;
    using DG.Tweening;
    using DG.Tweening.Core;
    using DG.Tweening.Plugins.Options;
    using Configs;
    using Movement;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using UnityEngine;

    /// <summary>
    /// Система движения препятствий
    /// </summary>
    public class ObstacleMovement : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;

        private readonly EcsCustomInject<GameConfig> _gameConfig;
        
        private EcsFilterInject<Inc<MovementComponent, ObstacleComponent>> _filter;

        private EcsPool<MovementComponent> _movementPool;

        private EcsPool<ObstacleComponent> _obstaclePool;

        private Dictionary<int, TweenerCore<Vector3, Vector3, VectorOptions>> _dictionary = new();

        public void Init(IEcsSystems systems)
        {
            _movementPool = _world.Value.GetPool<MovementComponent>();
            _obstaclePool = _world.Value.GetPool<ObstacleComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var movementComponent = ref _movementPool.Get(entity);

                ref var obstacleComponent = ref _obstaclePool.Get(entity);

                if (movementComponent.IsMoving)
                {
                    continue;
                }

                if (obstacleComponent.CanRelease)
                {
                    _dictionary.GetValueOrDefault(entity)?.Pause();
                    continue;
                }

                var tweenerCore = movementComponent.Transform
                    .DOMoveY(0, _gameConfig.Value.DifficultData.SpeedObstacle)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => EndMovement(entity));

                _dictionary.TryAdd(entity, tweenerCore);

                movementComponent.IsMoving = true;
            }
        }

        private void EndMovement(int entity)
        {
            ref var obstacleComponent = ref _obstaclePool.Get(entity);

            obstacleComponent.CanRelease = true;
        }
    }
}
