import { Moment } from 'moment';

export class Job {
    state: number;
    currentProgressPercentage: number;
    id: string;
    startTimeUtc: Moment;
    endTimeUtc: Moment;
    title: string;
    description: string;
    trigger: string;
}
