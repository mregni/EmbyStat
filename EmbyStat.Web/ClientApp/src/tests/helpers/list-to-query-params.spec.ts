import { MockComponent, MockPipe } from 'ng-mocks';
import { Observable, of } from 'rxjs';

import { DebugElement } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgProgressModule } from '@ngx-progressbar/core';
import { TranslateModule, TranslatePipe, TranslateService } from '@ngx-translate/core';

import { ListToQueryParam } from '../../app/shared/helpers/list-to-query-param';

describe('ListToQueryParams', () => {
    describe('when a list is passed', () => {
        it('should create map the list', () => {
            const helper = ListToQueryParam.convert('test', ['obj1', 'obj2']);
          expect(helper).toEqual('?test=obj1&test=obj2');
        });
    });
});
