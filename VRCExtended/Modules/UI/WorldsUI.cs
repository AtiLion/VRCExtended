using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using VRCMenuUtils;
using VRChat.UI;

using VRC.UI;

using VRCExtended.Config;

namespace VRCExtended.Modules.UI
{
    internal class WorldsUI : IExtendedModule
    {
        #region UI Variables
        public Dictionary<Transform, int> _positions = new Dictionary<Transform, int>();

        private Transform _worldsList;
        #endregion

        public void Setup() { }
        public IEnumerator AsyncSetup()
        {
            yield return VRCMenuUtilsAPI.WaitForInit();

            // Add VRCMenuUtils API
            VRCMenuUtilsAPI.OnPageShown += VRCMenuUtilsAPI_OnPageShown;

            // Wait for Worlds
            while (_worldsList == null)
                yield return null;

            // Wait for favourite and recent
            while (_worldsList.Find("Playlist1") == null || _worldsList.Find("Recent") == null)
                yield return null;

            // Grab favourites and recent
            Transform[] favourites = new Transform[]
            {
                _worldsList.Find("Playlist1"),
                _worldsList.Find("Playlist2"),
                _worldsList.Find("Playlist3"),
                _worldsList.Find("Playlist4")
            };
            Transform recent = _worldsList.Find("Recent");

            // Grab the positions
            foreach (Transform favourite in favourites)
                _positions.Add(favourite, favourite.GetSiblingIndex());
            _positions.Add(recent, recent.GetSiblingIndex());

            // Change positions
            ChangePositions();

            // Set watchers
            ManagerConfig.WatchForUpdate("UI.FavoriteWorldsAtTop", () =>
                ChangePositions());
            ManagerConfig.WatchForUpdate("UI.RecentWorldsAtTop", () =>
                ChangePositions());

            VRCMenuUtilsAPI.OnPageShown -= VRCMenuUtilsAPI_OnPageShown;
            ExtendedLogger.Log("Worlds UI positions setup!");
        }

        #region UI Events
        private void VRCMenuUtilsAPI_OnPageShown(VRCUiPage page)
        {
            if (page.GetType() != typeof(VRCUiPageWorlds))
                return;
            if (_positions.Count > 0) // Already initialized
                return;

            // Grab main transforms
            _worldsList = VRCEUi.WorldsScreen.transform.Find("Vertical Scroll View/Viewport/Content");
        }
        #endregion

        #region WorldsUI Functions
        private void ChangePositions()
        {
            KeyValuePair<Transform, int> element;

            for (int i = 0; i < 4; i++)
            {
                element = _positions.ElementAt(i);

                if ((bool)ManagerConfig.Config.UI.FavoriteWorldsAtTop)
                    element.Key.SetSiblingIndex(i);
                else
                    element.Key.SetSiblingIndex(element.Value);
            }

            element = _positions.ElementAt(4);
            if ((bool)ManagerConfig.Config.UI.RecentWorldsAtTop)
                element.Key.SetSiblingIndex(0);
            else
                element.Key.SetSiblingIndex(element.Value);
        }
        #endregion

        #region Enums
        public enum ETargetTransform
        {
            FAVOURITE,
            RECENT
        }
        #endregion
    }
}
