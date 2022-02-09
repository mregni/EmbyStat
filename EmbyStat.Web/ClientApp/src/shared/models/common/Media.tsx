export interface Media {
  id: string;
  dateCreated: Date | string | null;
  banner: string;
  logo: string;
  primary: string;
  thumb: string;
  backdrop: string;
  name: string;
  parentId: string;
  path: string;
  premiereDate: Date | string | null;
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
  communityRating: number | null;
  imdb: string;
  tmdb: string;
  tvdb: string;
  runTimeTicks: number | null;
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

export interface VideoStream {
  id: string;
  aspectRatio: string;
  averageFrameRate: number | null;
  bitRate: number | null;
  BitDepth: number | null;
  channels: number | null;
  height: number | null;
  codec: string | null;
  videoRange: string | null;
  language: string;
  width: number | null;
}

export interface AudioStream {
  id: string;
  bitRate: number | null;
  channelLayout: string;
  channels: number | null;
  codec: string;
  language: string;
  sampleRate: number | null;
  isDefault: boolean;
}

export interface SubtitleStream {
  id: string;
  codec: string;
  displayTitle: string;
  isDefault: boolean;
  language: string;
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

export interface Movie extends Video {
  originalTitle: string;
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
  cumulativeRunTimeTicks?: number;
  status: string;
  seasonCount: number;
  missingSeasons: VirtualSeason[];
  episodeCount: number;
  specialEpisodeCount: number;
  sizeInMb: number;
}
