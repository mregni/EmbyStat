import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { AppDispatch, AppThunk } from '.';
import { MovieStatistics, MovieStatisticsContainer } from '../shared/models/movie';

import { getStatistics } from '../shared/services/MovieService';
import { RootState } from './RootReducer';

const initialState: MovieStatisticsContainer = {
  statistics: {
    cards: [],
    charts: [],
    noImdb: [],
    noPrimary: [],
    topCards: [],
    shorts: [],
    people: {
      cards: [],
      mostFeaturedActorsPerGenre: [],
      posters: [],
    }
  },
  isLoaded: false,
};

const movieSlice = createSlice({
  name: "movie",
  initialState,
  reducers: {
    receiveStatistics(state, action: PayloadAction<MovieStatistics>) {
      return {
        statistics: action.payload,
        isLoaded: true
      };
    },
    alreadyLoaded(state, action: PayloadAction) {
      return state;
    }
  },
});

export const loadStatistics = (forced: boolean = false): AppThunk => async (dispatch: AppDispatch, getState: () => RootState) => {
  if (!getState().movies.isLoaded || forced) {
    const statistics = await getStatistics();
    dispatch(movieSlice.actions.receiveStatistics(statistics));
  } else {
    dispatch(movieSlice.actions.alreadyLoaded());
  }
};

export default movieSlice.reducer;
