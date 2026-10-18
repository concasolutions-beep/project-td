using UnityEngine;
using ProjectTD.Projectile;

namespace ProjectTD.Tower
{
    /// <summary>
    /// Valori base di un tipo di torretta, condivisi da tutte le torrette che puntano allo stesso asset.
    /// È sola lettura a runtime: in Editor una scrittura su uno ScriptableObject resterebbe nel file anche
    /// dopo lo stop del gioco. Lo stato modificabile di ogni torretta piazzata sta in <see cref="TowerRuntimeStats"/>.
    /// I nomi dei campi serializzati non vanno cambiati: sono quelli salvati negli asset.
    /// </summary>
    [CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
    public class TowerData : ScriptableObject
    {
        [Header("Combat")]
        [SerializeField] private float range = 3f;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private ProjectileData projectileData;
        [Header("Meta")]
        [SerializeField] private string towerName;
        [SerializeField] private int cost;

        public float Range => range;
        public float FireRate => fireRate;
        public ProjectileData ProjectileData => projectileData;
        public string TowerName => towerName;
        public int Cost => cost;

        /// <summary>Danno base di un colpo: vive nel <see cref="ProjectileData"/> della torretta.</summary>
        public float Damage => projectileData != null ? projectileData.damage : 0f;
    }
}
