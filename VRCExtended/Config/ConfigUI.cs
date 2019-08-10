using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRCExtended.Config.Attributes;

namespace VRCExtended.Config
{
    internal class ConfigUI
    {
        #region Avatars
        [ConfigItem("Avatar categories[Local]", false)]
        public bool? AvatarCategories { get; set; }
        [ConfigItem("Show previously used avatars[Local]", false)]
        public bool? PreviouslyUsedAvatars { get; set; }
        #endregion

        #region Worlds
        [ConfigItem("Recent worlds at top of list", false)]
        public bool? RecentWorldsAtTop { get; set; }
        [ConfigItem("Favorite worlds at top of list", false)]
        public bool? FavoriteWorldsAtTop { get; set; }
        #endregion

        #region Notifications
        [ConfigItem("Player join/leave notifications", false)]
        public bool? PlayerJoinLeaveNotification { get; set; }
        [ConfigItem("Friend came online notification", false)]
        public bool? FriendOnlineNotification { get; set; }
        #endregion
    }
}
