using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using VRCExtended.Config;
using VRCExtended.VRChat;

using VRC;
using VRC_AvatarDescriptor = VRCSDK2.VRC_AvatarDescriptor;

namespace VRCExtended.Modules.Users
{
    internal class Headlight : IExtendedModule
    {
        public void Setup() { }
        public IEnumerator AsyncSetup()
        {
            // Wait for VRChat to load
            yield return VRCEManager.WaitForVRChatLoad();

            // Setup events
            VRCEPlayerManager.OnPlayerJoined += VRCEPlayerManager_OnPlayerJoined;

            // Configuration watchers
            ManagerConfig.WatchForUpdate("Users.HeadLight", () =>
            {
                if ((bool)ManagerConfig.Config.Users.HeadLight)
                    AddLight();
                else
                    RemoveLight();
            });
        }

        #region Headlight Variables
        private Light _light;
        #endregion
        #region Headlight Functions
        public void AddLight()
        {
            if (_light != null)
                return;
            if (VRCEPlayer.Instance.VRCPlayer.avatarGameObject == null || VRCEPlayer.Instance.Animator == null)
                return;
            Transform head = VRCEPlayer.Instance.Animator.GetBoneTransform(HumanBodyBones.Head);
            
            if(head)
            {
                _light = head.gameObject.AddComponent<Light>();

                _light.color = Color.white;
                _light.type = LightType.Spot;
                _light.shadows = LightShadows.Soft;
                _light.intensity = 0.8f;

                ExtendedLogger.Log("Added headlight to user!");
            }
        }
        public void RemoveLight()
        {
            if (_light == null)
                return;

            GameObject.Destroy(_light);
            _light = null;
            ExtendedLogger.Log("Removed headlight from user!");
        }
        #endregion

        #region Player Event Handlers
        private void VRCEPlayerManager_OnPlayerJoined(VRCEPlayer player)
        {
            if (player != VRCEPlayer.Instance)
                return;

            // Add event
            player.AvatarManager.OnAvatarCreated += delegate (GameObject avatar, VRC_AvatarDescriptor descriptor, bool loaded)
            {
                if (!(bool)ManagerConfig.Config.Users.HeadLight)
                    return;
                AddLight();
            };

            // Add headlight
            if (!(bool)ManagerConfig.Config.Users.HeadLight)
                return;
            AddLight();
        }
        #endregion
    }
}
