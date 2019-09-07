using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine.SceneManagement;

using VRCExtended.VRChat;
using VRCExtended.Config;

namespace VRCExtended.Modules.LocalColliders
{
    internal class ColliderController : IExtendedModule
    {
        #region Controller Properties
        public static Dictionary<string, ColliderHandler> Users = new Dictionary<string, ColliderHandler>();
        #endregion

        public void Setup() { }
        public IEnumerator AsyncSetup()
        {
            // Wait for VRChat to load
            yield return VRCEManager.WaitForVRChatLoad();

            // Setup events
            VRCEPlayerManager.OnPlayerJoined += VRCEPlayerManager_OnPlayerJoined;
            VRCEPlayerManager.OnPlayerLeft += VRCEPlayerManager_OnPlayerLeft;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;

            // Setup configuration events
            ManagerConfig.OnValueUpdate += ManagerConfig_OnValueUpdate;
            ManagerConfig.OnSave += ManagerConfig_OnSave;
        }

        #region Configuration Variables
        private bool UpdateColliders = false;
        #endregion
        #region Configuration Event Handlers
        private void ManagerConfig_OnValueUpdate(MapConfig conf)
        {
            if (conf.Property.DeclaringType.Name != "ConfigLocalColliders")
                return;
            UpdateColliders = true;
        }
        private void ManagerConfig_OnSave()
        {
            if (!UpdateColliders)
                return;
            UpdateColliders = false;
            ExtendedLogger.Log("Updated collider configuration!");

            // If disabled remove all
            if(!(bool)ManagerConfig.Config.LocalColliders.Enabled)
            {
                foreach (ColliderHandler handler in Users.Values)
                    handler.ClearColliders();
                return;
            }

            // Reload colliders
            foreach (ColliderHandler handler in Users.Values)
            {
                handler.PopulateColliders();
                if (!(bool)ManagerConfig.Config.LocalColliders.DisableOnDistance)
                    handler.ApplyColliders();
            }
        }
        #endregion

        #region VRChat Event Handlers
        private void SceneManager_sceneUnloaded(Scene scene)
        {
            if (scene.buildIndex != -1)
                return;

            // Reset
            Users.Clear();
        }
        #endregion

        #region Player Event Handlers
        private void VRCEPlayerManager_OnPlayerJoined(VRCEPlayer player)
        {
            // Reset when switching worlds
            if(player == VRCEPlayer.Instance && Users.ContainsKey(player.UniqueID))
            {
                Users[player.UniqueID].ClearColliders();
                Users.Clear();
            }

            // Setup handler
            ColliderHandler handler = player.GameObject.AddComponent<ColliderHandler>();
            handler.enabled = true;

            // Add handler
            Users.Add(player.UniqueID, handler);
            ExtendedLogger.Log($"User {player.DisplayName} added to colliders!");
        }
        private void VRCEPlayerManager_OnPlayerLeft(VRCEPlayer player)
        {
            Users.Remove(player.UniqueID);
            ExtendedLogger.Log($"User {player.DisplayName} removed from colliders!");
        }
        #endregion
    }
}
