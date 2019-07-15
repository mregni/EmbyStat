export class Job {
    state: number;
    currentProgressPercentage: number;
    id: string;
    startTimeUtc: Date;
    endTimeUtc: Date;
    title: string;
    description: string;
    trigger: string;
}
