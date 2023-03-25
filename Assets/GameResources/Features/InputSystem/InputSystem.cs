namespace Game.Features.Input
{
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using UnityEngine;
    using Movement;

    /// <summary>
    /// Система управленя свайпами
    /// </summary>
    public class InputSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly float _swipeRange = 50f;

        private readonly EcsWorldInject _world = default;

        private EcsFilterInject<Inc<MovementComponent, InputComponent>> _filter;

        private Vector2 _startTouchPosition;

        private Vector2 _endTouchPosition;

        private EcsPool<InputComponent> _inputComponentPool;

        private bool _stopTouch = false;

        public void Init(IEcsSystems systems)
            => _inputComponentPool = _world.Value.GetPool<InputComponent>();

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var inputComponent = ref _inputComponentPool.Get(entity);
                inputComponent.MoveLeft = false;
                inputComponent.MoveRight = false;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _startTouchPosition = Input.GetTouch(0).position;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                _endTouchPosition = Input.GetTouch(0).position;
                Vector2 distance = _endTouchPosition - _startTouchPosition;

                if (!_stopTouch)
                {
                    foreach (var entity in _filter.Value)
                    {
                        ref var inputComponent = ref _inputComponentPool.Get(entity);

                        if (distance.x < -_swipeRange)
                        {
                            inputComponent.MoveLeft = true;
                            _stopTouch = true;
                        }

                        else if (distance.x > _swipeRange)
                        {
                            inputComponent.MoveRight = true;
                            _stopTouch = true;
                        }
                    }
                }
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                _stopTouch = false;
            }
        }
    }
}
