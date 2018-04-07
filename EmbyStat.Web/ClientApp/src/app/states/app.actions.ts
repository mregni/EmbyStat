import { Action } from "@ngrx/store";

export enum AppActionTypes {
  ERROR = '[Error] Effect Error',
  NOOP = '[App] NOOP',
  RESET_ALL_LOAD_STATES = '[App] Reset all load states'
}

export class EffectError implements Action {
  readonly type = AppActionTypes.ERROR;
  constructor(payload: Error = null) {}
}

export class NoopAction implements Action {
  readonly type = AppActionTypes.NOOP;
}

export type AppActions = EffectError | NoopAction;
