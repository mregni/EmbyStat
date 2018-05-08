import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurationMoviesComponent } from './configuration-movies.component';

describe('ConfigurationMoviesComponent', () => {
  let component: ConfigurationMoviesComponent;
  let fixture: ComponentFixture<ConfigurationMoviesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigurationMoviesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigurationMoviesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
