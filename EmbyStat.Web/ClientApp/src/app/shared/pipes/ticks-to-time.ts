import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'convertToTime' })
export class TicksToTime implements PipeTransform {
  transform(value: number, withSeconds = false): string {
    var seconds = value / 10000000;
    var hour = Math.floor(seconds / 3600);
    var minute = Math.floor((seconds / 60) % 60);
    var second = Math.floor(seconds) % 60;

    if (!withSeconds) {
      return pad2(hour) + ':' + pad2(minute);
    } else {
      return pad2(hour) + ':' + pad2(minute) + ':' + pad2(second);
    }

    function pad2(number) {
      number = '0' + number;
      return number.substr(number.length - 2);
    }
  }
}




