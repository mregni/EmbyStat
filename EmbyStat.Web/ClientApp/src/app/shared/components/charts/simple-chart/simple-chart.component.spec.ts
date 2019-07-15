import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SimpleChartComponent } from './simple-chart.component';

describe('SimpleBarChartComponent', () => {
  let component: SimpleChartComponent;
  let fixture: ComponentFixture<SimpleChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SimpleChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SimpleChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
