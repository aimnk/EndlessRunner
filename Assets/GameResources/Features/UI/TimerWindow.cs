namespace Game.Features.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Окно с таймером перезагрузки игры
    /// </summary>
    public class TimerWindow : MonoBehaviour
    {
        [field: SerializeField] 
        public Text TimerText { get; private set; }
    }
}