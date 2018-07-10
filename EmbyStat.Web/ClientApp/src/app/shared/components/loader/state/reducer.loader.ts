import { LoaderActions, LoaderActiontypes } from './actions.loader';
import { LoadingState } from '../model/loadingState';
import { ApplicationState } from '../../../../states/app.state';

const INITIAL_STATE: LoadingState = {
  isLoading: false
};

export function LoadingReducer(state: LoadingState = INITIAL_STATE, action: LoaderActions) {
  switch (action.type) {
    case LoaderActiontypes.HIDE:
      return { ...state, isLoading: false };
    case LoaderActiontypes.SHOW:
      return { ...state, isLoading: true };
  default:
    return state;
  }
}

export namespace LoaderQuery {
  export const isLoading = (state: ApplicationState) => state.loading.isLoading;
}
