import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LogsOverviewComponent } from './logs-overview.component';

describe('LogsOverviewComponent', () => {
  let component: LogsOverviewComponent;
  let fixture: ComponentFixture<LogsOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LogsOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LogsOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
