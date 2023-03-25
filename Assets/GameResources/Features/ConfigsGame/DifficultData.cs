namespace Game.Features.Configs
{
    using UnityEngine;

    /// <summary>
    /// Конфиг для настройки сложности игры
    /// </summary>
    [CreateAssetMenu(fileName = nameof(DifficultData), menuName = "Game/Data/" + nameof(DifficultData))]
    public class DifficultData : ScriptableObject
    {
        [field: SerializeField] 
        public float SpeedBalloon { get; private set; }

        [field: SerializeField]
        public int CountObstacleInScreen { get; private set; }
        
        [field: SerializeField]
        public int SpawnObstacleDelay { get; private set; }
        
        [field: SerializeField]
        public int SpeedObstacle { get; private set; }
    }
}
