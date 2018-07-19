import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NoTypeFoundComponent } from './no-type-found.component';

describe('NoTypeFoundComponent', () => {
  let component: NoTypeFoundComponent;
  let fixture: ComponentFixture<NoTypeFoundComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NoTypeFoundComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NoTypeFoundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
