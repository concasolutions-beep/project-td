using UnityEngine;
using ProjectTD.Enemy;

namespace ProjectTD.Wave
{
    [CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
    public class WaveData : ScriptableObject
    {
        [System.Serializable]
        public class EnemySpawnInfo
        {
            public EnemyData enemy;
            public int count;
            public float spawnDelay;
        }

        public EnemySpawnInfo[] enemies;
    }
}
