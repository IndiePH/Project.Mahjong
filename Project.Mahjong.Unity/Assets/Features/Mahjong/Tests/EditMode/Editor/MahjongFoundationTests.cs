using System.Collections.Generic;
using NUnit.Framework;
using ProjectMahjong.Features.Mahjong.Data.Configs;
using ProjectMahjong.Features.Mahjong.Runtime.Gameplay.Dealing;
using UnityEditor;
using UnityEngine;

namespace ProjectMahjong.Features.Mahjong.Tests.EditMode
{
    public sealed class MahjongFoundationTests
    {
        [Test]
        public void HandValidator_ReturnsTrue_ForStandardWinningHand()
        {
            var tiles = new List<MahjongTile>
            {
                T(TileSuit.Dots, 1), T(TileSuit.Dots, 1), T(TileSuit.Dots, 1),
                T(TileSuit.Dots, 2), T(TileSuit.Dots, 2), T(TileSuit.Dots, 2),
                T(TileSuit.Dots, 3), T(TileSuit.Dots, 3), T(TileSuit.Dots, 3),
                T(TileSuit.Bamboo, 4), T(TileSuit.Bamboo, 5), T(TileSuit.Bamboo, 6),
                T(TileSuit.Characters, 9), T(TileSuit.Characters, 9)
            };

            Assert.IsTrue(HandValidator.IsStandardWinningHand(tiles));
        }

        [Test]
        public void HandValidator_ReturnsFalse_ForInvalidHand()
        {
            var tiles = new List<MahjongTile>
            {
                T(TileSuit.Dots, 1), T(TileSuit.Dots, 1),
                T(TileSuit.Dots, 2), T(TileSuit.Dots, 2),
                T(TileSuit.Dots, 3), T(TileSuit.Dots, 4),
                T(TileSuit.Bamboo, 1), T(TileSuit.Bamboo, 2), T(TileSuit.Bamboo, 3),
                T(TileSuit.Characters, 4), T(TileSuit.Characters, 5), T(TileSuit.Characters, 6),
                T(TileSuit.Honor, 0, HonorType.East), T(TileSuit.Honor, 0, HonorType.West)
            };

            Assert.IsFalse(HandValidator.IsStandardWinningHand(tiles));
        }

        [Test]
        public void WallBuilder_Builds136_ForDefaultFullTileSet()
        {
            var rules = ScriptableObject.CreateInstance<RuleSetConfig>();
            var full = ScriptableObject.CreateInstance<TileSetDefinition>();
            var noChar = ScriptableObject.CreateInstance<TileSetDefinition>();

            var ok = WallBuilder.TryBuildShuffledWall(rules, full, noChar, seed: 123, out var wall, out var error);

            Assert.IsTrue(ok, error);
            Assert.AreEqual(136, wall.Count);
        }

        [Test]
        public void Dealer_DealsExpectedHandsAndRemainingWall()
        {
            var rules = ScriptableObject.CreateInstance<RuleSetConfig>();
            var full = ScriptableObject.CreateInstance<TileSetDefinition>();
            var noChar = ScriptableObject.CreateInstance<TileSetDefinition>();

            var built = WallBuilder.TryBuildShuffledWall(rules, full, noChar, seed: 123, out var wall, out var buildError);
            Assert.IsTrue(built, buildError);

            var dealt = Dealer.TryDealStartingHands(
                wall,
                seatCount: 4,
                startingHandTileCount: rules.StartingHandTileCount,
                out var dealResult,
                out var dealError);

            Assert.IsTrue(dealt, dealError);
            Assert.AreEqual(13, dealResult.Hands[0].Count);
            Assert.AreEqual(13, dealResult.Hands[1].Count);
            Assert.AreEqual(13, dealResult.Hands[2].Count);
            Assert.AreEqual(13, dealResult.Hands[3].Count);
            Assert.AreEqual(52, dealResult.TilesConsumedFromWall);
            Assert.AreEqual(84, wall.Count - dealResult.TilesConsumedFromWall);
        }

        [Test]
        public void TurnLoop_ResolvesReaction_ByPriority()
        {
            var rules = ScriptableObject.CreateInstance<RuleSetConfig>();
            SetRuleBool(rules, "_allowChow", false);
            SetRuleBool(rules, "_allowKong", false);
            SetRuleBool(rules, "_allowPong", true);
            SetRuleBool(rules, "_enableReactionWindow", true);
            SetRuleInt(rules, "_reactionWindowMs", 1000);

            var wall = new List<MahjongTile> { T(TileSuit.Dots, 5) };
            var hands = new IReadOnlyList<MahjongTile>[]
            {
                new List<MahjongTile> { T(TileSuit.Bamboo, 1) },
                new List<MahjongTile> { T(TileSuit.Dots, 5), T(TileSuit.Dots, 5) }
            };
            var deal = new DealResult(hands, 0);

            var ok = Dealer.TrySimulateTurnLoop(
                wall,
                initialWallIndex: 0,
                deal,
                rules,
                drawPerTurn: 1,
                maxTurns: 1,
                out var result,
                out var error);

            Assert.IsTrue(ok, error);
            Assert.AreEqual(1, result.CallSummary.PongCount);
            Assert.AreEqual(0, result.CallSummary.ChowCount);
            Assert.AreEqual(true, result.HasLastResolvedCall);
            Assert.AreEqual(CallType.Pong, result.LastResolvedCall);
        }

        [Test]
        public void TurnLoop_ReactionWindow_TimesOut_WhenConfiguredTooLow()
        {
            var rules = ScriptableObject.CreateInstance<RuleSetConfig>();
            SetRuleBool(rules, "_allowChow", false);
            SetRuleBool(rules, "_allowKong", false);
            SetRuleBool(rules, "_allowPong", true);
            SetRuleBool(rules, "_enableReactionWindow", true);
            SetRuleInt(rules, "_reactionWindowMs", 100);

            var wall = new List<MahjongTile> { T(TileSuit.Dots, 7) };
            var hands = new IReadOnlyList<MahjongTile>[]
            {
                new List<MahjongTile> { T(TileSuit.Bamboo, 2) },
                new List<MahjongTile> { T(TileSuit.Dots, 7), T(TileSuit.Dots, 7) }
            };
            var deal = new DealResult(hands, 0);

            var ok = Dealer.TrySimulateTurnLoop(
                wall,
                initialWallIndex: 0,
                deal,
                rules,
                drawPerTurn: 1,
                maxTurns: 1,
                out var result,
                out var error);

            Assert.IsTrue(ok, error);
            Assert.AreEqual(0, result.CallSummary.PongCount);
            Assert.AreEqual(1, result.ReactionWindowSummary.OpenedCount);
            Assert.AreEqual(1, result.ReactionWindowSummary.TimedOutCount);
            Assert.AreEqual(0, result.ReactionWindowSummary.AcceptedCount);
        }

        private static MahjongTile T(TileSuit suit, int rank, HonorType honor = HonorType.None, int tileId = 0)
        {
            return new MahjongTile(tileId, suit, rank, honor);
        }

        private static void SetRuleBool(RuleSetConfig rules, string fieldName, bool value)
        {
            var so = new SerializedObject(rules);
            so.FindProperty(fieldName).boolValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetRuleInt(RuleSetConfig rules, string fieldName, int value)
        {
            var so = new SerializedObject(rules);
            so.FindProperty(fieldName).intValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}

