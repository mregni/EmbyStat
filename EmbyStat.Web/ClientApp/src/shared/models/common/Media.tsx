export interface Media {
  id: string;
  dateCreated: string | null;
  banner: string;
  logo: string;
  primary: string;
  thumb: string;
  name: string;
  parentId: string;
  path: string;
  premiereDate: string | null;
  productionYear: number | null;
  sortName: string;
  collectionId: string;
}

export interface ExtraPerson {
  id: string;
  type: number;
  name: string;
}

export interface Extra extends Media {
  communityRating: number | undefined;
  imdb: string;
  tmdb: string;
  tvdb: string;
  runTime: number | null;
  officialRating: string;
  people: ExtraPerson[];
  genres: string[];
  lastUpdated: Date | string;
}

export interface MediaSource {
  id: string;
  bitRate: number | null;
  container: string;
  path: string;
  protocol: string;
  runTimeTicks: number | null;
  sizeInMb: number;
  bitDepth: number | null;
  codec: string;
  videoRange: string;
}

export interface VideoStream extends Stream {
  aspectRatio: string;
  averageFrameRate: number | null;
  bitRate: number | null;
  BitDepth: number | null;
  channels: number | null;
  height: number | null;
  codec: string | null;
  videoRange: string | null;
  width: number | null;
}

export interface Stream {
  id: string;
  isDefault: boolean;
  language: string;
}

export interface AudioStream extends Stream {
  bitRate: number | null;
  channelLayout: string;
  channels: number | null;
  codec: string;
  sampleRate: number | null;
}

export interface SubtitleStream extends Stream {
  codec: string;
  displayTitle: string;
}

export interface Video extends Extra {
  container: string;
  mediaType: string;
  mediaSources: MediaSource[];
  audioStreams: AudioStream[];
  subtitleStreams: SubtitleStream[];
  videoStreams: VideoStream[];
  video3DFormat: number;
}

export interface VirtualEpisode {
  indexNumber: number;
  name: string;
  id: string;
  premiereDate: Date;
}
export interface VirtualSeason {
  indexNumber: number;
  episodes: VirtualEpisode[];
}

export interface Show extends Extra {
  cumulativeRunTime?: number;
  status: string;
  seasonCount: number;
  missingSeasons: VirtualSeason[];
  episodeCount: number;
  specialEpisodeCount: number;
  sizeInMb: number;
}
