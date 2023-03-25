namespace Game.Features.Player
{
    using Configs;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using UnityEngine;
    using Input;
    using DG.Tweening;
    using Movement;

    /// <summary>
    /// Система передвижения игрокоы
    /// </summary>
    public class PlayerMovementSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float MOVE_STEP = 8;

        private const float ROTATE_STEP = 25;

        private readonly EcsWorldInject _world = default;

        private readonly EcsCustomInject<GameConfig> _gameConfig;
        
        private EcsFilterInject<Inc<MovementComponent, InputComponent>> _filter;
        
        private EcsPool<InputComponent> _inputComponentPool;

        private EcsPool<MovementComponent> _movementPool;

        private bool _isMovement = false;

        public void Init(IEcsSystems systems)
        {
            _inputComponentPool = _world.Value.GetPool<InputComponent>();
            _movementPool = _world.Value.GetPool<MovementComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref Transform transform = ref _movementPool.Get(entity).Transform;
                ref InputComponent inputComponent = ref _inputComponentPool.Get(entity);

                if (inputComponent.MoveRight && !_isMovement)
                {
                    if (transform.position.x < MOVE_STEP)
                    {
                        TweenMovement(ref transform, true);
                    }
                }

                if (inputComponent.MoveLeft && !_isMovement)
                {
                    if (transform.position.x > -MOVE_STEP)
                    {
                        TweenMovement(ref transform, false);
                    }
                }
            }
        }

        private void TweenMovement(ref Transform transform, bool isRight)
        {
            _isMovement = true;
            DOTween.Sequence()
                .Append(transform.DOMoveX(isRight ? transform.position.x + MOVE_STEP : transform.position.x - MOVE_STEP, _gameConfig.Value.InputConfigData.MoveDelay))
                .Insert(0, transform.DORotate(new Vector3(0, 0, isRight ? -ROTATE_STEP : ROTATE_STEP), _gameConfig.Value.InputConfigData.MoveDelay / 2))
                .Insert(_gameConfig.Value.InputConfigData.MoveDelay / 2, transform.DORotate(Vector3.zero, _gameConfig.Value.InputConfigData.MoveDelay / 2))
                .OnComplete(() => _isMovement = false);
        }
    }
}
