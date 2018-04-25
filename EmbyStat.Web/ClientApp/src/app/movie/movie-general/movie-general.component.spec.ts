import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MovieGeneralComponent } from './movie-general.component';

describe('MovieGeneralComponent', () => {
  let component: MovieGeneralComponent;
  let fixture: ComponentFixture<MovieGeneralComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MovieGeneralComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MovieGeneralComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
