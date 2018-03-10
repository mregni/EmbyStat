import { Action } from "@ngrx/store";

export enum AppActionTypes {
  ERROR = '[Error] Effect Error'
}

export class EffectError implements Action {
  readonly type = AppActionTypes.ERROR;
  constructor(payload: Error = null) {}
}

export type AppActions = EffectError;
