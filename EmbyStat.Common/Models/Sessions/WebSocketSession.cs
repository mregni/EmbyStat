using System;
using System.Collections.Generic;

namespace EmbyStat.Common.Models.Sessions;

public class WebSocketSession
{
    public PlayState PlayState { get; set; }
    public string RemoteEndPoint { get; set; }
    public string Protocol { get; set; }
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
    public NowPlayingItem NowPlayingItem { get; set; }
    public int InternalDeviceId { get; set; }
    public string DeviceId { get; set; }
    public string ApplicationVersion { get; set; }
    public string AppIconUrl { get; set; }
    public List<string> SupportedCommands { get; set; }
    public TranscodingInfo TranscodingInfo { get; set; }
    public bool SupportsRemoteControl { get; set; }
}

public class MediaStream
{
    public string Codec { get; set; }
    public string ColorTransfer { get; set; }
    public string ColorPrimaries { get; set; }
    public string ColorSpace { get; set; }
    public string TimeBase { get; set; }
    public string VideoRange { get; set; }
    public string DisplayTitle { get; set; }
    public string NalLengthSize { get; set; }
    public bool IsInterlaced { get; set; }
    public int BitRate { get; set; }
    public int BitDepth { get; set; }
    public int RefFrames { get; set; }
    public bool IsDefault { get; set; }
    public bool IsForced { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public double AverageFrameRate { get; set; }
    public double RealFrameRate { get; set; }
    public string Profile { get; set; }
    public string Type { get; set; }
    public string AspectRatio { get; set; }
    public int Index { get; set; }
    public bool IsExternal { get; set; }
    public bool IsTextSubtitleStream { get; set; }
    public bool SupportsExternalStream { get; set; }
    public string Protocol { get; set; }
    public string PixelFormat { get; set; }
    public int Level { get; set; }
    public bool IsAnamorphic { get; set; }
    public int AttachmentSize { get; set; }
    public string Language { get; set; }
    public string Title { get; set; }
    public string DisplayLanguage { get; set; }
    public string ChannelLayout { get; set; }
    public int? Channels { get; set; }
    public int? SampleRate { get; set; }
    public string SubtitleLocationType { get; set; }
    public string Path { get; set; }
}

public class NowPlayingItem
{
    public string Name { get; set; }
    public string OriginalTitle { get; set; }
    public string ServerId { get; set; }
    public string Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string PresentationUniqueKey { get; set; }
    public string Container { get; set; }
    public string Path { get; set; }
    public string Overview { get; set; }
    public List<object> Taglines { get; set; }
    public List<string> Genres { get; set; }
    public double CommunityRating { get; set; }
    public long RunTimeTicks { get; set; }
    public long Size { get; set; }
    public string FileName { get; set; }
    public int Bitrate { get; set; }
    public int ProductionYear { get; set; }
    public bool IsFolder { get; set; }
    public string ParentId { get; set; }
    public string Type { get; set; }
    public int LocalTrailerCount { get; set; }
    public double PrimaryImageAspectRatio { get; set; }
    public List<MediaStream> MediaStreams { get; set; }
    public string MediaType { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public DateTime? PremiereDate { get; set; }
    public int? IndexNumber { get; set; }
    public int? ParentIndexNumber { get; set; }
    public string ParentLogoItemId { get; set; }
    public string ParentBackdropItemId { get; set; }
    public List<string> ParentBackdropImageTags { get; set; }
    public string SeriesName { get; set; }
    public string SeriesId { get; set; }
    public string SeasonId { get; set; }
    public string SeriesPrimaryImageTag { get; set; }
    public string SeasonName { get; set; }
    public string ParentLogoImageTag { get; set; }
    public string ParentThumbItemId { get; set; }
    public string ParentThumbImageTag { get; set; }
}

public class PlayState
{
    public long PositionTicks { get; set; }
    public bool CanSeek { get; set; }
    public bool IsPaused { get; set; }
    public bool IsMuted { get; set; }
    public int VolumeLevel { get; set; }
    public int AudioStreamIndex { get; set; }
    public int SubtitleStreamIndex { get; set; }
    public string MediaSourceId { get; set; }
    public string PlayMethod { get; set; }
    public string RepeatMode { get; set; }
    public int SubtitleOffset { get; set; }
    public int PlaybackRate { get; set; }
}

public class TranscodingInfo
{
    public string AudioCodec { get; set; }
    public string VideoCodec { get; set; }
    public string SubProtocol { get; set; }
    public string Container { get; set; }
    public bool IsVideoDirect { get; set; }
    public bool IsAudioDirect { get; set; }
    public int Bitrate { get; set; }
    public int AudioBitrate { get; set; }
    public int VideoBitrate { get; set; }
    public int Framerate { get; set; }
    public double CompletionPercentage { get; set; }
    public long TranscodingPositionTicks { get; set; }
    public long TranscodingStartPositionTicks { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int AudioChannels { get; set; }
    public List<string> TranscodeReasons { get; set; }
    public double CurrentCpuUsage { get; set; }
    public double AverageCpuUsage { get; set; }
    public string VideoDecoder { get; set; }
    public bool VideoDecoderIsHardware { get; set; }
    public string VideoDecoderMediaType { get; set; }
    public string VideoEncoder { get; set; }
    public bool VideoEncoderIsHardware { get; set; }
    public string VideoEncoderMediaType { get; set; }
}

public class VideoPipelineInfo
{
    public string HardwareContextName { get; set; }
    public bool IsHardwareContext { get; set; }
    public string Name { get; set; }
    public string Short { get; set; }
    public string StepType { get; set; }
    public string StepTypeName { get; set; }
    public string FfmpegName { get; set; }
    public string FfmpegDescription { get; set; }
    public string FfmpegOptions { get; set; }
    public string Param { get; set; }
    public string ParamShort { get; set; }
}