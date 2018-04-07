import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({ name: 'convertToMinutesAgo' })
export class DateToMinutesAgo implements PipeTransform {
  transform(value: any, to = moment()): number {
    var from = moment(value);

    if (to instanceof moment) {

    } else {
      to = moment(to);
    }

    var milliseconds = to.diff(from);
    var duration = moment.duration(milliseconds);
    return Math.floor(duration.asMinutes()) % 60;
  }
}




