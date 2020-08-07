export interface Job {
  state: number;
  currentProgressPercentage: number;
  id: string;
  startTimeUtcIso: string;
  endTimeUtcIso: string;
  title: string;
  description: string;
  trigger: string;
}
