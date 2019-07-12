using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRC;
using VRC.Core;

using UnityEngine;

namespace VRChat
{
    public class VRCEPlayer
    {
        #region Reflection Variables
        private static MethodInfo _player_get_user;
        private static MethodInfo _player_get_Instance;

        private static MethodInfo _vrcplayer_get_AvatarManager;
        private static MethodInfo _vrcplayer_get_uSpeaker;

        private static FieldInfo _uspeaker_AudioSource;
        #endregion

        #region VRCEPlayer Properties
        public Player Player { get; private set; }
        public VRCPlayer VRCPlayer => Player.vrcPlayer;
        public APIUser APIUser => _player_get_user.Invoke(Player, new object[0]) as APIUser;

        public VRCAvatarManager AvatarManager => _vrcplayer_get_AvatarManager.Invoke(VRCPlayer, new object[0]) as VRCAvatarManager;
        public USpeaker uSpeaker => _vrcplayer_get_uSpeaker.Invoke(VRCPlayer, new object[0]) as USpeaker;

        public bool IsSelf => APIUser.id == APIUser.CurrentUser.id;
        public static VRCEPlayer Instance => new VRCEPlayer(_player_get_Instance.Invoke(null, new object[] { }) as Player);
        #endregion

        #region Avatar Properties
        public GameObject Avatar => AvatarManager.currentAvatarObject;

        public AudioSource Voice => _uspeaker_AudioSource.GetValue(uSpeaker) as AudioSource;
        public AudioSource[] Sounds => Avatar.GetComponentsInChildren<AudioSource>().Where(a => a != Voice).ToArray();
        #endregion

        #region User Properties
        public string UniqueID => APIUser.id;
        #endregion

        public VRCEPlayer(Player player)
        {
            Player = player;
        }
        public VRCEPlayer()
        {
            Player = _player_get_Instance.Invoke(null, new object[] { }) as Player;
        }

        internal static void Setup()
        {
            _player_get_user = typeof(Player).GetMethod("get_user", BindingFlags.Public | BindingFlags.Instance);
            _player_get_Instance = typeof(Player).GetMethod("get_Instance", BindingFlags.Public | BindingFlags.Static);

            _vrcplayer_get_AvatarManager = typeof(VRCPlayer).GetMethod("get_AvatarManager", BindingFlags.Public | BindingFlags.Instance);
            _vrcplayer_get_uSpeaker = typeof(VRCPlayer).GetMethod("get_uSpeaker", BindingFlags.Static | BindingFlags.Instance);

            _uspeaker_AudioSource = typeof(USpeaker).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).First(a => a.FieldType == typeof(AudioSource));
        }
    }
}
