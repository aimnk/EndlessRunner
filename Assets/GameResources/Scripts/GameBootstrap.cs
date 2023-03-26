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
   using Features.UI;
   using LeoEcsPhysics;
   using Features.States;
   using Leopotam.EcsLite.Unity.Ugui;
   
   /// <summary>
   /// Инициализатор игры
   /// </summary>
   public class GameBootstrap : MonoBehaviour
   {
      [SerializeField]
      private MainUI _mainUI;

      [SerializeField] 
      private EcsUguiEmitter _uguiEmitter;
      
      private EcsWorld _world;

      private EcsSystems _systems;

      private EcsSystems _physicsSystem;

      private IAssetProvider _assetProvider = new AssetResourcesProvider();

      private IStateMachine _stateMachine = new SimpleStateMachine();
      
      private void Start()
      {
         _world = new EcsWorld();
         _systems = new EcsSystems(_world, _stateMachine);
         _physicsSystem = new EcsSystems(_world, _stateMachine);
         EcsPhysicsEvents.ecsWorld = _world;
         
         _stateMachine.Enter(State.Start);
         InitBaseSystems();
         InitPhysicsSystems();
      }

      private void InitBaseSystems()
      {
         _systems
            .Add(new StartupSystem())
            .Add(new PlayerInitSystem())
            .Add(new ParallaxInitSystem())
            .Add(new InputSystem())
            .Add(new PlayerMovementSystem())
            .Add(new ObstaclesSpawnSystem())
            .Add(new ObstacleMovement())
            .Add(new  RestartSystem())
            .Inject(_assetProvider, new GameConfig(_assetProvider), _mainUI, _stateMachine)
            .InjectUgui(_uguiEmitter)
            .Init();
      }

      private void InitPhysicsSystems()
      {
         _physicsSystem
            .Add(new ObstaclesCollisionSystem())
            .Inject(_stateMachine)
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
            _systems.GetWorld("ugui-events")?.Destroy ();
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
