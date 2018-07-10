import { LoaderActions, LoaderActiontypes } from './actions.loader';
import { LoadingState } from '../model/loadingState';
import { ApplicationState } from '../../../../states/app.state';

const INITIAL_STATE: LoadingState = {
  isShowGeneralLoading: false,
  isShowGraphsLoading: false
};

export function LoadingReducer(state: LoadingState = INITIAL_STATE, action: LoaderActions) {
  switch (action.type) {
    case LoaderActiontypes.HIDESHOWGENERAL:
      return { ...state, isShowGeneralLoading: false };
    case LoaderActiontypes.SHOWSHOWGENERAL:
      return { ...state, isShowGeneralLoading: true };
    case LoaderActiontypes.HIDEHOWCHARTS:
      return { ...state, isShowGraphsLoading: false };
    case LoaderActiontypes.SHOWSHOWCHARTS:
      return { ...state, isShowGraphsLoading: true };
  default:
    return state;
  }
}

export namespace LoaderQuery {
  export const isShowGeneralLoading = (state: ApplicationState) => state.loading.isShowGeneralLoading;
  export const isShowGraphsLoading = (state: ApplicationState) => state.loading.isShowGraphsLoading;
}
