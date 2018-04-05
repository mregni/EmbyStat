import { Trigger } from './trigger';
import { LastExecutionResult } from './lastExecutionResult';

export class Task {
  name: string;
  state: number;
  currentProgressPercentage?: any;
  id: string;
  lastExecutionResult: LastExecutionResult;
  triggers: Trigger[];
  description: string;
  category: string;
}
