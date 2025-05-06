using ChristmasTree.Controls.Events;
using ChristmasTree.Controls.Platform;
using ChristmasTree.Controls.Projectiles;
using ChristmasTree.Controls.Thrower;
using ChristmasTree.Services.Inventory;
using ChristmasTree.Services.Score;
using Leopotam.Ecs;
using UnityEngine;
using Voody.UniLeo;

namespace ChristmasTree.Management
{
    public class EcsStartup : MonoBehaviour
    {
        public static EcsStartup Instance { get; private set; }
        public EcsWorld World => _world;
        
        private EcsWorld _world;
        private EcsSystems _runSystems;

        private GameStateMachine _gameStateMachine;
        private InventoryService _inventoryService;
        private ScoreService _scoreService;
        
        public EcsStartup Initialize(
            GameStateMachine gameStateMachine, 
            InventoryService inventoryService, 
            ScoreService scoreService)
        {
            _gameStateMachine = gameStateMachine;
            _inventoryService = inventoryService;
            _scoreService = scoreService;
            return this;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _world = new EcsWorld();
            _runSystems = new EcsSystems(_world);

            _runSystems.ConvertScene();
            
            AddInjections();
            AddSystems();
            AddEvents();
            
            _runSystems.Init();
        }

        private void AddInjections()
        {
            _runSystems.Inject(_gameStateMachine);
        }

        private void AddSystems()
        {
            _runSystems
                .Add(new RotatingPlatformSystem())
                .Add(new ThrowerSystem(_inventoryService))
                .Add(new ProjectileEventsSystem())
                .Add(new ScoreSystem(_scoreService));
        }

        private void AddEvents()
        {
            _runSystems
                .OneFrame<OnMouseDownComponent>()
                .OneFrame<OnMouseDragComponent>()
                .OneFrame<OnMouseUpComponent>()
                .OneFrame<OnProjectileDroppedComponent>()
                .OneFrame<OnScoreChangedComponent>()
                .OneFrame<OnStarSpent>();
        }

        private void Update()
        {
            _runSystems.Run();
        }

        private void OnDestroy() 
        {
            if (_runSystems == null)
            {
                Debug.LogWarning("Possible attempt to destroy EcsCore twice");
                return;
            }

            _runSystems.Destroy();
            _runSystems = null;
            _world.Destroy();
            _world = null;
        }
    }
}