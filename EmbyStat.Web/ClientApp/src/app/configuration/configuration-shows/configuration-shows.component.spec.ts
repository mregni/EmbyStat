import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurationShowsComponent } from './configuration-shows.component';

describe('ConfigurationShowsComponent', () => {
  let component: ConfigurationShowsComponent;
  let fixture: ComponentFixture<ConfigurationShowsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigurationShowsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigurationShowsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
