namespace ProjectTD.Stats
{
    /// <summary>
    /// Tipo di operazione di un <see cref="StatModifier"/>.
    /// Contratto di bilanciamento: i modificatori si applicano in ordine crescente di valore
    /// (Flat → PercentAdd → PercentMult). Non riordinare né rinumerare: cambierebbe il valore
    /// finale di tutte le statistiche e invaliderebbe il bilanciamento dei poteri.
    /// Formula: finale = (base + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult).
    /// </summary>
    public enum StatModifierType
    {
        /// <summary>Valore assoluto sommato alla base. Es. +5 danno.</summary>
        Flat = 100,

        /// <summary>
        /// Percentuale che si somma alle altre PercentAdd prima di essere applicata.
        /// Es. +10% e +20% danno +30%.
        /// </summary>
        PercentAdd = 200,

        /// <summary>
        /// Percentuale che si moltiplica in cascata con le altre PercentMult.
        /// Es. +10% e +20% danno +32% (1.1 × 1.2).
        /// </summary>
        PercentMult = 300
    }
}
