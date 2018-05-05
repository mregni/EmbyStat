import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MovieSuspiciousComponent } from './movie-suspicious.component';

describe('MovieSuspiciousComponent', () => {
  let component: MovieSuspiciousComponent;
  let fixture: ComponentFixture<MovieSuspiciousComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MovieSuspiciousComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MovieSuspiciousComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
