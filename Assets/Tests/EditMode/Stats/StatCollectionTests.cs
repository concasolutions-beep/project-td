using NUnit.Framework;
using ProjectTD.Stats;

namespace ProjectTD.Tests.Stats
{
    public class StatCollectionTests
    {
        private class FakeSource { }

        private static StatCollection CreateCollection()
        {
            var stats = new StatCollection();
            stats.SetBaseValue(ModifiableStat.Damage, 10f);
            stats.SetBaseValue(ModifiableStat.FireRate, 2f);
            return stats;
        }

        [Test]
        public void GetValue_NoModifiers_ReturnsBase()
        {
            Assert.AreEqual(10f, CreateCollection().GetValue(ModifiableStat.Damage));
        }

        [Test]
        public void GetValue_CombinesFlatPercentAddAndPercentMult()
        {
            var source = new FakeSource();
            var stats = CreateCollection();
            stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 5f, source));
            stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.PercentAdd, 0.1f, source));
            stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.PercentAdd, 0.2f, source));
            stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.PercentMult, 0.1f, source));
            stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.PercentMult, 0.2f, source));

            // (10 + 5) × (1 + 0.3) × 1.1 × 1.2
            Assert.AreEqual(25.74f, stats.GetValue(ModifiableStat.Damage), 0.0001f);
        }

        [Test]
        public void GetValue_IgnoresModifiersOfOtherStats()
        {
            var stats = CreateCollection();
            stats.AddModifier(new StatModifier(ModifiableStat.Range, StatModifierType.Flat, 99f, new FakeSource()));

            Assert.AreEqual(10f, stats.GetValue(ModifiableStat.Damage));
        }

        [Test]
        public void RemoveAllFromSource_RemovesOnlyThatInstance()
        {
            var sold = new FakeSource();
            var other = new FakeSource();
            var stats = CreateCollection();
            stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 5f, sold));
            stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 3f, other));

            Assert.AreEqual(1, stats.RemoveAllFromSource(sold));
            Assert.AreEqual(13f, stats.GetValue(ModifiableStat.Damage));
        }

        [Test]
        public void GetValue_RepeatedReads_RecalculatesOnlyOnce()
        {
            var stats = CreateCollection();

            for (int frame = 0; frame < 1000; frame++)
                stats.GetValue(ModifiableStat.Damage);

            Assert.AreEqual(1, stats.RecalculationCount);
        }

        [Test]
        public void GetValue_RecalculatesOnlyWhenModifiersChange()
        {
            var source = new FakeSource();
            var stats = CreateCollection();
            stats.GetValue(ModifiableStat.Damage);
            Assert.AreEqual(1, stats.RecalculationCount);

            stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 5f, source));
            for (int frame = 0; frame < 100; frame++)
                stats.GetValue(ModifiableStat.Damage);
            Assert.AreEqual(2, stats.RecalculationCount);

            stats.RemoveAllFromSource(source);
            for (int frame = 0; frame < 100; frame++)
                stats.GetValue(ModifiableStat.Damage);
            Assert.AreEqual(3, stats.RecalculationCount);
        }

        [Test]
        public void NoOpChanges_DoNotMarkDirty()
        {
            var stats = CreateCollection();
            stats.GetValue(ModifiableStat.Damage);

            stats.RemoveAllFromSource(new FakeSource());
            stats.ClearModifiers();
            stats.SetBaseValue(ModifiableStat.Damage, 10f);
            stats.GetValue(ModifiableStat.Damage);

            Assert.AreEqual(1, stats.RecalculationCount);
        }

        [Test]
        public void ModifierOnOneStat_DoesNotInvalidateOthers()
        {
            var stats = CreateCollection();
            stats.GetValue(ModifiableStat.Damage);
            stats.GetValue(ModifiableStat.FireRate);
            int before = stats.RecalculationCount;

            stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 1f, new FakeSource()));
            stats.GetValue(ModifiableStat.FireRate);

            Assert.AreEqual(before, stats.RecalculationCount);
        }
    }
}
