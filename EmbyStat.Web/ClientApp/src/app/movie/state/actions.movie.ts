import { Action } from '@ngrx/store';
import { SmallStat } from "../../shared/models/smallStat";

export enum MovieTypes {
  LOAD_STATS_GENERAL = '[MovieGeneralStat] Load General Movie Stats',
  LOAD_STATS_GENERAL_SUCCESS = '[MovieGeneralStat] Load General Movie Stats',
  NOT_NEEDED = '[MovieGeneralStat] Not Needed',
  RESET_LOADED_STATE = '[MovieGeneralStat] Reset Loaded State'
}

export class LoadGeneralStatsAction implements Action {
  readonly type = MovieTypes.LOAD_STATS_GENERAL;
  constructor(public payload = null) { }
}

export class LoadGeneralStatsSuccessAction implements Action {
  readonly type = MovieTypes.LOAD_STATS_GENERAL_SUCCESS;
  constructor(public payload: SmallStat[]) { }
}

export type MovieActions = LoadGeneralStatsAction | LoadGeneralStatsSuccessAction;
