namespace Game.Features.Obstacles
{
    using System.Collections.Generic;
    using DG.Tweening;
    using Configs;
    using Movement;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using States;


    /// <summary>
    /// Система движения препятствий
    /// </summary>
    public class ObstacleMovement : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private readonly EcsWorldInject _world = default;

        private readonly EcsCustomInject<GameConfig> _gameConfig;
        
        private EcsFilterInject<Inc<MovementComponent, ObstacleComponent>> _filter;

        private EcsCustomInject<IStateMachine> _stateMachine;
        
        private EcsPool<MovementComponent> _movementPool;

        private EcsPool<ObstacleComponent> _obstaclePool;

        private Dictionary<int, Sequence> _dictionary = new();

        private bool _wasKilled = false;
        

        public void Init(IEcsSystems systems)
        {
            _movementPool = _world.Value.GetPool<MovementComponent>();
            _obstaclePool = _world.Value.GetPool<ObstacleComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (_stateMachine.Value.CurrentState() != State.Game)
            {
                foreach (var sequence in _dictionary)
                {
                    ref var obstacleComponent = ref _obstaclePool.Get(sequence.Key);

                    if (!sequence.Value.IsPlaying())
                    {
                        return;
                    }
                    if (!obstacleComponent.CanRelease)
                    {
                        obstacleComponent.CanRelease = true;
                        sequence.Value.Kill();
                    }
                }
                _dictionary.Clear();
                return;
            }
            
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
                
                var sequence = DOTween.Sequence()
                    .Append(movementComponent.Transform.DOMoveY(0, _gameConfig.Value.DifficultData.SpeedObstacle).SetEase(Ease.Linear))
                    .Insert(0, movementComponent.Transform.DOShakeRotation(_gameConfig.Value.DifficultData.SpeedObstacle, 50, 1).SetEase(Ease.Linear))
                    .OnComplete(() => EndMovement(entity));
                
                _dictionary.Add(entity, sequence);

                movementComponent.IsMoving = true;
                _wasKilled = false;
            }
        }

        private void EndMovement(int entity)
        {
            ref var obstacleComponent = ref _obstaclePool.Get(entity);
            obstacleComponent.CanRelease = true;
        }

        public void Destroy(IEcsSystems systems)
        {
            foreach (var sequence in _dictionary)
            {
                sequence.Value.Kill();
            }
        }
    }
}
