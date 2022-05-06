export interface LibraryStepProps {
  type: 'movie' | 'show';
}

export interface ValidationHandle {
  validate: () => Promise<boolean>;
}

export interface StepProps {
  handleNext?: () => void;
}

export interface ValidationHandleWithSave {
  saveChanges: () => void;
  validate: () => Promise<boolean>;
}
