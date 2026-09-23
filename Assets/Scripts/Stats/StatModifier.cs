using System;
using System.Globalization;

namespace ProjectTD.Stats
{
    /// <summary>
    /// Modificatore applicato a una statistica di una torretta da un potere, una carta o un effetto.
    /// È un valore immutabile: per cambiarlo si rimuove e se ne aggiunge uno nuovo.
    /// L'ordine di applicazione è definito da <see cref="StatModifierType"/>.
    /// </summary>
    public readonly struct StatModifier
    {
        /// <summary>Statistica su cui agisce il modificatore.</summary>
        public readonly ModifiableStat Stat;

        /// <summary>Operazione con cui il modificatore si combina al valore base.</summary>
        public readonly StatModifierType Type;

        /// <summary>
        /// Entità del modificatore.
        /// Per <see cref="StatModifierType.Flat"/>: valore assoluto da sommare (es. 5 = +5 danno).
        /// Per <see cref="StatModifierType.PercentAdd"/> e <see cref="StatModifierType.PercentMult"/>:
        /// frazione (0.1 = +10%, -0.2 = -20%).
        /// </summary>
        public readonly float Value;

        /// <summary>
        /// Chi ha applicato il modificatore (torretta, potere, effetto a tempo).
        /// Serve a rimuovere tutti i modificatori di una sorgente quando la torretta viene venduta
        /// o l'effetto scade. Il confronto avviene per riferimento: non usare stringhe o id condivisi
        /// tra più istanze, altrimenti rimuovendone una si rimuoverebbero anche i modificatori delle altre.
        /// </summary>
        public readonly object Source;

        /// <summary>Crea un modificatore per la statistica indicata.</summary>
        /// <param name="stat">Statistica da modificare.</param>
        /// <param name="type">Operazione da applicare.</param>
        /// <param name="value">Entità del modificatore; per le percentuali è una frazione (0.1 = +10%).</param>
        /// <param name="source">Chi applica il modificatore, usato per rimuoverlo in seguito.</param>
        public StatModifier(ModifiableStat stat, StatModifierType type, float value, object source)
        {
            Stat = stat;
            Type = type;
            Value = value;
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        /// <summary>
        /// Rappresentazione leggibile per log e debug, es. "Damage Flat +5 (Tower)" o
        /// "FireRate PercentAdd +20% (Tower)". Usa la cultura invariante così il separatore
        /// decimale è sempre il punto, indipendentemente dalle impostazioni del sistema.
        /// </summary>
        public override string ToString()
        {
            string formattedValue = Type == StatModifierType.Flat
                ? Value.ToString(SignedNumberFormat, CultureInfo.InvariantCulture)
                : (Value * 100f).ToString(SignedNumberFormat, CultureInfo.InvariantCulture) + "%";

            return $"{Stat} {Type} {formattedValue} ({Source})";
        }

        // Segno sempre esplicito e al massimo due decimali: +5, -0.5, +12.34.
        private const string SignedNumberFormat = "+0.##;-0.##";
    }
}
