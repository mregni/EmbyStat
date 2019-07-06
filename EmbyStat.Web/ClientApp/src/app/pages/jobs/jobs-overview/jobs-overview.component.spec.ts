import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { JobsOverviewComponent } from './jobs-overview.component';

describe('JobsOverviewComponent', () => {
  let component: JobsOverviewComponent;
  let fixture: ComponentFixture<JobsOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JobsOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobsOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
