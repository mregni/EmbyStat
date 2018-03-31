import { Action } from "@ngrx/store";

export enum AppActionTypes {
  ERROR = '[Error] Effect Error',
  NOOP = '[App] NOOP'
}

export class EffectError implements Action {
  readonly type = AppActionTypes.ERROR;
  constructor(payload: Error = null) {}
}

export class NoopAction implements Action {
  readonly type = AppActionTypes.NOOP;
}

export type AppActions = EffectError | NoopAction;
