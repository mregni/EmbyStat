import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingsEmbyComponent } from './settings-emby.component';

describe('SettingsEmbyComponent', () => {
  let component: SettingsEmbyComponent;
  let fixture: ComponentFixture<SettingsEmbyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SettingsEmbyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingsEmbyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
