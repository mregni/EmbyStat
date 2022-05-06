import {createContext, useEffect, useState} from 'react';

import {Job, JobLogLine, JobProgress} from '../../models/jobs';
import {fireJob, getAllJobs, updateTrigger} from '../../services';

export interface JobsContextProps {
  fireJobById: (id: string) => Promise<void>;
  onProgressReceived: (progress: JobProgress) => void;
  onLogReceived: (line: JobLogLine) => void;
  updateJobTrigger: (id: string, cron: string) => Promise<boolean>;
  jobs: Job[];
  logLines: JobLogLine[];
}

export const JobsContext = createContext<JobsContextProps>(null!);

export const useJobsContext = (): JobsContextProps => {
  const [jobs, setJobs] = useState<Job[]>([]);
  const [logLines, setLogLines] = useState<JobLogLine[]>([]);
  const [loaded, setLoaded] = useState(false);

  const load = async (): Promise<void> => {
    const jobs = await getAllJobs();
    setJobs(jobs);
  };

  const fireJobById = async (id: string) => {
    await fireJob(id);
    const firedJob = jobs.find((job) => job.id === id);
    if (firedJob !== undefined) {
      firedJob.state = 4; // Set processing state to Preparing
      setJobs((prev) => prev.map((job: Job) => job.id === id ? firedJob as Job : job));
    }
  };

  const onLogReceived = (line: JobLogLine) => {
    if (logLines.length >= 19) {
      const tempLines = [...logLines];
      tempLines.shift();
      setLogLines([...tempLines, line]);
      return;
    }

    setLogLines((prev) => [...prev, line]);
  };

  const onProgressReceived = (progress: JobProgress) => {
    const currentJob = jobs.find((job) => job.id === progress.id);
    if (currentJob !== undefined) {
      currentJob.state = progress.state;
      currentJob.startTimeUtcIso = progress.startTimeUtc;
      currentJob.endTimeUtcIso = progress.endTimeUtc;
      currentJob.currentProgressPercentage = progress.currentProgressPercentage;
      setJobs((prev) => prev.map((job: Job) => job.id === progress.id ? currentJob as Job : job));
    }
  };

  const updateJobTrigger = async (id: string, cron: string): Promise<boolean> => {
    try {
      await updateTrigger(id, cron);
      await load();
      return Promise.resolve(true);
    } catch (error) {
      return Promise.resolve(false);
    }
  };

  useEffect(() => {
    if (!loaded) {
      load();
      setLoaded(true);
    }
  }, []);


  return {
    fireJobById, onLogReceived, updateJobTrigger,
    onProgressReceived, jobs, logLines,
  };
};
