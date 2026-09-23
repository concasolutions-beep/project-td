namespace ProjectTD.Stats
{
    /// <summary>
    /// Statistiche di una torretta che un potere/carta può modificare.
    /// Contratto di design: una statistica non presente qui non è raggiungibile da nessun potere.
    /// </summary>
    public enum ModifiableStat
    {
        /// <summary>Danno inflitto da un singolo colpo.</summary>
        Damage,

        /// <summary>Colpi al secondo (o intervallo tra un colpo e il successivo).</summary>
        FireRate,

        /// <summary>Raggio massimo entro cui la torretta può bersagliare un nemico.</summary>
        Range,

        /// <summary>Raggio minimo: distanza sotto la quale la torretta non può colpire.</summary>
        MinRange,

        /// <summary>Raggio dell'esplosione per proiettili ad area (AoE).</summary>
        ExplosionRadius,

        /// <summary>Velocità di spostamento del proiettile.</summary>
        ProjectileSpeed,

        /// <summary>Numero di nemici che un proiettile può attraversare prima di esaurirsi.</summary>
        Pierce,

        /// <summary>Costo in oro per costruire la torretta.</summary>
        BuildCost,

        /// <summary>Costo in oro per potenziare la torretta al livello successivo.</summary>
        UpgradeCost
    }
}
