using System;
using System.Collections.Generic;
using NUnit.Framework;
using ProjectTD.Stats;

namespace ProjectTD.Tests.Stats
{
    public class StatModifierTests
    {
        // Sorgente fittizia: ogni istanza è una sorgente distinta, anche con lo stesso nome.
        private class FakeSource
        {
            private readonly string name;

            public FakeSource(string name) => this.name = name;

            public override string ToString() => name;
        }

        [Test]
        public void Constructor_AssignsAllFields()
        {
            var source = new FakeSource("Tower");

            var modifier = new StatModifier(ModifiableStat.Damage, StatModifierType.PercentAdd, 0.2f, source);

            Assert.AreEqual(ModifiableStat.Damage, modifier.Stat);
            Assert.AreEqual(StatModifierType.PercentAdd, modifier.Type);
            Assert.AreEqual(0.2f, modifier.Value);
            Assert.AreSame(source, modifier.Source);
        }

        [Test]
        public void Constructor_NullSource_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 5f, null));
        }

        [Test]
        public void RemoveBySource_RemovesOnlyModifiersOfThatInstance()
        {
            // Due torrette dello stesso tipo: vendere la prima non deve togliere i bonus della seconda.
            var soldTower = new FakeSource("Tower");
            var otherTower = new FakeSource("Tower");
            var modifiers = new List<StatModifier>
            {
                new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 5f, soldTower),
                new StatModifier(ModifiableStat.Range, StatModifierType.PercentAdd, 0.1f, soldTower),
                new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 3f, otherTower)
            };

            modifiers.RemoveAll(m => m.Source == soldTower);

            Assert.AreEqual(1, modifiers.Count);
            Assert.AreSame(otherTower, modifiers[0].Source);
        }

        [TestCase(StatModifierType.Flat, 5f, "Damage Flat +5 (Tower)")]
        [TestCase(StatModifierType.Flat, -2.5f, "Damage Flat -2.5 (Tower)")]
        [TestCase(StatModifierType.PercentAdd, 0.2f, "Damage PercentAdd +20% (Tower)")]
        [TestCase(StatModifierType.PercentMult, -0.15f, "Damage PercentMult -15% (Tower)")]
        public void ToString_FormatsValueByType(StatModifierType type, float value, string expected)
        {
            var modifier = new StatModifier(ModifiableStat.Damage, type, value, new FakeSource("Tower"));

            Assert.AreEqual(expected, modifier.ToString());
        }
    }
}
