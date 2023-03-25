using Game.Features.Configs;

namespace Game.Features.Player
{
    using AssetProvider;
    using Extensions;
    using Input;
    using Movement;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using UnityEngine;

    /// <summary>
    /// Система спавна и инициализации игрока
    /// </summary>
    public class PlayerInitSystem : IEcsInitSystem
    {
        private const string PLAYER_DATA_PATH = "PlayerData";

        private readonly EcsWorldInject _world = default;

        private EcsCustomInject<IAssetProvider> _assetProvider;

        private int _playerEntity;

        public void Init(IEcsSystems systems)
        {
            SpawnObjectData spawnObjectData = _assetProvider.Value.LoadAsset<SpawnObjectData>(PLAYER_DATA_PATH);

            var playerGameObject =
                GameObject.Instantiate(spawnObjectData.Prefab, spawnObjectData.StartPoint, Quaternion.identity);

            _playerEntity = _world.Value.NewEntity();

            ref var movementComponent = ref _world.Value.AddComponent<MovementComponent>(_playerEntity);
            _world.Value.AddComponent<InputComponent>(_playerEntity);
            _world.Value.AddComponent<PlayerMarker>(_playerEntity);

            movementComponent.Transform = playerGameObject.transform;
        }
    }
}
