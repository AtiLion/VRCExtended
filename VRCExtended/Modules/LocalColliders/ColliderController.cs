using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine.SceneManagement;

using VRCExtended.VRChat;

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
        }

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
            Users.Add(player.UniqueID, player.GameObject.AddComponent<ColliderHandler>());
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
