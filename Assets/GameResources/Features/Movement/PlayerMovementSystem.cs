namespace Game.Features.Player
{
    using Configs;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using UnityEngine;
    using Input;
    using DG.Tweening;
    using Movement;
    using AssetProvider;
    using States;
    using Unity.Mathematics;

    /// <summary>
    /// Система передвижения игрокоы
    /// </summary>
    public class PlayerMovementSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private const float MOVE_STEP = 8;

        private const float ROTATE_STEP = 25;

        private readonly EcsWorldInject _world = default;

        private readonly EcsCustomInject<GameConfig> _gameConfig;
        
        private readonly EcsCustomInject<IAssetProvider> _assetProvider;
        
        private EcsFilterInject<Inc<MovementComponent, InputComponent>> _filter;
        
        private EcsCustomInject<IStateMachine> _stateMachine;
        
        private EcsPool<InputComponent> _inputComponentPool;

        private EcsPool<MovementComponent> _movementPool;

        private SpawnObjectData _spawnObjectData;
        
        private bool _isMovement = false;

        private Sequence _sequence;

        private Vector3 _startPoint;
        
        public void Init(IEcsSystems systems)
        {
            _spawnObjectData = _assetProvider.Value.LoadAsset<SpawnObjectData>(AssetsDataPath.PLAYER_DATA_PATH);
            _inputComponentPool = _world.Value.GetPool<InputComponent>();
            _movementPool = _world.Value.GetPool<MovementComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (_stateMachine.Value.CurrentState() != State.Game)
            {
                foreach (var entity in _filter.Value)
                {
                    ref Transform transform = ref _movementPool.Get(entity).Transform;
                    transform.position = _spawnObjectData.StartPoint;
                    transform.rotation = quaternion.identity;
                }
                
                _sequence.Kill();
                return;
            }

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
            _sequence = DOTween.Sequence()
                .Append(transform.DOMoveX(isRight ? transform.position.x + MOVE_STEP : transform.position.x - MOVE_STEP, _gameConfig.Value.InputConfigData.MoveDelay))
                .Insert(0, transform.DORotate(new Vector3(0, 0, isRight ? -ROTATE_STEP : ROTATE_STEP), _gameConfig.Value.InputConfigData.MoveDelay / 2))
                .Insert(_gameConfig.Value.InputConfigData.MoveDelay / 2, transform.DORotate(Vector3.zero, _gameConfig.Value.InputConfigData.MoveDelay / 2))
                .OnComplete(() => _isMovement = false)
                .OnKill(() => _isMovement = false);
        }
        
        public void Destroy(IEcsSystems systems) 
            => _sequence.Kill();
    }
}
