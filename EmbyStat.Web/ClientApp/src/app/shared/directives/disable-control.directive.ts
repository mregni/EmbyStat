import { Component, Input } from '@angular/core';
import { NgControl } from '@angular/forms';

@Component({
    selector: '[disableForm]',
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
    @Input('disableForm') state: boolean;
    constructor() { }
}
