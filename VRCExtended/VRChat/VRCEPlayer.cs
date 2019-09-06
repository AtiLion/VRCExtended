using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC;
using VRC.Core;

using UnityEngine;

namespace VRCExtended.VRChat
{
    internal class VRCEPlayer
    {
        #region VRCEPlayer Reflection
        private static MethodInfo _player_get_user;
        private static MethodInfo _player_get_Instance;

        private static MethodInfo _vrcplayer_get_AvatarManager;
        #endregion
        #region VRCEPlayer Properties
        public static VRCEPlayer Instance
        {
            get
            {
                if (_player_get_Instance == null)
                    _player_get_Instance = typeof(Player).GetMethod("get_Instance", BindingFlags.Public | BindingFlags.Static);
                return new VRCEPlayer(_player_get_Instance?.Invoke(null, new object[0]) as Player);
            }
        }

        public Player Player { get; private set; }
        public VRCPlayer VRCPlayer => Player?.vrcPlayer;
        public APIUser APIUser
        {
            get
            {
                if (Player == null)
                    return null;
                if (_player_get_user == null)
                    _player_get_user = typeof(Player).GetMethod("get_user", BindingFlags.Public | BindingFlags.Instance);
                return _player_get_user?.Invoke(Player, new object[0]) as APIUser;
            }
        }

        public VRCAvatarManager AvatarManager
        {
            get
            {
                if (VRCPlayer == null)
                    return null;
                if (_vrcplayer_get_AvatarManager == null)
                    _vrcplayer_get_AvatarManager = typeof(VRCPlayer).GetMethod("get_AvatarManager", BindingFlags.Public | BindingFlags.Instance);
                return _vrcplayer_get_AvatarManager?.Invoke(VRCPlayer, new object[0]) as VRCAvatarManager;
            }
        }
        #endregion

        #region AvatarManager Properties
        public GameObject Avatar => VRCPlayer?.avatarGameObject;
        public Animator Animator => VRCPlayer?.avatarAnimator;
        #endregion

        #region Player Properties
        public GameObject GameObject => Player.gameObject;
        #endregion

        #region VRCPlayer Reflection
        private static MethodInfo _vrcplayer_add_onAvatarIsReady;
        private static MethodInfo _vrcplayer_remove_onAvatarIsReady;
        #endregion
        #region VRCPlayer Delegates
        public delegate void AvatarReadyHandler();
        #endregion
        #region VRCPlayer Events
        public event AvatarReadyHandler onAvatarIsReady
        {
            add
            {
                if (VRCPlayer == null)
                    return;
                if (_vrcplayer_add_onAvatarIsReady == null)
                    _vrcplayer_add_onAvatarIsReady = typeof(VRCPlayer).GetMethod("add_onAvatarIsReady", BindingFlags.Public | BindingFlags.Instance);
                _vrcplayer_add_onAvatarIsReady.Invoke(VRCPlayer, new object[] { value });
            }
            remove
            {
                if (VRCPlayer == null)
                    return;
                if (_vrcplayer_remove_onAvatarIsReady == null)
                    _vrcplayer_remove_onAvatarIsReady = typeof(VRCPlayer).GetMethod("remove_onAvatarIsReady", BindingFlags.Public | BindingFlags.Instance);
                _vrcplayer_remove_onAvatarIsReady.Invoke(VRCPlayer, new object[] { value });
            }
        }
        #endregion

        #region User Information
        public string DisplayName => APIUser?.displayName;
        public string Username => APIUser?.username;
        public string UniqueID => APIUser?.id;
        #endregion

        public VRCEPlayer(Player player) =>
            Player = player;

        #region .NET Functions
        public override bool Equals(object obj)
        {
            if(obj.GetType() != typeof(VRCEPlayer))
                return false;
            return UniqueID == ((VRCEPlayer)obj).UniqueID;
        }
        public override string ToString() => $"ID: {UniqueID} Username: {Username} DisplayName: {DisplayName}";
        #endregion
        #region .NET Operators
        public static bool operator ==(VRCEPlayer player1, VRCEPlayer player2) =>
            player1.UniqueID == player2.UniqueID;
        public static bool operator !=(VRCEPlayer player1, VRCEPlayer player2) =>
            player1.UniqueID != player2.UniqueID;
        #endregion
    }
}
