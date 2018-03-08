using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Data;
using Emby.ApiClient.Model;
using Emby.ApiClient.Net;

namespace Emby.ApiClient.Playback
{
    public class PlaybackManager : IPlaybackManager
    {
        private readonly ILocalAssetManager _localAssetManager;
        private readonly ILogger _logger;
        private readonly IDevice _device;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackManager" /> class.
        /// </summary>
        /// <param name="localAssetManager">The local asset manager.</param>
        /// <param name="device">The device.</param>
        /// <param name="logger">The logger.</param>
        public PlaybackManager(ILocalAssetManager localAssetManager, IDevice device, ILogger logger)
        {
            _localAssetManager = localAssetManager;
            _device = device;
            _logger = logger;
        }

        public PlaybackManager(ILocalAssetManager localAssetManager, IDevice device, ILogger logger, INetworkConnection network)
            : this(localAssetManager, device, logger)
        {
        }

        /// <summary>
        /// Gets the pre playback selectable audio streams.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;IEnumerable&lt;MediaStream&gt;&gt;.</returns>
        public async Task<IEnumerable<MediaStream>> GetPrePlaybackSelectableAudioStreams(string serverId, VideoOptions options)
        {
            var info = await GetVideoStreamInfoInternal(serverId, options).ConfigureAwait(false);

            return info.GetSelectableAudioStreams();
        }

        /// <summary>
        /// Gets the pre playback selectable subtitle streams.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;IEnumerable&lt;MediaStream&gt;&gt;.</returns>
        public async Task<IEnumerable<MediaStream>> GetPrePlaybackSelectableSubtitleStreams(string serverId, VideoOptions options)
        {
            var info = await GetVideoStreamInfoInternal(serverId, options).ConfigureAwait(false);

            return info.GetSelectableSubtitleStreams();
        }

        /// <summary>
        /// Gets the in playback selectable audio streams.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>IEnumerable&lt;MediaStream&gt;.</returns>
        public IEnumerable<MediaStream> GetInPlaybackSelectableAudioStreams(StreamInfo info)
        {
            return info.GetSelectableAudioStreams();
        }

        /// <summary>
        /// Gets the in playback selectable subtitle streams.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>IEnumerable&lt;MediaStream&gt;.</returns>
        public IEnumerable<MediaStream> GetInPlaybackSelectableSubtitleStreams(StreamInfo info)
        {
            return info.GetSelectableSubtitleStreams();
        }

        /// <summary>
        /// Gets the stream builder.
        /// </summary>
        /// <returns>StreamBuilder.</returns>
        private StreamBuilder GetStreamBuilder()
        {
            return new StreamBuilder(_logger);
        }

        /// <summary>
        /// Gets the audio stream information.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="options">The options.</param>
        /// <param name="isOffline">if set to <c>true</c> [is offline].</param>
        /// <param name="apiClient">The API client.</param>
        /// <returns>Task&lt;StreamInfo&gt;.</returns>
        public async Task<StreamInfo> GetAudioStreamInfo(string serverId, AudioOptions options, bool isOffline, IApiClient apiClient)
        {
            var streamBuilder = GetStreamBuilder();

            var localItem = await _localAssetManager.GetLocalItem(serverId, options.ItemId);
            if (localItem != null)
            {
                var localMediaSource = localItem.Item.MediaSources[0];

                // Use the local media source, unless a specific server media source was requested
                if (string.IsNullOrWhiteSpace(options.MediaSourceId) ||
                    string.Equals(localMediaSource.Id, options.MediaSourceId,
                    StringComparison.OrdinalIgnoreCase))
                {
                    // Finally, check to make sure the local file is actually available at this time
                    var fileExists = await _localAssetManager.FileExists(localMediaSource.Path).ConfigureAwait(false);

                    if (fileExists)
                    {
                        options.MediaSources = localItem.Item.MediaSources.ToArray();

                        var result = streamBuilder.BuildAudioItem(options);
                        if (result == null)
                        {
                            _logger.Warn("LocalItem returned no compatible streams. Will dummy up a StreamInfo to force it to direct play.");
                            var mediaSource = localItem.Item.MediaSources.First();
                            result = GetForcedDirectPlayStreamInfo(DlnaProfileType.Audio, options, mediaSource);
                        }
                        result.PlayMethod = PlayMethod.DirectPlay;
                        return result;
                    }
                }
            }

            PlaybackInfoResponse playbackInfo = null;
            string playSessionId = null;
            if (!isOffline)
            {
                playbackInfo = await apiClient.GetPlaybackInfo(new PlaybackInfoRequest
                {
                    Id = options.ItemId,
                    UserId = apiClient.CurrentUserId,
                    MaxStreamingBitrate = options.MaxBitrate,
                    MediaSourceId = options.MediaSourceId

                }).ConfigureAwait(false);

                if (playbackInfo.ErrorCode.HasValue)
                {
                    throw new PlaybackException { ErrorCode = playbackInfo.ErrorCode.Value };
                }

                options.MediaSources = playbackInfo.MediaSources;
                playSessionId = playbackInfo.PlaySessionId;
            }

            var streamInfo = streamBuilder.BuildAudioItem(options);
            EnsureSuccess(streamInfo);

            if (!isOffline)
            {
                var liveMediaSource = await GetLiveStreamInfo(playSessionId, streamInfo.MediaSource, options, apiClient).ConfigureAwait(false);

                if (liveMediaSource != null)
                {
                    options.MediaSources = new List<MediaSourceInfo> { liveMediaSource }.ToArray();
                    streamInfo = GetStreamBuilder().BuildAudioItem(options);
                    EnsureSuccess(streamInfo);
                }
            }

            if (playbackInfo != null)
            {
                streamInfo.AllMediaSources = playbackInfo.MediaSources.ToList();
                streamInfo.PlaySessionId = playbackInfo.PlaySessionId;
            }

            return streamInfo;
        }

        /// <summary>
        /// Sets the live stream.
        /// </summary>
        /// <param name="mediaSource">The media source.</param>
        /// <param name="options">The options.</param>
        /// <param name="apiClient">The API client.</param>
        /// <returns>Task.</returns>
        private async Task<MediaSourceInfo> GetLiveStreamInfo(string playSessionId, MediaSourceInfo mediaSource, AudioOptions options, IApiClient apiClient)
        {
            if (mediaSource.RequiresOpening)
            {
                var liveStreamResponse = await apiClient.OpenLiveStream(new LiveStreamRequest(options)
                {
                    OpenToken = mediaSource.OpenToken,
                    UserId = apiClient.CurrentUserId,
                    PlaySessionId = playSessionId

                }, CancellationToken.None).ConfigureAwait(false);

                return liveStreamResponse.MediaSource;
            }

            return null;
        }

        /// <summary>
        /// Changes the video stream.
        /// </summary>
        /// <param name="currentInfo">The current information.</param>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="options">The options.</param>
        /// <param name="apiClient">The API client.</param>
        /// <returns>Task&lt;StreamInfo&gt;.</returns>
        /// <exception cref="MediaBrowser.Model.Dlna.PlaybackException"></exception>
        public async Task<StreamInfo> ChangeVideoStream(StreamInfo currentInfo, string serverId, VideoOptions options, IApiClient apiClient)
        {
            await StopStranscoding(currentInfo, apiClient).ConfigureAwait(false);

            if (currentInfo.AllMediaSources != null)
            {
                options.MediaSources = currentInfo.AllMediaSources.ToArray();
            }

            var streamInfo = await GetVideoStreamInfoInternal(serverId, options).ConfigureAwait(false);
            streamInfo.PlaySessionId = currentInfo.PlaySessionId;
            streamInfo.AllMediaSources = currentInfo.AllMediaSources;
            return streamInfo;
        }

        /// <summary>
        /// Gets the video stream information.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="options">The options.</param>
        /// <param name="isOffline">if set to <c>true</c> [is offline].</param>
        /// <param name="apiClient">The API client.</param>
        /// <returns>Task&lt;StreamInfo&gt;.</returns>
        public async Task<StreamInfo> GetVideoStreamInfo(string serverId, VideoOptions options, bool isOffline, IApiClient apiClient)
        {
            PlaybackInfoResponse playbackInfo = null;
            string playSessionId = null;

            if (!isOffline)
            {
                playbackInfo = await apiClient.GetPlaybackInfo(new PlaybackInfoRequest
                {
                    Id = options.ItemId,
                    UserId = apiClient.CurrentUserId,
                    MaxStreamingBitrate = options.MaxBitrate,
                    MediaSourceId = options.MediaSourceId,
                    AudioStreamIndex = options.AudioStreamIndex,
                    SubtitleStreamIndex = options.SubtitleStreamIndex

                }).ConfigureAwait(false);

                if (playbackInfo.ErrorCode.HasValue)
                {
                    throw new PlaybackException { ErrorCode = playbackInfo.ErrorCode.Value };
                }

                options.MediaSources = playbackInfo.MediaSources;
                playSessionId = playbackInfo.PlaySessionId;
            }

            var streamInfo = await GetVideoStreamInfoInternal(serverId, options).ConfigureAwait(false);

            if (!isOffline)
            {
                var liveMediaSource = await GetLiveStreamInfo(playSessionId, streamInfo.MediaSource, options, apiClient).ConfigureAwait(false);

                if (liveMediaSource != null)
                {
                    options.MediaSources = new List<MediaSourceInfo> { liveMediaSource }.ToArray();
                    streamInfo = GetStreamBuilder().BuildVideoItem(options);
                    EnsureSuccess(streamInfo);
                }
            }

            if (playbackInfo != null)
            {
                streamInfo.AllMediaSources = playbackInfo.MediaSources.ToList();
                streamInfo.PlaySessionId = playbackInfo.PlaySessionId;
            }

            return streamInfo;
        }

        private async Task<StreamInfo> GetVideoStreamInfoInternal(string serverId, VideoOptions options)
        {
            var streamBuilder = GetStreamBuilder();

            var localItem = await _localAssetManager.GetLocalItem(serverId, options.ItemId);

            if (localItem != null)
            {
                var localMediaSource = localItem.Item.MediaSources[0];

                // Use the local media source, unless a specific server media source was requested
                if (string.IsNullOrWhiteSpace(options.MediaSourceId) ||
                    string.Equals(localMediaSource.Id, options.MediaSourceId,
                    StringComparison.OrdinalIgnoreCase))
                {
                    // Finally, check to make sure the local file is actually available at this time
                    var fileExists = await _localAssetManager.FileExists(localMediaSource.Path).ConfigureAwait(false);

                    if (fileExists)
                    {
                        options.MediaSources = localItem.Item.MediaSources.ToArray();

                        var result = streamBuilder.BuildVideoItem(options);
                        if (result == null)
                        {
                            _logger.Warn("LocalItem returned no compatible streams. Will dummy up a StreamInfo to force it to direct play.");
                            var mediaSource = localItem.Item.MediaSources.First();
                            result = GetForcedDirectPlayStreamInfo(DlnaProfileType.Video, options, mediaSource);
                        }
                        result.PlayMethod = PlayMethod.DirectPlay;
                        return result;
                    }
                }
            }

            var streamInfo = streamBuilder.BuildVideoItem(options);
            EnsureSuccess(streamInfo);
            return streamInfo;
        }

        private StreamInfo GetForcedDirectPlayStreamInfo(DlnaProfileType mediaType, AudioOptions options, MediaSourceInfo mediaSource)
        {
            return new StreamInfo
            {
                ItemId = options.ItemId,
                MediaType = mediaType,
                MediaSource = mediaSource,
                RunTimeTicks = mediaSource.RunTimeTicks,
                Context = options.Context,
                DeviceProfile = options.Profile,
                Container = mediaSource.Container,
                PlayMethod = PlayMethod.DirectPlay
            };
        }

        private void EnsureSuccess(StreamInfo info)
        {
            if (info == null)
            {
                throw new PlaybackException
                {
                    ErrorCode = PlaybackErrorCode.NoCompatibleStream
                };
            }
        }

        /// <summary>
        /// Reports playback start
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="isOffline">if set to <c>true</c> [is offline].</param>
        /// <param name="apiClient">The current apiClient. It can be null if offline</param>
        /// <returns>Task.</returns>
        public async Task ReportPlaybackStart(PlaybackStartInfo info, bool isOffline, IApiClient apiClient)
        {
            if (!isOffline)
            {
                await apiClient.ReportPlaybackStartAsync(info).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Reports playback progress
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="streamInfo">The stream information.</param>
        /// <param name="isOffline">if set to <c>true</c> [is offline].</param>
        /// <param name="apiClient">The current apiClient. It can be null if offline</param>
        /// <returns>Task.</returns>
        public async Task ReportPlaybackProgress(PlaybackProgressInfo info, StreamInfo streamInfo, bool isOffline, IApiClient apiClient)
        {
            if (!isOffline)
            {
                if (streamInfo != null)
                {
                    info.PlaySessionId = streamInfo.PlaySessionId;

                    if (streamInfo.MediaSource != null)
                    {
                        info.LiveStreamId = streamInfo.MediaSource.LiveStreamId;
                    }
                }

                await apiClient.ReportPlaybackProgressAsync(info).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Reports playback progress
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="streamInfo">The stream information.</param>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="isOffline">if set to <c>true</c> [is offline].</param>
        /// <param name="apiClient">The current apiClient. It can be null if offline</param>
        /// <returns>Task.</returns>
        public async Task ReportPlaybackStopped(PlaybackStopInfo info, StreamInfo streamInfo, string serverId, string userId, bool isOffline, IApiClient apiClient)
        {
            if (isOffline)
            {
                var action = new UserAction
                {
                    Date = DateTime.UtcNow,
                    ItemId = info.ItemId,
                    PositionTicks = info.PositionTicks,
                    ServerId = serverId,
                    Type = UserActionType.PlayedItem,
                    UserId = userId
                };

                await _localAssetManager.RecordUserAction(action).ConfigureAwait(false);
                return;
            }

            if (streamInfo != null)
            {
                info.PlaySessionId = streamInfo.PlaySessionId;

                if (streamInfo.MediaSource != null)
                {
                    info.LiveStreamId = streamInfo.MediaSource.LiveStreamId;
                }
            }

            // Put a try/catch here because we need to stop transcoding regardless
            try
            {
                await apiClient.ReportPlaybackStoppedAsync(info).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error in ReportPlaybackStoppedAsync", ex);
            }
        }

        private async Task StopStranscoding(StreamInfo streamInfo, IApiClient apiClient)
        {
            if (streamInfo.MediaType != DlnaProfileType.Video)
            {
                return;
            }

            if (streamInfo.PlayMethod != PlayMethod.Transcode)
            {
                return;
            }

            var playSessionId = streamInfo.PlaySessionId;

            try
            {
                await apiClient.StopTranscodingProcesses(_device.DeviceId, playSessionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Error in StopStranscoding", ex);
            }
        }
    }
}
