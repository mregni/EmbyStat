using System;
using System.Collections.Generic;

namespace EmbyStat.Common.Models.Sessions;

public class Session
{
    public PlayState PlayState { get; set; }
    public List<object> AdditionalUsers { get; set; }
    public string RemoteEndPoint { get; set; }
    public string Protocol { get; set; }
    public List<string> PlayableMediaTypes { get; set; }
    public int PlaylistIndex { get; set; }
    public int PlaylistLength { get; set; }
    public string Id { get; set; }
    public string ServerId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string UserPrimaryImageTag { get; set; }
    public string Client { get; set; }
    public DateTime LastActivityDate { get; set; }
    public string DeviceName { get; set; }
    public int InternalDeviceId { get; set; }
    public string DeviceId { get; set; }
    public string ApplicationVersion { get; set; }
    public string AppIconUrl { get; set; }
    public List<string> SupportedCommands { get; set; }
    public bool SupportsRemoteControl { get; set; }
}

public abstract class PlayState
{
    public bool CanSeek { get; set; }
    public bool IsPaused { get; set; }
    public bool IsMuted { get; set; }
    public string RepeatMode { get; set; }
    public int SubtitleOffset { get; set; }
    public int PlaybackRate { get; set; }
}