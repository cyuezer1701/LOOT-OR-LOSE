using System;
using System.Collections.Generic;
using NUnit.Framework;
using LootOrLose.Core.Items;
using LootOrLose.Core.Scoring;
using LootOrLose.Data;
using LootOrLose.Enums;

namespace LootOrLose.Tests.EditMode
{
    [TestFixture]
    public class ScoreCalculatorTests
    {
        // ─────────────────────────────────────────────
        //  Round Scoring
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateRoundScore_BasicScoring_10PointsPerRound()
        {
            Assert.AreEqual(100, ScoreCalculator.CalculateRoundScore(10));
            Assert.AreEqual(250, ScoreCalculator.CalculateRoundScore(25));
            Assert.AreEqual(500, ScoreCalculator.CalculateRoundScore(50));
        }

        [Test]
        public void CalculateRoundScore_ZeroRounds_ReturnsZero()
        {
            Assert.AreEqual(0, ScoreCalculator.CalculateRoundScore(0));
        }

        [Test]
        public void CalculateRoundScore_NegativeRounds_ClampsToZero()
        {
            Assert.AreEqual(0, ScoreCalculator.CalculateRoundScore(-5));
        }

        // ─────────────────────────────────────────────
        //  Boss Scoring
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateBossScore_500PointsPerBoss()
        {
            Assert.AreEqual(500, ScoreCalculator.CalculateBossScore(1));
            Assert.AreEqual(1500, ScoreCalculator.CalculateBossScore(3));
        }

        [Test]
        public void CalculateBossScore_ZeroBosses_ReturnsZero()
        {
            Assert.AreEqual(0, ScoreCalculator.CalculateBossScore(0));
        }

        [Test]
        public void CalculateBossScore_NegativeBosses_ClampsToZero()
        {
            Assert.AreEqual(0, ScoreCalculator.CalculateBossScore(-2));
        }

        // ─────────────────────────────────────────────
        //  Synergy Scoring
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateSynergyScore_100PointsPerSynergy()
        {
            var synergies = new List<SynergyResult>
            {
                new SynergyResult { type = SynergyType.ArmorSet },
                new SynergyResult { type = SynergyType.DualWield },
                new SynergyResult { type = SynergyType.PotionMaster }
            };

            Assert.AreEqual(300, ScoreCalculator.CalculateSynergyScore(synergies));
        }

        [Test]
        public void CalculateSynergyScore_NullSynergies_ReturnsZero()
        {
            Assert.AreEqual(0, ScoreCalculator.CalculateSynergyScore(null));
        }

        [Test]
        public void CalculateSynergyScore_EmptyList_ReturnsZero()
        {
            Assert.AreEqual(0, ScoreCalculator.CalculateSynergyScore(new List<SynergyResult>()));
        }

        // ─────────────────────────────────────────────
        //  Rarity Scoring
        // ─────────────────────────────────────────────

        [Test]
        public void GetRarityValue_ReturnsCorrectValues()
        {
            Assert.AreEqual(10, ScoreCalculator.GetRarityValue(ItemRarity.Common));
            Assert.AreEqual(25, ScoreCalculator.GetRarityValue(ItemRarity.Uncommon));
            Assert.AreEqual(50, ScoreCalculator.GetRarityValue(ItemRarity.Rare));
            Assert.AreEqual(100, ScoreCalculator.GetRarityValue(ItemRarity.Legendary));
        }

        [Test]
        public void CalculateRarityScore_SumsAllItemRarities()
        {
            var inventory = new List<ItemData>
            {
                new ItemData { rarity = ItemRarity.Common },    // 10
                new ItemData { rarity = ItemRarity.Uncommon },  // 25
                new ItemData { rarity = ItemRarity.Rare },      // 50
                new ItemData { rarity = ItemRarity.Legendary }  // 100
            };

            Assert.AreEqual(185, ScoreCalculator.CalculateRarityScore(inventory));
        }

        [Test]
        public void CalculateRarityScore_NullInventory_ReturnsZero()
        {
            Assert.AreEqual(0, ScoreCalculator.CalculateRarityScore(null));
        }

        [Test]
        public void CalculateRarityScore_EmptyInventory_ReturnsZero()
        {
            Assert.AreEqual(0, ScoreCalculator.CalculateRarityScore(new List<ItemData>()));
        }

        // ─────────────────────────────────────────────
        //  Streak Multiplier
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateStreakMultiplier_ZeroStreak_Returns1()
        {
            Assert.AreEqual(1.0f, ScoreCalculator.CalculateStreakMultiplier(0));
        }

        [Test]
        public void CalculateStreakMultiplier_IncreasesBy0Point1PerStreak()
        {
            Assert.AreEqual(1.1f, ScoreCalculator.CalculateStreakMultiplier(1), 0.001f);
            Assert.AreEqual(1.5f, ScoreCalculator.CalculateStreakMultiplier(5), 0.001f);
        }

        [Test]
        public void CalculateStreakMultiplier_CappedAt2Point0()
        {
            Assert.AreEqual(2.0f, ScoreCalculator.CalculateStreakMultiplier(10), 0.001f);
            Assert.AreEqual(2.0f, ScoreCalculator.CalculateStreakMultiplier(20), 0.001f);
            Assert.AreEqual(2.0f, ScoreCalculator.CalculateStreakMultiplier(100), 0.001f);
        }

        [Test]
        public void CalculateStreakMultiplier_NegativeStreak_ClampsToBase()
        {
            Assert.AreEqual(1.0f, ScoreCalculator.CalculateStreakMultiplier(-3), 0.001f);
        }

        // ─────────────────────────────────────────────
        //  Total Score Calculation
        // ─────────────────────────────────────────────

        [Test]
        public void CalculateRunScore_CombinesAllComponents()
        {
            var input = new RunScoreInput
            {
                roundsCompleted = 20,      // 200 pts
                bossesDefeated = 1,        // 500 pts
                finalInventory = new List<ItemData>
                {
                    new ItemData { rarity = ItemRarity.Common },     // 10 pts
                    new ItemData { rarity = ItemRarity.Rare }        // 50 pts
                },
                synergies = new List<SynergyResult>
                {
                    new SynergyResult { type = SynergyType.DualWield }  // 100 pts
                },
                currentStreak = 0,
                isDailyRun = false
            };

            var result = ScoreCalculator.CalculateRunScore(input);

            Assert.AreEqual(200, result.roundScore);
            Assert.AreEqual(500, result.bossScore);
            Assert.AreEqual(100, result.synergyScore);
            Assert.AreEqual(60, result.rarityScore);
            Assert.AreEqual(1.0f, result.streakMultiplier, 0.001f);
            Assert.AreEqual(860, result.totalScore);
        }

        [Test]
        public void CalculateRunScore_WithStreakMultiplier_AppliesCorrectly()
        {
            var input = new RunScoreInput
            {
                roundsCompleted = 10,       // 100 pts
                bossesDefeated = 0,         // 0 pts
                finalInventory = new List<ItemData>(),
                synergies = new List<SynergyResult>(),
                currentStreak = 5,          // 1.5x multiplier
                isDailyRun = false
            };

            var result = ScoreCalculator.CalculateRunScore(input);

            Assert.AreEqual(100, result.roundScore);
            Assert.AreEqual(1.5f, result.streakMultiplier, 0.001f);
            // (100 + 0 + 0 + 0) * 1.5 = 150
            Assert.AreEqual(150, result.totalScore);
        }

        [Test]
        public void CalculateRunScore_ZeroEverything_ReturnsZero()
        {
            var input = new RunScoreInput
            {
                roundsCompleted = 0,
                bossesDefeated = 0,
                finalInventory = new List<ItemData>(),
                synergies = new List<SynergyResult>(),
                currentStreak = 0,
                isDailyRun = false
            };

            var result = ScoreCalculator.CalculateRunScore(input);

            Assert.AreEqual(0, result.totalScore);
        }

        [Test]
        public void CalculateRunScore_NullInventoryAndSynergies_HandlesGracefully()
        {
            var input = new RunScoreInput
            {
                roundsCompleted = 5,
                bossesDefeated = 0,
                finalInventory = null,
                synergies = null,
                currentStreak = 0,
                isDailyRun = false
            };

            var result = ScoreCalculator.CalculateRunScore(input);

            Assert.AreEqual(50, result.roundScore);
            Assert.AreEqual(0, result.rarityScore);
            Assert.AreEqual(0, result.synergyScore);
            Assert.AreEqual(50, result.totalScore);
        }

        [Test]
        public void CalculateRunScore_MaxStreakMultiplier_DoublesScore()
        {
            var input = new RunScoreInput
            {
                roundsCompleted = 50,      // 500 pts
                bossesDefeated = 3,        // 1500 pts
                finalInventory = new List<ItemData>
                {
                    new ItemData { rarity = ItemRarity.Legendary }  // 100 pts
                },
                synergies = new List<SynergyResult>
                {
                    new SynergyResult { type = SynergyType.ArmorSet },
                    new SynergyResult { type = SynergyType.DualWield }
                },                         // 200 pts
                currentStreak = 15,        // capped at 2.0x
                isDailyRun = false
            };

            var result = ScoreCalculator.CalculateRunScore(input);

            int subtotal = 500 + 1500 + 100 + 200; // 2300
            Assert.AreEqual(2.0f, result.streakMultiplier, 0.001f);
            Assert.AreEqual((int)(subtotal * 2.0f), result.totalScore);
        }
    }
}
