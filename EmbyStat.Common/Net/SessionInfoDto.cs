using System;
using System.Collections.Generic;
using System.Text;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Session;

namespace EmbyStat.Common.Net
{
    public class SessionInfoDto
    {
        public string[] SupportedCommands { get; set; }
        public string[] PlayableMediaTypes { get; set; }
        public string Id { get; set; }
        public string ServerId { get; set; }
        public string UserId { get; set; }
        public string UserPrimaryImageTag { get; set; }
        public string UserName { get; set; }
        public SessionUserInfo[] AdditionalUsers { get; set; }
        public string ApplicationVersion { get; set; }
        public string Client { get; set; }
        public DateTime LastActivityDate { get; set; }
        public BaseItemDto NowViewingItem { get; set; }
        public string DeviceName { get; set; }
        public BaseItemDto NowPlayingItem { get; set; }
        public string DeviceId { get; set; }
        public string AppIconUrl { get; set; }
        public bool SupportsRemoteControl { get; set; }
        public PlayerStateInfo PlayState { get; set; }
        public TranscodingInfo TranscodingInfo { get; set; }

        public SessionInfoDto()
        {
            AdditionalUsers = new SessionUserInfo[0];
            PlayableMediaTypes = new string[0];
            SupportedCommands = new string[0];
        }
    }
}
