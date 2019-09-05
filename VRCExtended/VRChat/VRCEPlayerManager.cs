using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Obfuscation;

using UnityEngine.Events;

using VRC;
using VRC.Core;

namespace VRCExtended.VRChat
{
    internal static class VRCEPlayerManager
    {
        #region NetworkManager Variables
        private static Type _networkManagerType;
        private static FieldInfo _networkManagerInstance;

        private static UnityActionInternal<Player> _networkManager_OnPlayerJoinedEvent;
        private static UnityActionInternal<Player> _networkManager_OnPlayerLeftEvent;
        #endregion
        #region NetworkManager Properties
        public static Type NetworkManagerType
        {
            get
            {
                if (_networkManagerType == null)
                    _networkManagerType = typeof(PlayerManager).Assembly.GetType("NetworkManager");
                return _networkManagerType;
            }
        }
        public static object NetworkManager
        {
            get
            {
                if(_networkManagerInstance == null)
                    _networkManagerInstance = NetworkManagerType?.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
                return _networkManagerInstance?.GetValue(null);
            }
        }

        public static UnityActionInternal<Player> NetworkManager_OnPlayerJoinedEvent
        {
            get
            {
                if(_networkManager_OnPlayerJoinedEvent == null)
                {
                    if (NetworkManager == null)
                        return null;
                    FieldInfo field = NetworkManagerType?.GetField("OnPlayerJoinedEvent", BindingFlags.Public | BindingFlags.Instance);

                    if (field == null)
                        return null;
                    _networkManager_OnPlayerJoinedEvent = new UnityActionInternal<Player>(field.FieldType, field.GetValue(NetworkManager));
                }
                return _networkManager_OnPlayerJoinedEvent;
            }
        }
        public static UnityActionInternal<Player> NetworkManager_OnPlayerLeftEvent
        {
            get
            {
                if (_networkManager_OnPlayerLeftEvent == null)
                {
                    if (NetworkManager == null)
                        return null;
                    FieldInfo field = NetworkManagerType?.GetField("OnPlayerLeftEvent", BindingFlags.Public | BindingFlags.Instance);

                    if (field == null)
                        return null;
                    _networkManager_OnPlayerLeftEvent = new UnityActionInternal<Player>(field.FieldType, field.GetValue(NetworkManager));
                }
                return _networkManager_OnPlayerLeftEvent;
            }
        }
        #endregion

        #region VRCEPlayerManager Event Variables
        private static Dictionary<PlayerChangeHandle, UnityAction<Player>> _playerJoinedEvents = new Dictionary<PlayerChangeHandle, UnityAction<Player>>();
        private static Dictionary<PlayerChangeHandle, UnityAction<Player>> _playerLeftEvents = new Dictionary<PlayerChangeHandle, UnityAction<Player>>();
        #endregion
        #region VRCEPlayerManager Event Handlers
        public delegate void PlayerChangeHandle(VRCEPlayer player);
        #endregion
        #region VRCEPlayerManager Events
        public static event PlayerChangeHandle OnPlayerJoined
        {
            add
            {
                if (_playerJoinedEvents.ContainsKey(value))
                    return;
                UnityAction<Player> action = new UnityAction<Player>(delegate (Player player)
                {
                    value(new VRCEPlayer(player));
                });

                NetworkManager_OnPlayerJoinedEvent?.Add(action);
                _playerJoinedEvents.Add(value, action);
            }
            remove
            {
                if (!_playerJoinedEvents.ContainsKey(value))
                    return;

                NetworkManager_OnPlayerJoinedEvent?.Remove(_playerJoinedEvents[value]);
                _playerJoinedEvents.Remove(value);
            }
        }
        public static event PlayerChangeHandle OnPlayerLeft
        {
            add
            {
                if (_playerLeftEvents.ContainsKey(value))
                    return;
                UnityAction<Player> action = new UnityAction<Player>(delegate (Player player)
                {
                    value(new VRCEPlayer(player));
                });

                NetworkManager_OnPlayerLeftEvent?.Add(action);
                _playerLeftEvents.Add(value, action);
            }
            remove
            {
                if (!_playerLeftEvents.ContainsKey(value))
                    return;

                NetworkManager_OnPlayerLeftEvent?.Remove(_playerLeftEvents[value]);
                _playerLeftEvents.Remove(value);
            }
        }
        #endregion
    }
}
