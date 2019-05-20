import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingsMovieComponent } from './settings-movie.component';

describe('SettingsMovieComponent', () => {
  let component: SettingsMovieComponent;
  let fixture: ComponentFixture<SettingsMovieComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SettingsMovieComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingsMovieComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
