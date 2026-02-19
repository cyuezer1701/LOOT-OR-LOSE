using System;
using System.Collections.Generic;
using NUnit.Framework;
using LootOrLose.Core.Items;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.Tests.EditMode
{
    [TestFixture]
    public class ItemGeneratorTests
    {
        private List<ItemData> testPool;
        private Random fixedRandom;

        [SetUp]
        public void SetUp()
        {
            fixedRandom = new Random(42); // Fixed seed for reproducibility
            testPool = CreateTestItemPool();
        }

        [Test]
        public void GetZoneForRound_ReturnsCorrectZones()
        {
            Assert.AreEqual(DungeonZone.Tutorial, ItemGenerator.GetZoneForRound(1));
            Assert.AreEqual(DungeonZone.Tutorial, ItemGenerator.GetZoneForRound(10));
            Assert.AreEqual(DungeonZone.Standard, ItemGenerator.GetZoneForRound(11));
            Assert.AreEqual(DungeonZone.Standard, ItemGenerator.GetZoneForRound(25));
            Assert.AreEqual(DungeonZone.Danger, ItemGenerator.GetZoneForRound(26));
            Assert.AreEqual(DungeonZone.Danger, ItemGenerator.GetZoneForRound(40));
            Assert.AreEqual(DungeonZone.Chaos, ItemGenerator.GetZoneForRound(41));
            Assert.AreEqual(DungeonZone.Chaos, ItemGenerator.GetZoneForRound(100));
        }

        [Test]
        public void IsBossRound_ReturnsTrueForBossRounds()
        {
            Assert.IsTrue(ItemGenerator.IsBossRound(15));
            Assert.IsTrue(ItemGenerator.IsBossRound(30));
            Assert.IsTrue(ItemGenerator.IsBossRound(45));
            Assert.IsFalse(ItemGenerator.IsBossRound(1));
            Assert.IsFalse(ItemGenerator.IsBossRound(14));
            Assert.IsFalse(ItemGenerator.IsBossRound(16));
            Assert.IsFalse(ItemGenerator.IsBossRound(0));
        }

        [Test]
        public void FilterByZone_Tutorial_ExcludesTrapsAndCurses()
        {
            var filtered = ItemGenerator.FilterByZone(testPool, DungeonZone.Tutorial);
            Assert.IsTrue(filtered.TrueForAll(i => i.category != ItemCategory.Trap));
            Assert.IsTrue(filtered.TrueForAll(i => i.category != ItemCategory.Curse));
        }

        [Test]
        public void FilterByZone_Tutorial_OnlyCommonAndUncommon()
        {
            var filtered = ItemGenerator.FilterByZone(testPool, DungeonZone.Tutorial);
            Assert.IsTrue(filtered.TrueForAll(i =>
                i.rarity == ItemRarity.Common || i.rarity == ItemRarity.Uncommon));
        }

        [Test]
        public void FilterByZone_Standard_AllowsAllItems()
        {
            var filtered = ItemGenerator.FilterByZone(testPool, DungeonZone.Standard);
            Assert.AreEqual(testPool.Count, filtered.Count);
        }

        [Test]
        public void GenerateItem_ReturnsItemFromPool()
        {
            var item = ItemGenerator.GenerateItem(testPool, 5, BiomeType.Crypt, DungeonZone.Tutorial, fixedRandom);
            Assert.IsNotNull(item);
            Assert.Contains(item, testPool);
        }

        [Test]
        public void GenerateItem_EmptyPool_ReturnsNull()
        {
            var item = ItemGenerator.GenerateItem(new List<ItemData>(), 5, BiomeType.Crypt, DungeonZone.Tutorial, fixedRandom);
            Assert.IsNull(item);
        }

        [Test]
        public void GenerateItem_RespectsZoneFiltering()
        {
            // Tutorial zone should never return traps or curses
            for (int i = 0; i < 100; i++)
            {
                var item = ItemGenerator.GenerateItem(testPool, 5, BiomeType.Crypt, DungeonZone.Tutorial, fixedRandom);
                if (item != null)
                {
                    Assert.AreNotEqual(ItemCategory.Trap, item.category);
                    Assert.AreNotEqual(ItemCategory.Curse, item.category);
                }
            }
        }

        [Test]
        public void SelectWeighted_WithFixedSeed_IsDeterministic()
        {
            var items = new List<string> { "a", "b", "c" };
            var weights = new float[] { 1f, 1f, 1f };

            var random1 = new Random(123);
            var random2 = new Random(123);

            var result1 = ItemGenerator.SelectWeighted(items, weights, random1);
            var result2 = ItemGenerator.SelectWeighted(items, weights, random2);

            Assert.AreEqual(result1, result2);
        }

        [Test]
        public void CalculateWeights_HigherZone_IncreasesRareWeights()
        {
            var tutorialWeights = ItemGenerator.CalculateWeights(testPool, 5, DungeonZone.Tutorial);
            var chaosWeights = ItemGenerator.CalculateWeights(testPool, 50, DungeonZone.Chaos);

            // Find a rare item index
            int rareIdx = testPool.FindIndex(i => i.rarity == ItemRarity.Rare);
            if (rareIdx >= 0)
            {
                Assert.Greater(chaosWeights[rareIdx], tutorialWeights[rareIdx]);
            }
        }

        private List<ItemData> CreateTestItemPool()
        {
            return new List<ItemData>
            {
                new ItemData { id = "sword", category = ItemCategory.Weapon, rarity = ItemRarity.Common, dropWeight = 1f, availableBiomes = new BiomeType[0] },
                new ItemData { id = "shield", category = ItemCategory.Defense, rarity = ItemRarity.Common, dropWeight = 1f, availableBiomes = new BiomeType[0] },
                new ItemData { id = "potion", category = ItemCategory.Consumable, rarity = ItemRarity.Uncommon, dropWeight = 1f, availableBiomes = new BiomeType[0] },
                new ItemData { id = "rare_staff", category = ItemCategory.Weapon, rarity = ItemRarity.Rare, dropWeight = 1f, availableBiomes = new BiomeType[0] },
                new ItemData { id = "legendary_blade", category = ItemCategory.Weapon, rarity = ItemRarity.Legendary, dropWeight = 0.5f, availableBiomes = new BiomeType[0] },
                new ItemData { id = "trap_poison", category = ItemCategory.Trap, rarity = ItemRarity.Common, dropWeight = 1f, availableBiomes = new BiomeType[0] },
                new ItemData { id = "curse_weak", category = ItemCategory.Curse, rarity = ItemRarity.Common, dropWeight = 1f, availableBiomes = new BiomeType[0] },
                new ItemData { id = "key_bronze", category = ItemCategory.Key, rarity = ItemRarity.Common, dropWeight = 1f, availableBiomes = new BiomeType[0] },
            };
        }
    }
}
