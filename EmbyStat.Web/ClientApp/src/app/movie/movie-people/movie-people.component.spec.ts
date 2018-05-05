import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MoviePeopleComponent } from './movie-people.component';

describe('MoviePeopleComponent', () => {
  let component: MoviePeopleComponent;
  let fixture: ComponentFixture<MoviePeopleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MoviePeopleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MoviePeopleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
