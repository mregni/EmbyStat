import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowOverviewComponent } from './show-overview.component';

describe('ShowOverviewComponent', () => {
  let component: ShowOverviewComponent;
  let fixture: ComponentFixture<ShowOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
