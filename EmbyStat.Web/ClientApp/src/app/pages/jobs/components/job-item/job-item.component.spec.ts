import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { JobItemComponent } from './job-item.component';

describe('JobItemComponent', () => {
  let component: JobItemComponent;
  let fixture: ComponentFixture<JobItemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JobItemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
