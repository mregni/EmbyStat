import { LoaderActions, LoaderActiontypes } from './actions.loader';
import { LoadingState } from '../model/loadingState';
import { ApplicationState } from '../../../../states/app.state';

const INITIAL_STATE: LoadingState = {
  isShowGeneralLoading: false,
  isShowGraphsLoading: false,
  isShowCollectionLoading: false,
  isMovieGeneralLoading: false,
  isMovieGraphsLoading: false,
  isMoviePeopleLoading: false,
  isMovieSuspiciousLoading: false
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
    case LoaderActiontypes.HIDESHOWCOLLECTION:
      return { ...state, isShowCollectionLoading: false };
    case LoaderActiontypes.HIDESHOWCOLLECTION:
      return { ...state, isShowCollectionLoading: true };
    case LoaderActiontypes.HIDEMOVIEGENERAL:
      return { ...state, isMovieGeneralLoading: false };
    case LoaderActiontypes.SHOWMOVIEGENERAL:
      return { ...state, isMovieGeneralLoading: true };
    case LoaderActiontypes.HIDEMOVIEGRAPHS:
      return { ...state, isMovieGraphsLoading: false };
    case LoaderActiontypes.SHOWMOVIEGRAPHS:
      return { ...state, isMovieGraphsLoading: true };
    case LoaderActiontypes.HIDEMOVIEPEOPLE:
      return { ...state, isMoviePeopleLoading: false };
    case LoaderActiontypes.SHOWMOVIEPEOPLE:
      return { ...state, isMoviePeopleLoading: true };
    case LoaderActiontypes.HIDEMOVIESUSPICIOUS:
      return { ...state, isMovieSuspiciousLoading: false };
    case LoaderActiontypes.SHOWMOVIESUSPICIOUS:
      return { ...state, isMovieSuspiciousLoading: true };
  default:
    return state;
  }
}

export namespace LoaderQuery {
  export const isShowGeneralLoading = (state: ApplicationState) => state.loading.isShowGeneralLoading;
  export const isShowGraphsLoading = (state: ApplicationState) => state.loading.isShowGraphsLoading;
  export const isShowCollectionLoading = (state: ApplicationState) => state.loading.isShowCollectionLoading;
  export const isMovieGeneralLoading = (state: ApplicationState) => state.loading.isMovieGeneralLoading;
  export const isMovieGraphsLoading = (state: ApplicationState) => state.loading.isMovieGraphsLoading;
  export const isMoviePeopleLoading = (state: ApplicationState) => state.loading.isMoviePeopleLoading;
  export const isMovieSuspiciousLoading = (state: ApplicationState) => state.loading.isMovieSuspiciousLoading;

}
