import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { AppDispatch, AppThunk } from '.';
import { ServerState, UpdateSuccessFull } from '../shared/models/embystat';
import { RootState } from './RootReducer';

const initialState: ServerState = {
  missedPings: 0,
  updating: false,
  updateSuccesfull: UpdateSuccessFull.unknown,
}

const serverStatusSlice = createSlice({
  name: 'serverStatus',
  initialState,
  reducers: {
    updateState(state, action: PayloadAction<ServerState>) {
      return {
        ...action.payload,
      }
    },
  },
});

export const receivePingUpdate = (missedPings: number): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const state = { ...getState().serverStatus };
  console.log(state);
  state.missedPings = missedPings;
  console.log(state);
  dispatch(serverStatusSlice.actions.updateState(state));
};

export const receivedServerUpdateState = (updating: boolean): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const state = { ...getState().serverStatus };
  state.updating = updating;
  dispatch(serverStatusSlice.actions.updateState(state));
};

export const receivedUpdateFinishedState = (succesfull: UpdateSuccessFull): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const state = { ...getState().serverStatus };
  state.updateSuccesfull = succesfull;
  dispatch(serverStatusSlice.actions.updateState(state));
};

export default serverStatusSlice;
