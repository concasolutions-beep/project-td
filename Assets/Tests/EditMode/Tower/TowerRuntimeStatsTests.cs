using System.Linq;
using System.Reflection;
using NUnit.Framework;
using ProjectTD.Projectile;
using ProjectTD.Stats;
using ProjectTD.Tower;
using UnityEditor;
using UnityEngine;

namespace ProjectTD.Tests.Tower
{
    public class TowerRuntimeStatsTests
    {
        private class FakeSource { }

        private TowerData towerData;
        private ProjectileData projectileData;

        [SetUp]
        public void SetUp()
        {
            projectileData = ScriptableObject.CreateInstance<ProjectileData>();
            projectileData.damage = 4f;

            // I campi di TowerData sono privati: si impostano come farebbe l'Inspector.
            towerData = ScriptableObject.CreateInstance<TowerData>();
            var serialized = new SerializedObject(towerData);
            serialized.FindProperty("range").floatValue = 3f;
            serialized.FindProperty("fireRate").floatValue = 2f;
            serialized.FindProperty("cost").intValue = 50;
            serialized.FindProperty("projectileData").objectReferenceValue = projectileData;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(towerData);
            Object.DestroyImmediate(projectileData);
        }

        [Test]
        public void Constructor_CopiesBaseValuesFromAsset()
        {
            var stats = new TowerRuntimeStats(towerData);

            Assert.AreEqual(4f, stats.Damage);
            Assert.AreEqual(3f, stats.Range);
            Assert.AreEqual(2f, stats.FireRate);
        }

        [Test]
        public void Constructor_NullData_Throws()
        {
            Assert.Throws<System.ArgumentNullException>(() => new TowerRuntimeStats(null));
        }

        [Test]
        public void Modifiers_DoNotChangeTheAsset()
        {
            var stats = new TowerRuntimeStats(towerData);
            var source = new FakeSource();

            stats.Stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 6f, source));
            stats.Stats.AddModifier(new StatModifier(ModifiableStat.Range, StatModifierType.PercentAdd, 0.5f, source));
            stats.Stats.AddModifier(new StatModifier(ModifiableStat.FireRate, StatModifierType.PercentMult, 1f, source));

            Assert.AreEqual(10f, stats.Damage);
            Assert.AreEqual(4.5f, stats.Range);
            Assert.AreEqual(4f, stats.FireRate);

            Assert.AreEqual(4f, towerData.Damage);
            Assert.AreEqual(4f, projectileData.damage);
            Assert.AreEqual(3f, towerData.Range);
            Assert.AreEqual(2f, towerData.FireRate);
        }

        [Test]
        public void TwoTowersFromSameAsset_AreIndependent()
        {
            var first = new TowerRuntimeStats(towerData);
            var second = new TowerRuntimeStats(towerData);

            first.Stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 6f, new FakeSource()));

            Assert.AreEqual(10f, first.Damage);
            Assert.AreEqual(4f, second.Damage);
        }

        [Test]
        public void ResetModifiers_RestoresAssetValues()
        {
            var stats = new TowerRuntimeStats(towerData);
            stats.Stats.AddModifier(new StatModifier(ModifiableStat.Damage, StatModifierType.Flat, 6f, new FakeSource()));

            stats.ResetModifiers();

            Assert.AreEqual(4f, stats.Damage);
        }

        [Test]
        public void TowerData_ExposesNoPublicWriteAccess()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

            string[] writableFields = typeof(TowerData).GetFields(flags)
                .Where(f => !f.IsInitOnly)
                .Select(f => f.Name)
                .ToArray();
            string[] writableProperties = typeof(TowerData).GetProperties(flags)
                .Where(p => p.SetMethod != null && p.SetMethod.IsPublic)
                .Select(p => p.Name)
                .ToArray();

            Assert.IsEmpty(writableFields, "Campi pubblici scrivibili su TowerData");
            Assert.IsEmpty(writableProperties, "Proprietà con setter pubblico su TowerData");
        }
    }
}
