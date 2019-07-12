using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended;
using VRChat.Obfuscation;

using UnityEngine.Events;

using VRC;
using VRC.Core;

namespace VRChat
{
    public static class VRCPlayerManager
    {
        #region Reflection Variables
        // Network Manager
        private static Type _networkManagerType;

        private static FieldInfo _networkManagerInstance;
        private static OBFUnityActionInternal<Player> _networkManager_OnPlayerJoinedEvent;
        private static OBFUnityActionInternal<Player> _networkManager_OnPlayerLeftEvent;
        #endregion

        #region PlayerManager Event Delegates
        public delegate void PlayerChangeHandle(VRCEPlayer player);
        #endregion

        #region PlayerManager Events
        public static event PlayerChangeHandle OnPlayerJoined;
        public static event PlayerChangeHandle OnPlayerLeft;
        #endregion

        internal static void Setup()
        {
            // Network manager setup
            _networkManagerType = typeof(PlayerManager).Assembly.GetType("NetworkManager");
            if(_networkManagerType == null)
            {
                ExtendedLogger.LogError("Failed to get NetworkManager!");
                return;
            }
            ExtendedLogger.Log("Found NetworkManager!");

            _networkManagerInstance = _networkManagerType.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
            if (_networkManagerInstance == null)
            {
                ExtendedLogger.LogError("Failed to get NetworkManager Instance!");
                return;
            }
            ExtendedLogger.Log("Found NetworkManager Instance!");

            FieldInfo onPlayerJoinedEvent = _networkManagerType.GetField("OnPlayerJoinedEvent", BindingFlags.Public | BindingFlags.Instance);
            if (onPlayerJoinedEvent == null)
            {
                ExtendedLogger.LogError("Failed to get NetworkManager OnPlayerJoinedEvent!");
                return;
            }
            ExtendedLogger.Log("Found NetworkManager OnPlayerJoinedEvent!");

            FieldInfo onPlayerLeftEvent = _networkManagerType.GetField("OnPlayerLeftEvent", BindingFlags.Public | BindingFlags.Instance);
            if (onPlayerLeftEvent == null)
            {
                ExtendedLogger.LogError("Failed to get NetworkManager OnPlayerLeftEvent!");
                return;
            }
            ExtendedLogger.Log("Found NetworkManager OnPlayerLeftEvent!");

            Type unityActionInternalType = onPlayerJoinedEvent.FieldType;
            if (unityActionInternalType == null)
            {
                ExtendedLogger.LogError("Failed to get UnityActionInternal!");
                return;
            }
            ExtendedLogger.Log("Found UnityActionInternal named " + unityActionInternalType.Name + "!");

            _networkManager_OnPlayerJoinedEvent = new OBFUnityActionInternal<Player>(unityActionInternalType, onPlayerJoinedEvent.GetValue(_networkManagerInstance.GetValue(null)));
            _networkManager_OnPlayerLeftEvent = new OBFUnityActionInternal<Player>(unityActionInternalType, onPlayerLeftEvent.GetValue(_networkManagerInstance.GetValue(null)));

            _networkManager_OnPlayerJoinedEvent.Add(new UnityAction<Player>(delegate(Player player)
            {
                if (OnPlayerJoined != null)
                    OnPlayerJoined(new VRCEPlayer(player));
            }));
            _networkManager_OnPlayerLeftEvent.Add(new UnityAction<Player>(delegate (Player player)
            {
                if (OnPlayerLeft != null)
                    OnPlayerLeft(new VRCEPlayer(player));
            }));
        }
    }
}
