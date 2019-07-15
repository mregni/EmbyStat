import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'toShorterString' })
export class ToShorterStringPipe implements PipeTransform {
  transform(value: string, length: number): string {
    if (!value) {
       return null;
    }

    if (value.length >= length) {
      return value.substr(0, length - 3) + '...';
    }

    return value;
  }
}
