import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowPosterComponent } from './show-poster.component';

describe('ShowPosterComponent', () => {
  let component: ShowPosterComponent;
  let fixture: ComponentFixture<ShowPosterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowPosterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowPosterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
