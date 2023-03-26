namespace Game.Features.Obstacles
{
    using LeoEcsPhysics;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using States;
    
    /// <summary>
    /// Система столкновения с препятствиями
    /// </summary>
    public class ObstaclesCollisionSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        
        private EcsFilterInject<Inc<OnTriggerEnter2DEvent>> _filterTrigger;
        
        private EcsCustomInject<IStateMachine> _stateMachine;
        
        private bool _isTrigger = false;

        private float _timeDelay;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filterTrigger.Value)
            {
                _stateMachine.Value.Enter(State.Death);
                var triggerPool = _world.Value.GetPool<OnTriggerEnter2DEvent>();
                triggerPool.Del(entity);
            }
        }
    }
}    