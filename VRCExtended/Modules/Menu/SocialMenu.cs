using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCMenuUtils;
using VRChat.UI;

using UnityEngine;

namespace VRCExtended.Modules.Menu
{
    internal class SocialMenu : IExtendedModule
    {
        public void Setup() { }
        public IEnumerator AsyncSetup()
        {
            // Wait for load and check
            yield return VRCMenuUtilsAPI.WaitForInit();
            if (VRCEUi.SocialScreen == null)
                yield break;

            // Get VRChat UI elements
            Transform currentStatus = VRCEUi.SocialScreen.transform.Find("Current Status");
            Transform btnStatus = currentStatus.Find("StatusButton");
            RectTransform rt_btnStatus = btnStatus.GetComponent<RectTransform>();
            Transform icnStatus = currentStatus.Find("StatusIcon");
            RectTransform rt_icnStatus = icnStatus.GetComponent<RectTransform>();
            Transform txtStatus = currentStatus.Find("StatusText");
            RectTransform rt_txtStatus = txtStatus.GetComponent<RectTransform>();

            // Setup refresh button
            VRCEUiButton btnRefresh = new VRCEUiButton("refresh", new Vector2(rt_btnStatus.localPosition.x - 20f, rt_btnStatus.localPosition.y), "Refresh", currentStatus);
            RectTransform rt_btnRefresh = btnRefresh.Control.GetComponent<RectTransform>();

            // Fix UI positions
            rt_btnStatus.localPosition += new Vector3(210f, 0f, 0f);
            rt_icnStatus.localPosition += new Vector3(210f, 0f, 0f);
            rt_txtStatus.localPosition += new Vector3(210f, 0f, 0f);
            rt_btnRefresh.sizeDelta -= new Vector2(5f, 10f);

            // Add click check
            btnRefresh.OnClick += () =>
            {
                UiUserList[] userLists = VRCEUi.SocialScreen.GetComponentsInChildren<UiUserList>(true);

                foreach (UiUserList userList in userLists)
                {
                    userList.ClearAll();
                    userList.Refresh();
                    userList.RefreshData();
                }
                ExtendedLogger.Log("Refreshed social lists!");
            };
        }
    }
}
