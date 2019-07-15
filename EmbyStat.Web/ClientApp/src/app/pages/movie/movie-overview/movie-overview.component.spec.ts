import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MovieOverviewComponent } from './movie-overview.component';

describe('MovieOverviewComponent', () => {
  let component: MovieOverviewComponent;
  let fixture: ComponentFixture<MovieOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MovieOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MovieOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
