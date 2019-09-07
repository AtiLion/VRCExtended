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
        private GameObject _lightGameObject;
        #endregion
        #region Headlight Functions
        public void AddLight()
        {
            if (_lightGameObject != null)
                return;
            Transform head = VRCEPlayer.Instance.Animator.GetBoneTransform(HumanBodyBones.Head);
            
            if(head)
            {
                _lightGameObject = new GameObject("Headlight");
                Light light = _lightGameObject.AddComponent<Light>();

                light.color = Color.white;
                light.type = LightType.Spot;
                light.shadows = LightShadows.Soft;
                light.intensity = 0.6f;

                _lightGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                _lightGameObject.transform.position = Camera.main.transform.position;
                _lightGameObject.transform.SetParent(Camera.main.transform);

                ExtendedLogger.Log("Added headlight to user!");
            }
        }
        public void RemoveLight()
        {
            if (_lightGameObject == null)
                return;

            GameObject.Destroy(_lightGameObject);
            _lightGameObject = null;
            ExtendedLogger.Log("Removed headlight from user!");
        }
        #endregion

        #region Player Event Handlers
        private void VRCEPlayerManager_OnPlayerJoined(VRCEPlayer player)
        {
            if (player != VRCEPlayer.Instance)
                return;
            if (!(bool)ManagerConfig.Config.Users.HeadLight)
                return;

            AddLight();
        }
        #endregion
    }
}
