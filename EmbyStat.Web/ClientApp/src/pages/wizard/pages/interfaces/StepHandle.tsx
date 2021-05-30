export interface ValidationHandle {
  validate: () => Promise<boolean>;
}
