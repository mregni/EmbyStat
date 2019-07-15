import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonPosterComponent } from './person-poster.component';

describe('PersonPosterComponent', () => {
  let component: PersonPosterComponent;
  let fixture: ComponentFixture<PersonPosterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PersonPosterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonPosterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
