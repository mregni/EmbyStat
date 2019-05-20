import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingsUpdateComponent } from './settings-update.component';

describe('SettingsUpdateComponent', () => {
  let component: SettingsUpdateComponent;
  let fixture: ComponentFixture<SettingsUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SettingsUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingsUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
