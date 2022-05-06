export interface JobProgress {
  state: number;
  currentProgressPercentage: number;
  id: string;
  startTimeUtc: string;
  endTimeUtc: string;
}
