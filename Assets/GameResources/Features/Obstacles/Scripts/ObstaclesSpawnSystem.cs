using System.Linq;

namespace Game.Features.Obstacles
{
    using System.Collections.Generic;
    using AssetProvider;
    using Configs;
    using Extensions;
    using Movement;
    using Parallax;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using UnityEngine;
    using UnityEngine.Pool;
    using States;

    /// <summary>
    /// Спавнер препятствий
    /// </summary>
    public class ObstaclesSpawnSystem : IEcsInitSystem, IEcsRunSystem
    {
        private const float MOVE_STEP = 8;
        private const int COUNT_LINES = 3;

        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<IAssetProvider> _assetProvider;
        private readonly EcsCustomInject<GameConfig> _gameConfig;
        private EcsCustomInject<IStateMachine> _stateMachine;

        private EcsFilterInject<Inc<LevelComponent>> _filterLevel;
        private EcsFilterInject<Inc<ObstacleComponent>> _filterObstacle;

        private SpawnObjectData _spawnObjectData;
        private ObstaclesContainerData _obstaclesContainerData;

        private EcsPool<LevelComponent> _levelPool;
        private EcsPool<ObstacleComponent> _obstaclePool;
        private EcsPool<MovementComponent> _movementPool;

        private ObjectPool<EntityGameObjectData> _objectPool;
        private List<EntityGameObjectData> _entityPoolDatas = new List<EntityGameObjectData>();

        private float _timeDelay;

        private float _currentHeight;

        public void Init(IEcsSystems systems)
        {
            _spawnObjectData = _assetProvider.Value.LoadAsset<SpawnObjectData>(AssetsDataPath.OBSTACLES_SPAWN_DATA);
            _obstaclesContainerData =
                _assetProvider.Value.LoadAsset<ObstaclesContainerData>(AssetsDataPath.OBSTACLES_CONTAINER_DATA_PATH);
            _levelPool = _world.Value.GetPool<LevelComponent>();
            _obstaclePool = _world.Value.GetPool<ObstacleComponent>();
            _movementPool = _world.Value.GetPool<MovementComponent>();

            InitPool();
            SpawnObstacleByHeight();
        }

        private void InitPool()
        {
            _objectPool = new ObjectPool<EntityGameObjectData>(
                CreateEntityData,
                data =>
                {
                    ref var obstacleComponent = ref _movementPool.Get(data.Entity);
                    obstacleComponent.IsMoving = false;
                    data.GameObject.SetActive(true);
                },
                data =>
                {
                    ref var obstacleComponent = ref _obstaclePool.Get(data.Entity);
                    obstacleComponent.CanRelease = false;
                    data.GameObject.SetActive(false);
                },
                DestroyEntityData,
                true, _gameConfig.Value.DifficultData.CountObstacleInScreen);
        }

        private void DestroyEntityData(EntityGameObjectData data)
        {
            _entityPoolDatas.Remove(data);
            _world.Value.DelEntity(data.Entity);
            Object.Destroy(data.GameObject);
        }

        private EntityGameObjectData CreateEntityData()
        {
            var entity = _world.Value.NewEntity();
            var gameObject = Object.Instantiate(_spawnObjectData.Prefab, Vector3.zero, Quaternion.identity);
            _world.Value.AddComponent<MovementComponent>(entity).Transform = gameObject.transform;
            _world.Value.AddComponent<ObstacleComponent>(entity);
            
            var obstacle = new EntityGameObjectData(entity, gameObject);
            _entityPoolDatas.Add(obstacle);
            
            return new EntityGameObjectData(entity, gameObject);
        }

        public void Run(IEcsSystems systems)
        {
            if (_stateMachine.Value.CurrentState() != State.Game)
            {
                if (_objectPool.CountActive == 0)
                {
                    return;
                }
                
                foreach (var entityPoolData in _entityPoolDatas)
                {
                    ref var obstacleComponent = ref _obstaclePool.Get(entityPoolData.Entity);
                    if (obstacleComponent.CanRelease)
                    {
                        _objectPool?.Release(entityPoolData);
                    }
                }
                return;
            }
            
            foreach (var entity in _filterObstacle.Value)
            {
                ref ObstacleComponent obstacleComponent = ref _obstaclePool.Get(entity);

                if (obstacleComponent.CanRelease)
                {
                    _objectPool.Release(_entityPoolDatas.FirstOrDefault(x => x.Entity == entity));
                }
                else if (_objectPool.CountActive < _gameConfig.Value.DifficultData.CountObstacleInScreen)
                {
                    if (Time.time - _timeDelay < _gameConfig.Value.DifficultData.SpawnObstacleDelay)
                    {
                        return;
                    }

                    SpawnObstacleByHeight();

                    _timeDelay = Time.time;
                }
            }
        }

        private void SpawnObstacleByHeight()
        {
            foreach (var entity in _filterLevel.Value)
            {
                ref LevelComponent level = ref _levelPool.Get(entity);
                _currentHeight = level.CurrentHeight;
            }

            foreach (var heightObstaclesData in _obstaclesContainerData.HeightObstaclesDatas)
            {
                if (heightObstaclesData.MinHeight <= _currentHeight && heightObstaclesData.Maxheight > _currentHeight)
                {
                    var obstacle = _objectPool.Get();

                    var obstaclesData =
                        heightObstaclesData.ObstaclesDatas[Random.Range(0, heightObstaclesData.ObstaclesDatas.Count)];

                    SpawnObstacle(obstacle, obstaclesData);
                }
            }
        }

        private void SpawnObstacle(EntityGameObjectData obstacle, ObstaclesData obstaclesData)
        {
            var randomPos = Random.Range(0, COUNT_LINES + 1);

            obstacle.GameObject.transform.localPosition = new Vector3(
                randomPos == 0 ? -MOVE_STEP : randomPos == 1 ? 0 : MOVE_STEP, _spawnObjectData.StartPoint.y, 0);

            obstacle.GameObject.transform.localRotation = Quaternion.Euler(obstaclesData.Rotation);
            obstacle.GameObject.transform.localScale = obstaclesData.Scale;
            obstacle.GameObject.GetComponent<SpriteRenderer>().sprite = obstaclesData.SpriteObstacle;
        }

        private class EntityGameObjectData
        {
            public int Entity { get; }

            public GameObject GameObject { get; }

            public EntityGameObjectData(int entity, GameObject gameObject)
            {
                Entity = entity;
                GameObject = gameObject;
            }
        }
    }
}