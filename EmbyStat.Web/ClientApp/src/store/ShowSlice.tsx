import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { AppDispatch, AppThunk } from '.';
import { getStatistics } from '../shared/services/ShowService';
import { RootState } from './RootReducer';
import { ShowStatisticsContainer, ShowStatistics } from '../shared/models/show';

const initialState: ShowStatisticsContainer = {
  statistics: {
    cards: [],
    topCards: [],
    barCharts: [],
    pieCharts: [],
    people: {
      cards: [],
      mostFeaturedActorsPerGenreCards: [],
      globalCards: [],
    },
  },
  isLoaded: false,
};

const showSlice = createSlice({
  name: 'show',
  initialState,
  reducers: {
    receiveStatistics(state, action: PayloadAction<ShowStatistics>) {
      return {
        statistics: action.payload,
        isLoaded: true,
      };
    },
    alreadyLoaded(state, action: PayloadAction) {
      return state;
    },
  },
});

export const loadStatistics = (forced: boolean = false): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  if (!getState().shows.isLoaded || forced) {
    const statistics = await getStatistics();
    dispatch(showSlice.actions.receiveStatistics(statistics));
  } else {
    dispatch(showSlice.actions.alreadyLoaded());
  }
};

export default showSlice.reducer;
