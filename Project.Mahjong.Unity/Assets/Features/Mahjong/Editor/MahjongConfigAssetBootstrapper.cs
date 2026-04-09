using ProjectMahjong.Features.Mahjong.Data.Configs;
using UnityEditor;
using UnityEngine;

namespace ProjectMahjong.Features.Mahjong.Editor
{
    public static class MahjongConfigAssetBootstrapper
    {
        private const string RootFolder = "Assets/Features/Mahjong/Data/Configs/Assets";

        [MenuItem("Project Mahjong/Configs/Generate Default Config Assets")]
        public static void GenerateDefaultConfigAssets()
        {
            EnsureFolder("Assets/Features");
            EnsureFolder("Assets/Features/Mahjong");
            EnsureFolder("Assets/Features/Mahjong/Data");
            EnsureFolder("Assets/Features/Mahjong/Data/Configs");
            EnsureFolder(RootFolder);

            var rules2P = CreateOrLoadAsset<RuleSetConfig>($"{RootFolder}/Rules_2P_Default.asset");
            var rules3P = CreateOrLoadAsset<RuleSetConfig>($"{RootFolder}/Rules_3P_Default.asset");
            var rules4P = CreateOrLoadAsset<RuleSetConfig>($"{RootFolder}/Rules_4P_Default.asset");
            var full136TileSet = CreateOrLoadAsset<TileSetDefinition>($"{RootFolder}/TileSet_Full136.asset");
            var removeCharactersTileSet = CreateOrLoadAsset<TileSetDefinition>($"{RootFolder}/TileSet_RemoveCharacters.asset");

            ConfigureRules2P(rules2P);
            ConfigureRules3P(rules3P);
            ConfigureRules4P(rules4P);
            ConfigureTileSetFull136(full136TileSet);
            ConfigureTileSetRemoveCharacters(removeCharactersTileSet);

            var scoring = CreateOrLoadAsset<ScoringConfig>($"{RootFolder}/Scoring_Default_Simple.asset");
            ConfigureScoringDefault(scoring);

            var aiEasy = CreateOrLoadAsset<AiTuningConfig>($"{RootFolder}/AI_Easy_Default.asset");
            var aiMedium = CreateOrLoadAsset<AiTuningConfig>($"{RootFolder}/AI_Medium_Default.asset");
            var aiHard = CreateOrLoadAsset<AiTuningConfig>($"{RootFolder}/AI_Hard_Default.asset");

            ConfigureAiEasy(aiEasy);
            ConfigureAiMedium(aiMedium);
            ConfigureAiHard(aiHard);

            var matchSet = CreateOrLoadAsset<MatchConfigSet>($"{RootFolder}/MatchConfigSet_Default.asset");
            ConfigureDefaultMatchSet(matchSet, rules4P, scoring, aiEasy, aiMedium, aiHard);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Mahjong default config assets generated in: {RootFolder}");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parentPath = path[..path.LastIndexOf('/')];
            var folderName = path[(path.LastIndexOf('/') + 1)..];
            AssetDatabase.CreateFolder(parentPath, folderName);
        }

        private static T CreateOrLoadAsset<T>(string assetPath) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existing != null)
            {
                return existing;
            }

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            return asset;
        }

        private static void ConfigureRules2P(RuleSetConfig asset)
        {
            var so = new SerializedObject(asset);
            SetString(so, "_ruleSetId", "rules.2p.default");
            SetString(so, "_displayName", "2P Default Rules");
            SetInt(so, "_supportedPlayerCount", 2);
            SetEnum(so, "_tileSetMode", (int)TileSetMode.RemoveCharacters);
            SetEnum(so, "_customIncludedSuits", (int)SuitFlags.All);
            SetInt(so, "_wallTileCountOverride", 72);
            SetBool(so, "_allowChow", false);
            SetBool(so, "_allowPong", true);
            SetBool(so, "_allowKong", true);
            SetCallPriorityDefault(so);
            SetInt(so, "_startingHandTileCount", 13);
            SetInt(so, "_drawPerTurn", 1);
            SetBool(so, "_enableReactionWindow", true);
            SetInt(so, "_reactionWindowMs", 3500);
            SetEnum(so, "_winPatternProfile", (int)WinPatternProfile.Standard4Sets1Pair);
            SetBool(so, "_allowSelfDrawBonus", true);
            SetString(so, "_notes", "2P profile with Chow disabled and reduced wall.");
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureRules3P(RuleSetConfig asset)
        {
            var so = new SerializedObject(asset);
            SetString(so, "_ruleSetId", "rules.3p.default");
            SetString(so, "_displayName", "3P Default Rules");
            SetInt(so, "_supportedPlayerCount", 3);
            SetEnum(so, "_tileSetMode", (int)TileSetMode.RemoveCharacters);
            SetEnum(so, "_customIncludedSuits", (int)SuitFlags.All);
            SetInt(so, "_wallTileCountOverride", 0);
            SetBool(so, "_allowChow", true);
            SetBool(so, "_allowPong", true);
            SetBool(so, "_allowKong", true);
            SetCallPriorityDefault(so);
            SetInt(so, "_startingHandTileCount", 13);
            SetInt(so, "_drawPerTurn", 1);
            SetBool(so, "_enableReactionWindow", true);
            SetInt(so, "_reactionWindowMs", 4000);
            SetEnum(so, "_winPatternProfile", (int)WinPatternProfile.Standard4Sets1Pair);
            SetBool(so, "_allowSelfDrawBonus", true);
            SetString(so, "_notes", "3P profile with one suit removed.");
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureRules4P(RuleSetConfig asset)
        {
            var so = new SerializedObject(asset);
            SetString(so, "_ruleSetId", "rules.4p.default");
            SetString(so, "_displayName", "4P Default Rules");
            SetInt(so, "_supportedPlayerCount", 4);
            SetEnum(so, "_tileSetMode", (int)TileSetMode.Full136);
            SetEnum(so, "_customIncludedSuits", (int)SuitFlags.All);
            SetInt(so, "_wallTileCountOverride", 0);
            SetBool(so, "_allowChow", true);
            SetBool(so, "_allowPong", true);
            SetBool(so, "_allowKong", true);
            SetCallPriorityDefault(so);
            SetInt(so, "_startingHandTileCount", 13);
            SetInt(so, "_drawPerTurn", 1);
            SetBool(so, "_enableReactionWindow", true);
            SetInt(so, "_reactionWindowMs", 4000);
            SetEnum(so, "_winPatternProfile", (int)WinPatternProfile.Standard4Sets1Pair);
            SetBool(so, "_allowSelfDrawBonus", true);
            SetString(so, "_notes", "Baseline 4P rules.");
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureScoringDefault(ScoringConfig asset)
        {
            var so = new SerializedObject(asset);
            SetString(so, "_scoringProfileId", "scoring.simple.default");
            SetString(so, "_displayName", "Simple Default Scoring");
            SetInt(so, "_baseWinPoints", 10);
            SetInt(so, "_selfDrawBonusPoints", 2);
            SetInt(so, "_kongBonusPoints", 1);
            SetInt(so, "_clampMinScore", 0);
            SetInt(so, "_clampMaxScore", 9999);
            SetBool(so, "_roundEndOnFirstWin", true);
            SetString(so, "_notes", "MVP scoring profile.");
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureTileSetFull136(TileSetDefinition asset)
        {
            var so = new SerializedObject(asset);
            SetString(so, "_tileSetId", "tileset.full136");
            SetString(so, "_displayName", "Full 136 Tile Set");
            SetBool(so, "_includeDots", true);
            SetBool(so, "_includeBamboo", true);
            SetBool(so, "_includeCharacters", true);
            SetBool(so, "_includeWinds", true);
            SetBool(so, "_includeDragons", true);
            SetInt(so, "_copiesPerTile", 4);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureTileSetRemoveCharacters(TileSetDefinition asset)
        {
            var so = new SerializedObject(asset);
            SetString(so, "_tileSetId", "tileset.remove_characters");
            SetString(so, "_displayName", "Tile Set (No Characters)");
            SetBool(so, "_includeDots", true);
            SetBool(so, "_includeBamboo", true);
            SetBool(so, "_includeCharacters", false);
            SetBool(so, "_includeWinds", true);
            SetBool(so, "_includeDragons", true);
            SetInt(so, "_copiesPerTile", 4);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureAiEasy(AiTuningConfig asset)
        {
            var so = new SerializedObject(asset);
            SetString(so, "_aiProfileId", "ai.easy.default");
            SetString(so, "_displayName", "AI Easy");
            SetEnum(so, "_difficulty", (int)DifficultyLevel.Easy);
            SetEnum(so, "_behaviorStyle", (int)AiBehaviorStyle.Balanced);
            SetFloat(so, "_discardRandomness", 0.7f);
            SetFloat(so, "_offenseWeight", 0.8f);
            SetFloat(so, "_defenseWeight", 0.6f);
            SetFloat(so, "_callAggression", 0.3f);
            SetFloat(so, "_riskTolerance", 0.6f);
            SetInt(so, "_lookaheadDepth", 0);
            SetBool(so, "_tileTrackingEnabled", false);
            SetInt(so, "_reactionDelayMinMs", 500);
            SetInt(so, "_reactionDelayMaxMs", 1100);
            SetString(so, "_notes", "Beginner-friendly AI.");
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureAiMedium(AiTuningConfig asset)
        {
            var so = new SerializedObject(asset);
            SetString(so, "_aiProfileId", "ai.medium.default");
            SetString(so, "_displayName", "AI Medium");
            SetEnum(so, "_difficulty", (int)DifficultyLevel.Medium);
            SetEnum(so, "_behaviorStyle", (int)AiBehaviorStyle.Balanced);
            SetFloat(so, "_discardRandomness", 0.35f);
            SetFloat(so, "_offenseWeight", 1f);
            SetFloat(so, "_defenseWeight", 1f);
            SetFloat(so, "_callAggression", 0.5f);
            SetFloat(so, "_riskTolerance", 0.45f);
            SetInt(so, "_lookaheadDepth", 1);
            SetBool(so, "_tileTrackingEnabled", true);
            SetInt(so, "_reactionDelayMinMs", 400);
            SetInt(so, "_reactionDelayMaxMs", 900);
            SetString(so, "_notes", "Baseline medium AI.");
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureAiHard(AiTuningConfig asset)
        {
            var so = new SerializedObject(asset);
            SetString(so, "_aiProfileId", "ai.hard.default");
            SetString(so, "_displayName", "AI Hard");
            SetEnum(so, "_difficulty", (int)DifficultyLevel.Hard);
            SetEnum(so, "_behaviorStyle", (int)AiBehaviorStyle.Defensive);
            SetFloat(so, "_discardRandomness", 0.1f);
            SetFloat(so, "_offenseWeight", 1.2f);
            SetFloat(so, "_defenseWeight", 1.4f);
            SetFloat(so, "_callAggression", 0.55f);
            SetFloat(so, "_riskTolerance", 0.25f);
            SetInt(so, "_lookaheadDepth", 2);
            SetBool(so, "_tileTrackingEnabled", true);
            SetInt(so, "_reactionDelayMinMs", 300);
            SetInt(so, "_reactionDelayMaxMs", 700);
            SetString(so, "_notes", "Stronger defensive AI.");
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void ConfigureDefaultMatchSet(
            MatchConfigSet asset,
            RuleSetConfig rules,
            ScoringConfig scoring,
            AiTuningConfig aiEasy,
            AiTuningConfig aiMedium,
            AiTuningConfig aiHard)
        {
            var so = new SerializedObject(asset);
            SetString(so, "_configSetId", "match.default");
            SetString(so, "_displayName", "Default Match Config Set");
            SetObjectReference(so, "_rules", rules);
            SetObjectReference(so, "_scoring", scoring);

            var aiProfiles = so.FindProperty("_aiProfilesBySeatOrDifficulty");
            aiProfiles.arraySize = 3;
            aiProfiles.GetArrayElementAtIndex(0).objectReferenceValue = aiEasy;
            aiProfiles.GetArrayElementAtIndex(1).objectReferenceValue = aiMedium;
            aiProfiles.GetArrayElementAtIndex(2).objectReferenceValue = aiHard;

            SetString(so, "_notes", "Default config bundle for gameplay bootstrap.");
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
        }

        private static void SetCallPriorityDefault(SerializedObject so)
        {
            var arr = so.FindProperty("_callPriority");
            arr.arraySize = 4;
            arr.GetArrayElementAtIndex(0).intValue = (int)CallType.Win;
            arr.GetArrayElementAtIndex(1).intValue = (int)CallType.Kong;
            arr.GetArrayElementAtIndex(2).intValue = (int)CallType.Pong;
            arr.GetArrayElementAtIndex(3).intValue = (int)CallType.Chow;
        }

        private static void SetString(SerializedObject so, string propertyName, string value)
        {
            so.FindProperty(propertyName).stringValue = value;
        }

        private static void SetInt(SerializedObject so, string propertyName, int value)
        {
            so.FindProperty(propertyName).intValue = value;
        }

        private static void SetFloat(SerializedObject so, string propertyName, float value)
        {
            so.FindProperty(propertyName).floatValue = value;
        }

        private static void SetBool(SerializedObject so, string propertyName, bool value)
        {
            so.FindProperty(propertyName).boolValue = value;
        }

        private static void SetEnum(SerializedObject so, string propertyName, int value)
        {
            // Use raw enum int value so flags/non-sequential enums remain valid.
            so.FindProperty(propertyName).intValue = value;
        }

        private static void SetObjectReference(SerializedObject so, string propertyName, Object value)
        {
            so.FindProperty(propertyName).objectReferenceValue = value;
        }
    }
}

