using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Session;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emby.ApiClient.Model;

namespace Emby.ApiClient.Playback
{
    public interface IPlaybackManager
    {
        /// <summary>
        /// Gets the pre playback selectable audio streams.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;IEnumerable&lt;MediaStream&gt;&gt;.</returns>
        Task<IEnumerable<MediaStream>> GetPrePlaybackSelectableAudioStreams(string serverId, VideoOptions options);

        /// <summary>
        /// Gets the pre playback selectable subtitle streams.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;IEnumerable&lt;MediaStream&gt;&gt;.</returns>
        Task<IEnumerable<MediaStream>> GetPrePlaybackSelectableSubtitleStreams(string serverId, VideoOptions options);

        /// <summary>
        /// Gets the in playback selectable audio streams.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>IEnumerable&lt;MediaStream&gt;.</returns>
        IEnumerable<MediaStream> GetInPlaybackSelectableAudioStreams(StreamInfo info);

        /// <summary>
        /// Gets the in playback selectable subtitle streams.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>IEnumerable&lt;MediaStream&gt;.</returns>
        IEnumerable<MediaStream> GetInPlaybackSelectableSubtitleStreams(StreamInfo info);

        /// <summary>
        /// Gets the audio stream information.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="options">The options.</param>
        /// <param name="isOffline">if set to <c>true</c> [is offline].</param>
        /// <param name="apiClient">The API client.</param>
        /// <returns>Task&lt;StreamInfo&gt;.</returns>
        Task<StreamInfo> GetAudioStreamInfo(string serverId, AudioOptions options, bool isOffline, IApiClient apiClient);

        /// <summary>
        /// Gets the video stream information.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="options">The options.</param>
        /// <param name="isOffline">if set to <c>true</c> [is offline].</param>
        /// <param name="apiClient">The API client.</param>
        /// <returns>Task&lt;StreamInfo&gt;.</returns>
        Task<StreamInfo> GetVideoStreamInfo(string serverId, VideoOptions options, bool isOffline, IApiClient apiClient);

        /// <summary>
        /// Changes the video stream.
        /// </summary>
        /// <param name="currentInfo">The current information.</param>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="options">The options.</param>
        /// <param name="apiClient">The API client.</param>
        /// <returns>Task&lt;StreamInfo&gt;.</returns>
        Task<StreamInfo> ChangeVideoStream(StreamInfo currentInfo, string serverId, VideoOptions options, IApiClient apiClient);

        /// <summary>
        /// Reports playback start
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="isOffline">if set to <c>true</c> [is offline].</param>
        /// <param name="apiClient">The current apiClient. It can be null if offline</param>
        /// <returns>Task.</returns>
        Task ReportPlaybackStart(PlaybackStartInfo info, bool isOffline, IApiClient apiClient);

        /// <summary>
        /// Reports playback progress
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="streamInfo">The stream information.</param>
        /// <param name="isOffline">if set to <c>true</c> [is offline].</param>
        /// <param name="apiClient">The current apiClient. It can be null if offline</param>
        /// <returns>Task.</returns>
        Task ReportPlaybackProgress(PlaybackProgressInfo info, StreamInfo streamInfo, bool isOffline, IApiClient apiClient);

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
        Task ReportPlaybackStopped(PlaybackStopInfo info, StreamInfo streamInfo, string serverId, string userId, bool isOffline, IApiClient apiClient);
    }
}