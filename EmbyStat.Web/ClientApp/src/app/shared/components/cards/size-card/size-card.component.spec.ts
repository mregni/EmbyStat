import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SizeCardComponent } from './size-card.component';

describe('SizeCardComponent', () => {
  let component: SizeCardComponent;
  let fixture: ComponentFixture<SizeCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SizeCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SizeCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
