import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { AppDispatch, AppThunk } from '.';

import { RootState } from './RootReducer';
import { JobsContainer, Job } from '../shared/models/jobs';
import { getAllJobs } from '../shared/services/JobService';

const initialState: JobsContainer = {
  jobs: [],
  isLoaded: false,
};

const jobSlice = createSlice({
  name: "jobs",
  initialState,
  reducers: {
    receiveJobs(state, action: PayloadAction<Job[]>) {
      return {
        jobs: action.payload,
        isLoaded: true
      };
    },
    alreadyLoaded(state, action: PayloadAction) {
      return state;
    },
    updateJob(state, action: PayloadAction<Job>) {
      const jobIndex = state.jobs.findIndex(x => x.id === action.payload.id);
      if (jobIndex !== -1) {
        state.jobs[jobIndex] = action.payload;
      }
      return state;
    }
  },
});

export const loadJobs = (): AppThunk => async (dispatch: AppDispatch, getState: () => RootState) => {
  if (!getState().jobs.isLoaded) {
    const jobs = await getAllJobs();
    dispatch(jobSlice.actions.receiveJobs(jobs));
  } else {
    dispatch(jobSlice.actions.alreadyLoaded());
  }
};

export default jobSlice;
