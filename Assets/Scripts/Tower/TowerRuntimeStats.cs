using System;
using ProjectTD.Stats;

namespace ProjectTD.Tower
{
    /// <summary>
    /// Statistiche di una singola torretta piazzata. Copia i valori base da <see cref="TowerData"/> alla
    /// creazione e da quel momento non lo legge né lo scrive più: i poteri agiscono solo su questa istanza,
    /// quindi due torrette dello stesso tipo non si influenzano e l'asset non viene mai toccato.
    /// </summary>
    public class TowerRuntimeStats
    {
        private readonly StatCollection stats = new StatCollection();

        public TowerRuntimeStats(TowerData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            stats.SetBaseValue(ModifiableStat.Damage, data.Damage);
            stats.SetBaseValue(ModifiableStat.FireRate, data.FireRate);
            stats.SetBaseValue(ModifiableStat.Range, data.Range);
            stats.SetBaseValue(ModifiableStat.BuildCost, data.Cost);
        }

        /// <summary>Modificatori e valori finali di questa torretta.</summary>
        public StatCollection Stats => stats;

        public float Damage => stats.GetValue(ModifiableStat.Damage);
        public float FireRate => stats.GetValue(ModifiableStat.FireRate);
        public float Range => stats.GetValue(ModifiableStat.Range);

        /// <summary>Riporta la torretta ai valori base dell'asset (es. quando torna nel pool).</summary>
        public void ResetModifiers() => stats.ClearModifiers();
    }
}
