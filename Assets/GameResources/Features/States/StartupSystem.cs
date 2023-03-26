namespace Game.Features.States
{
    using UI;
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using Leopotam.EcsLite.Unity.Ugui;
    using UnityEngine.UI;

    public class StartupSystem : EcsUguiCallbackSystem, IEcsInitSystem, IEcsDestroySystem
    {
        private const string WIDGET_NAME = "StartGame";

        private readonly EcsWorldInject _world = default;

        private readonly EcsUguiEmitter _ugui = default;

        private readonly EcsCustomInject<MainUI> _mainUI;

        private readonly EcsCustomInject<IStateMachine> _stateMachine;

        private Button _btnStart;

        public void Init(IEcsSystems systems)
        {
            if (_stateMachine.Value.CurrentState() != State.Start)
            {
                return;
            }

            _mainUI.Value.StartWindow.SetActive(true);
            _btnStart = _ugui.GetNamedObject(WIDGET_NAME).GetComponent<Button>();
            _btnStart.onClick.AddListener(OnStartGame);
        }

        private void OnStartGame()
        {
            _stateMachine.Value.Enter(State.Game);
            _mainUI.Value.StartWindow.SetActive(false);
        }

        public void Destroy(IEcsSystems systems)
            => _btnStart.onClick.RemoveListener(OnStartGame);
    }
}
