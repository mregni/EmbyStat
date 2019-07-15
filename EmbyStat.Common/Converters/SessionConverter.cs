using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Events;
using Newtonsoft.Json.Linq;

namespace EmbyStat.Common.Converters
{
    public static class SessionConverter
    {
        public static IEnumerable<Session> ConvertToSessions(JArray sessions)
        {
            var now = DateTime.Now;
            foreach (var session in sessions.Children().Where(s => s["UserId"] != null))
            {
                var ses = new Session
                {
                    Id = session["Id"].Value<string>(),
                    UserId = session["UserId"].Value<string>(),
                    AppIconUrl = session["AppIconUrl"]?.Value<string>(),
                    ApplicationVersion = session["ApplicationVersion"].Value<string>(),
                    Client = session["Client"].Value<string>(),
                    DeviceId = session["DeviceId"].Value<string>(),
                    ServerId = session["ServerId"].Value<string>(),
                };

                if (session["PlayState"]["MediaSourceId"] != null)
                {
                    Enum.TryParse(session["NowPlayingItem"]["Type"].Value<string>(), true, out PlayType playType);
                    var play = new Play
                    {
                        MediaId = session["NowPlayingItem"]["Id"].Value<string>(),
                        Type = playType,
                        ParentId = session["NowPlayingItem"]["ParentId"].Value<string>(),
                        SessionId = session["Id"].Value<string>(),
                        UserId = session["UserId"].Value<string>(),
                        Id = Guid.NewGuid()
                    };
                    
                    var videoStream = session["NowPlayingItem"]["MediaStreams"]?.FirstOrDefault(x => x["Type"].Value<string>() == "Video");
                    if (videoStream != null)
                    {
                        play.VideoAspectRatio = videoStream["AspectRatio"]?.Value<string>();
                        play.VideoAverageFrameRate = videoStream["AverageFrameRate"]?.Value<double?>();
                        play.VideoCodec = videoStream["Codec"]?.Value<string>();
                        play.VideoHeight = videoStream["Height"]?.Value<int?>();
                        play.VideoRealFrameRate = videoStream["RealFrameRate"]?.Value<double?>();
                        play.VideoWidth = videoStream["Width"]?.Value<int?>();
                    }

                    var subtitleIndex = session["PlayState"]["SubtitleStreamIndex"]?.Value<int?>();
                    if (subtitleIndex != null && subtitleIndex != -1)
                    {
                        var subtitleStream = session["NowPlayingItem"]["MediaStreams"][subtitleIndex];
                        play.SubtitleCodec = subtitleStream["Codec"]?.Value<string>();
                        play.SubtitleDisplayTitle = subtitleStream["DisplayTitle"]?.Value<string>();
                        play.SubtitleLanguage = subtitleStream["Language"]?.Value<string>();
                    }

                    var audioIndex = session["PlayState"]["AudioStreamIndex"]?.Value<int?>();
                    if (audioIndex != null && audioIndex != -1)
                    {
                        var audioStream = session["NowPlayingItem"]["MediaStreams"][audioIndex];
                        play.AudioChannelLayout = audioStream["ChannelLayout"]?.Value<string>();
                        play.AudioCodec = audioStream["Codec"]?.Value<string>();
                        play.AudioLanguage = audioStream["Language"]?.Value<string>();
                    }

                    Enum.TryParse(session["PlayState"]["RepeatMode"].Value<string>(), true, out RepeatMode repeatMode);
                    Enum.TryParse(session["PlayState"]["PlayMethod"]?.Value<string>(), true, out PlayMethod playMethod);
                    play.PlayStates.Add(new PlayState
                    {
                        MediaSourceId = session["PlayState"]["MediaSourceId"].Value<string>(),
                        PositionTicks = session["PlayState"]["PositionTicks"]?.Value<long>(),
                        IsPaused = session["PlayState"]["IsPaused"].Value<bool>(),
                        TimeLogged = now,
                        VolumeLevel = session["PlayState"]["VolumeLevel"]?.Value<int>(),
                        IsTranscoding = session["TranscodingInfo"] != null,
                        AudioCodec = session["TranscodingInfo"]?["AudioCodec"].Value<string>(),
                        TranscodeReasons = session["TranscodingInfo"]?["TranscodeReasons"]
                            ?.ToObject<List<TranscodeReason>>(),
                        VideoCodec = session["TranscodingInfo"]?["VideoCodec"]?.Value<string>(),
                        RepeatMode = repeatMode,
                        PlayMethod = playMethod
                    });

                    ses.Plays.Add(play);
                }

                yield return ses;
            }
        }
    }
}