using UnityEngine;

namespace Game.Features.Configs
{
    /// <summary>
    /// Конфиг для настройки управления
    /// </summary>
    [CreateAssetMenu(fileName = nameof(InputConfigData), menuName = "Game/Data/" + nameof(InputConfigData))]
    public class InputConfigData : ScriptableObject
    {
        [field: SerializeField] 
        public float MoveDelay { get; private set; }
    }
}