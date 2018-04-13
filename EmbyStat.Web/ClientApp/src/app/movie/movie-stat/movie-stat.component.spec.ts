import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MovieStatComponent } from './movie-stat.component';

describe('MovieStatComponent', () => {
  let component: MovieStatComponent;
  let fixture: ComponentFixture<MovieStatComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MovieStatComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MovieStatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
