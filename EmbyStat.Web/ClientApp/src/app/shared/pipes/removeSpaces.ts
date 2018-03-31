import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'removeSpaces' })
export class RemoveSpaces implements PipeTransform {
  transform(value) {
    return value.replace(/ /g, "");
  }
}
