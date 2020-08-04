export interface ServerState {
  missedPings: number;
  updating: boolean;
  updateSuccesfull: UpdateSuccessFull;
}

export enum UpdateSuccessFull {
  unknown = 0,
  SuccesFull = 1,
  Failed = 2,
}