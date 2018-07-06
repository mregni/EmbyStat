import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowGeneralComponent } from './show-general.component';

describe('ShowGeneralComponent', () => {
  let component: ShowGeneralComponent;
  let fixture: ComponentFixture<ShowGeneralComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowGeneralComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowGeneralComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
