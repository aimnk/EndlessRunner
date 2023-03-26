namespace Game.Features.States
{
    using Leopotam.EcsLite;
    using Leopotam.EcsLite.Di;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using UI;
    
    /// <summary>
    /// Система перезапуска игры
    /// </summary>
    public class RestartSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        
        private readonly EcsCustomInject<MainUI> _mainUI;
        
        private readonly EcsCustomInject<IStateMachine> _stateMachine;

        private int _time = 3;

        private bool _wasShowTimer = false;
        
        public void Run(IEcsSystems systems)
        {
            if (_stateMachine.Value.CurrentState() != State.Death)
            {
                return;
            }
            if (!_wasShowTimer)
            {
                _time = 3;
                _mainUI.Value.TimerWindow.gameObject.SetActive(true);
                _mainUI.Value.TimerWindow.TimerText.text = _time.ToString();
                _mainUI.Value.TimerWindow.StartCoroutine(ShowTimer(_mainUI.Value.TimerWindow.TimerText));
                _wasShowTimer = true;
            }
        }

        private IEnumerator ShowTimer(Text timerText)
        {
            while (_time > 0)
            {
                yield return new WaitForSecondsRealtime(1);
                _time--;
                timerText.text = _time.ToString();
            }
            
            _mainUI.Value.TimerWindow.gameObject.SetActive(false);
            _wasShowTimer = false;
            _stateMachine.Value.Enter(State.Game);
        }
    }
}
