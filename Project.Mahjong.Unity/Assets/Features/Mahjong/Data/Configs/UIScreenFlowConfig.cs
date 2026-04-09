using System;
using UnityEngine;

namespace ProjectMahjong.Features.Mahjong.Data.Configs
{
    public enum UIScreenId
    {
        MainMenu = 0,
        LobbySetup = 1,
        Gameplay = 2,
        Results = 3
    }

    [CreateAssetMenu(
        menuName = "Project Mahjong/Configs/UI Screen Flow",
        fileName = "UIScreenFlowConfig")]
    public sealed class UIScreenFlowConfig : ScriptableObject
    {
        [Serializable]
        public struct TransitionRule
        {
            [SerializeField] private UIScreenId _from;
            [SerializeField] private UIScreenId _to;

            public TransitionRule(UIScreenId from, UIScreenId to)
            {
                _from = from;
                _to = to;
            }

            public UIScreenId From => _from;
            public UIScreenId To => _to;
        }

        [Header("Identity")]
        [Min(1)]
        [SerializeField] private int _schemaVersion = 1;
        [SerializeField] private string _flowId = "ui.flow.default";
        [SerializeField] private string _displayName = "Default UI Flow";

        [Header("Flow")]
        [SerializeField] private UIScreenId _defaultEntryScreen = UIScreenId.MainMenu;
        [SerializeField] private TransitionRule[] _transitions =
        {
            new TransitionRule(UIScreenId.MainMenu, UIScreenId.LobbySetup),
            new TransitionRule(UIScreenId.LobbySetup, UIScreenId.MainMenu),
            new TransitionRule(UIScreenId.LobbySetup, UIScreenId.Gameplay),
            new TransitionRule(UIScreenId.Gameplay, UIScreenId.Results),
            new TransitionRule(UIScreenId.Results, UIScreenId.LobbySetup),
            new TransitionRule(UIScreenId.Results, UIScreenId.MainMenu)
        };

        public int SchemaVersion => _schemaVersion;
        public string FlowId => _flowId;
        public string DisplayName => _displayName;
        public UIScreenId DefaultEntryScreen => _defaultEntryScreen;
        public TransitionRule[] Transitions => _transitions;

        public bool IsTransitionAllowed(UIScreenId from, UIScreenId to)
        {
            if (_transitions == null || _transitions.Length == 0)
            {
                return false;
            }

            for (var i = 0; i < _transitions.Length; i++)
            {
                var t = _transitions[i];
                if (t.From == from && t.To == to)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnValidate()
        {
            if (_schemaVersion < 1)
            {
                _schemaVersion = 1;
            }
        }
    }
}

