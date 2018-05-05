import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MovieChartsComponent } from './movie-charts.component';

describe('MovieChartsComponent', () => {
  let component: MovieChartsComponent;
  let fixture: ComponentFixture<MovieChartsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MovieChartsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MovieChartsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
