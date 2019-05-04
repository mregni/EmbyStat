import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SimpleBarChartComponent } from './simple-bar-chart.component';

describe('SimpleBarChartComponent', () => {
  let component: SimpleBarChartComponent;
  let fixture: ComponentFixture<SimpleBarChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SimpleBarChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SimpleBarChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
