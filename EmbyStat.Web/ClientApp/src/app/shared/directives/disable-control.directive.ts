import { Component, Input } from '@angular/core';
import { NgControl } from '@angular/forms';

@Component({
  selector: 'es-disable-form',
  styles: [`
      fieldset {
        display: block;
        margin: unset;
        padding: unset;
        border: unset;
      }
    `],
  template: `
      <fieldset [disabled]="state">
        <ng-content></ng-content>
      </fieldset>
    `
})
export class DisableFormComponent {
  // tslint:disable-next-line:no-input-rename
  @Input('disableForm') state: boolean;
}
