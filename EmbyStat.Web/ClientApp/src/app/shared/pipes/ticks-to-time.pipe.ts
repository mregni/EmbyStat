import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'convertToTime' })
export class TicksToTimePipe implements PipeTransform {
  transform(value: number, withSeconds = false): string {
    const seconds = value / 10000000;
    const hour = Math.floor(seconds / 3600);
    const minute = Math.floor((seconds / 60) % 60);
    const second = Math.floor(seconds) % 60;

    if (!withSeconds) {
      return `${pad2(hour)}:${pad2(minute)}`;
    } else {
      return `${pad2(hour)}:${pad2(minute)}:${pad2(second)}`;
    }

    function pad2(valueToPad: number) {
      const result = `0${valueToPad}`;
      return result.substr(result.length - 2);
    }
  }
}
