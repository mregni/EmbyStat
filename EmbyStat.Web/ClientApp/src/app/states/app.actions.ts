import { Action } from "@ngrx/store";

export enum AppActionTypes {
  NOOP = '[App] NOOP'
}

export class NoopAction implements Action {
  readonly type = AppActionTypes.NOOP;
}

export type AppActions = NoopAction;
