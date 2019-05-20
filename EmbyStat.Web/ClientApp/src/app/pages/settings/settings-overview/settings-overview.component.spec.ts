import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingsOverviewComponent } from './settings-overview.component';

describe('SettingsOverviewComponent', () => {
  let component: SettingsOverviewComponent;
  let fixture: ComponentFixture<SettingsOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SettingsOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingsOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
