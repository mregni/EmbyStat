import { Job } from './Jobs';

export interface JobsContainer {
  jobs: Job[];
  isLoaded: boolean;
}
