namespace Game.Base
{
   using Features.AssetProvider;
   using Features.Configs;
   using Features.Input;
   using Features.Obstacles;
   using Features.Parallax;
   using Features.Player;
   using Leopotam.EcsLite;
   using Leopotam.EcsLite.Di;
   using UnityEngine;
   using Features.Restart;
   using Features.UI;
   using GameResources.Features.Obstacles;
   using LeoEcsPhysics;
   
   /// <summary>
   /// Инициализатор игры
   /// </summary>
   public class GameBootstrap : MonoBehaviour
   {
      [SerializeField]
      private MainUI _mainUI;
      
      private EcsWorld _world;

      private EcsSystems _systems;

      private EcsSystems _physicsSystem;

      private IAssetProvider _assetProvider = new AssetResourcesProvider();
      
      private void Start()
      {
         _world = new EcsWorld();
         _systems = new EcsSystems(_world);
         _physicsSystem = new EcsSystems(_world);
         EcsPhysicsEvents.ecsWorld = _world;
         
         InitBaseSystems();
         InitPhysicsSystems();
      }

      private void InitBaseSystems()
      {
         _systems
            .Add(new PlayerInitSystem())
            .Add(new ParallaxInitSystem())
            .Add(new InputSystem())
            .Add(new PlayerMovementSystem())
            .Add(new ObstaclesSpawnSystem())
            .Add(new ObstacleMovement())
            .Add(new  RestartSystem())
            .Inject(_assetProvider, new GameConfig(_assetProvider), _mainUI)
            .Init();
      }

      private void InitPhysicsSystems()
      {
         _physicsSystem
            .Add(new ObstaclesCollisionSystem())
            .Inject()
            .Init();
      }

      private void Update()
         => _systems?.Run();

      private void FixedUpdate()
         => _physicsSystem?.Run();

      private void OnDestroy()
      {
         if (_systems != null)
         {
            _systems.Destroy();
            _systems = null;
         }

         if (_physicsSystem != null)
         {
            _physicsSystem.Destroy();
            _physicsSystem = null;
            EcsPhysicsEvents.ecsWorld = null;
         }

         if (_world != null)
         {
            _world.Destroy();
            _world = null;
         }
      }
   }
}
