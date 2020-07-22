import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { AppThunk } from '.';

const initialState: number = 0;

const serverStatusSlice = createSlice({
  name: 'serverStatus',
  initialState,
  reducers: {
    receivePingResult(state, action: PayloadAction<number>) {
      return action.payload;
    }
  },
});

export const receivePingUpdate = (missedPings: number): AppThunk => async (dispatch) => {
  dispatch(serverStatusSlice.actions.receivePingResult(missedPings));
};

export default serverStatusSlice;
