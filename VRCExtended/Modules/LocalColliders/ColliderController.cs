using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.VRChat;

namespace VRCExtended.Modules.LocalColliders
{
    internal class ColliderController : IExtendedModule
    {
        #region Controller Properties
        public static Dictionary<string, ColliderUser> _users = new Dictionary<string, ColliderUser>();
        #endregion

        public void Setup() { }
        public IEnumerator AsyncSetup()
        {
            // Wait for VRChat to load
            yield return VRCEManager.WaitForVRChatLoad();

            // Setup events
            VRCEPlayerManager.OnPlayerJoined += VRCEPlayerManager_OnPlayerJoined;
            VRCEPlayerManager.OnPlayerLeft += VRCEPlayerManager_OnPlayerLeft;
        }

        #region Player Event Handlers
        private void VRCEPlayerManager_OnPlayerJoined(VRCEPlayer player)
        {
            
        }
        private void VRCEPlayerManager_OnPlayerLeft(VRCEPlayer player)
        {
        }
        #endregion
    }
}
