using System;
using System.Collections.Generic;

namespace ProjectTD.Stats
{
    /// <summary>
    /// Valori base e modificatori attivi di una torretta, con valore finale in cache.
    /// <see cref="GetValue"/> si può chiamare ogni frame: il ricalcolo avviene solo se la lista dei
    /// modificatori (o il valore base) di quella statistica è cambiata dall'ultima lettura.
    /// Formula: finale = (base + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult).
    /// </summary>
    public class StatCollection
    {
        private static readonly int StatCount = Enum.GetValues(typeof(ModifiableStat)).Length;

        private readonly float[] baseValues = new float[StatCount];
        private readonly float[] cachedValues = new float[StatCount];
        private readonly bool[] dirty = new bool[StatCount];
        private readonly List<StatModifier> modifiers = new List<StatModifier>();

        public StatCollection()
        {
            MarkAllDirty();
        }

        /// <summary>Modificatori attualmente applicati, in sola lettura.</summary>
        public IReadOnlyList<StatModifier> Modifiers => modifiers;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>Numero di ricalcoli effettuati finora; solo debug, per verificare che non avvengano a ogni frame.</summary>
        public int RecalculationCount { get; private set; }
#endif

        public void SetBaseValue(ModifiableStat stat, float value)
        {
            int index = (int)stat;
            if (baseValues[index] == value)
                return;

            baseValues[index] = value;
            dirty[index] = true;
        }

        public void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
            dirty[(int)modifier.Stat] = true;
        }

        /// <summary>Rimuove la prima occorrenza del modificatore. Restituisce false se non presente.</summary>
        public bool RemoveModifier(StatModifier modifier)
        {
            int index = modifiers.FindIndex(m => m.Equals(modifier));
            if (index < 0)
                return false;

            modifiers.RemoveAt(index);
            dirty[(int)modifier.Stat] = true;
            return true;
        }

        /// <summary>Rimuove tutti i modificatori di una sorgente (confronto per riferimento). Restituisce quanti ne ha tolti.</summary>
        public int RemoveAllFromSource(object source)
        {
            int removed = 0;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                if (!ReferenceEquals(modifiers[i].Source, source))
                    continue;

                dirty[(int)modifiers[i].Stat] = true;
                modifiers.RemoveAt(i);
                removed++;
            }

            return removed;
        }

        public void ClearModifiers()
        {
            if (modifiers.Count == 0)
                return;

            modifiers.Clear();
            MarkAllDirty();
        }

        public float GetValue(ModifiableStat stat)
        {
            int index = (int)stat;
            if (dirty[index])
            {
                cachedValues[index] = Recalculate(stat, baseValues[index]);
                dirty[index] = false;
            }

            return cachedValues[index];
        }

        private float Recalculate(ModifiableStat stat, float baseValue)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            RecalculationCount++;
#endif
            float flat = 0f;
            float percentAdd = 0f;
            float percentMult = 1f;

            for (int i = 0; i < modifiers.Count; i++)
            {
                StatModifier modifier = modifiers[i];
                if (modifier.Stat != stat)
                    continue;

                switch (modifier.Type)
                {
                    case StatModifierType.Flat:
                        flat += modifier.Value;
                        break;
                    case StatModifierType.PercentAdd:
                        percentAdd += modifier.Value;
                        break;
                    case StatModifierType.PercentMult:
                        percentMult *= 1f + modifier.Value;
                        break;
                }
            }

            return (baseValue + flat) * (1f + percentAdd) * percentMult;
        }

        private void MarkAllDirty()
        {
            for (int i = 0; i < dirty.Length; i++)
                dirty[i] = true;
        }
    }
}
