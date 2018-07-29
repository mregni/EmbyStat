import { LoaderActions, LoaderActiontypes } from './actions.loader';
import { LoadingState } from '../model/loadingState';
import { ApplicationState } from '../../../../states/app.state';

const INITIAL_STATE: LoadingState = {
  isMovieGeneralLoading: false,
  isMovieGraphsLoading: false,
  isMoviePeopleLoading: false,
  isMovieSuspiciousLoading: false
};

export function LoadingReducer(state: LoadingState = INITIAL_STATE, action: LoaderActions) {
  switch (action.type) {
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
  export const isMovieGeneralLoading = (state: ApplicationState) => state.loading.isMovieGeneralLoading;
  export const isMovieGraphsLoading = (state: ApplicationState) => state.loading.isMovieGraphsLoading;
  export const isMoviePeopleLoading = (state: ApplicationState) => state.loading.isMoviePeopleLoading;
  export const isMovieSuspiciousLoading = (state: ApplicationState) => state.loading.isMovieSuspiciousLoading;

}
