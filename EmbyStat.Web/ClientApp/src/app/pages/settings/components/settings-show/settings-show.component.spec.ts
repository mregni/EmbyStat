import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingsShowComponent } from './settings-show.component';

describe('SettingsShowComponent', () => {
  let component: SettingsShowComponent;
  let fixture: ComponentFixture<SettingsShowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SettingsShowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingsShowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
