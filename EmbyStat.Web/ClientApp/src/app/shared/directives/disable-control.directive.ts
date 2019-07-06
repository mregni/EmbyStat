import { Directive, Input } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
// tslint:disable-next-line: directive-selector
    selector: '[disableControl]'
})
export class DisableControlDirective {
    @Input() set disableControl(condition: boolean) {
        const action = condition ? 'disable' : 'enable';
        this.ngControl.control[action]();
    }

    constructor(private ngControl: NgControl) { }
}
