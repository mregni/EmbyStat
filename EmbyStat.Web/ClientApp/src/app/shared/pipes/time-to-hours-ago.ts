import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({ name: 'convertToHoursAgo' })
export class DateToHoursAgo implements PipeTransform {
  transform(value: any, to = moment()): number {
    const from = moment(value);

    if (to instanceof moment) {

    } else {
      to = moment(to);
    }

    const milliseconds = to.diff(from);
    const duration = moment.duration(milliseconds);
    return Math.floor(duration.asHours());
  }
}
