using System;
using EmbyStat.Common.Models.Entities.Users;

namespace EmbyStat.Common.Models.Entities.Events;

public class MediaPlay
{
    public int Id { get; set; }
    public string SessionId { get; set; }
    public Session Session { get; set; }
    public string UserId { get; set; }
    public MediaServerUser User { get; set; }
    public string MediaId { get; set; }
    public string Type { get; set; }
    public string PlayMethod { get; set; }
    public DateTime Start { get; set; }
    public DateTime LastUpdate { get; set; }
    public DateTime? Stop { get; set; }
    public long StartPositionTicks { get; set; }
    public long EndPositionTicks { get; set; }
    public double WatchedPercentage { get; set; }
    public bool IsPaused { get; set; }
    public string AudioCodec { get; set; }
    public string AudioChannelLayout { get; set; }
    public int? AudioSampleRate { get; set; }
    public string SubtitleCodec { get; set; }
    public string SubtitleDisplayLanguage { get; set; }
    public string SubtitleLanguage { get; set; }
    public string SubtitleProtocol { get; set; }
    public double? TranscodeAverageCpuUsage { get; set; }
    public double? TranscodeCurrentCpuUsage { get; set; }
    public string TranscodeVideoCodec { get; set; }
    public string TranscodeAudioCodec { get; set; }
    public string TranscodeSubProtocol { get; set; }
    public string TranscodeReasons { get; set; }
    public string Encoder { get; set; }
    public bool? EncoderIsHardware { get; set; }
    public string EncoderMediaType { get; set; }
    public string Decoder { get; set; }
    public bool? DecoderIsHardware { get; set; }
    public string DecoderMediaType { get; set; }
}