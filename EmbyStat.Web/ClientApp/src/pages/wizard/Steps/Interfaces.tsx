export interface StepProps {
  handleNext?: () => void;
}

export interface LibraryStepProps {
  type: 'movie' | 'show';
}

export interface ValidationHandle {
  validate: () => Promise<boolean>;
}

export interface ValidationHandleWithSave extends ValidationHandle {
  saveChanges: () => void;
}