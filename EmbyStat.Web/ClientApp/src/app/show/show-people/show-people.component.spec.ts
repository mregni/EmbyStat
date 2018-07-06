import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowPeopleComponent } from './show-people.component';

describe('ShowPeopleComponent', () => {
  let component: ShowPeopleComponent;
  let fixture: ComponentFixture<ShowPeopleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowPeopleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowPeopleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
