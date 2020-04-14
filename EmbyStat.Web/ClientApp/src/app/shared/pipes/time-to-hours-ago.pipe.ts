import * as moment from 'moment';

import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'convertToHoursAgo' })
export class DateToHoursAgoPipe implements PipeTransform {
  transform(value: any, to = moment()): number {
    const from = moment(value);
    if (!(to instanceof moment)) {
      to = moment(to);
    }

    const milliseconds = to.diff(from);
    const duration = moment.duration(milliseconds);
    return Math.floor(duration.asHours());
  }
}
