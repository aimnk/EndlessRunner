using Game.Features.Extensions;
using Game.Features.Player;
using Game.Features.Restart;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace GameResources.Features.Obstacles
{
    /// <summary>
    /// Система столкновения с препятствиями
    /// </summary>
    public class ObstaclesCollisionSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        
        private EcsFilterInject<Inc<OnTriggerEnter2DEvent>> _filterTrigger;
        
        private EcsFilterInject<Inc<PlayerMarker>> _filterPlayer;
        
        private bool _isTrigger = false;
        
        public void Run(IEcsSystems systems)
        {
            if (_isTrigger)
            {
                return;
            }
            
            foreach (var entity in _filterTrigger.Value)
            {
                _isTrigger = true;
            }

            if (_isTrigger)
            {
                foreach (var entity in _filterPlayer.Value)
                {
                    _world.Value.AddComponent<RestartEvent>(entity);
                }
            }
        }
    }
}    