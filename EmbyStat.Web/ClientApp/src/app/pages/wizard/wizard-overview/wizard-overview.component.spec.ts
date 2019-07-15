import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WizardOverviewComponent } from './wizard-overview.component';

describe('WizardOverviewComponent', () => {
  let component: WizardOverviewComponent;
  let fixture: ComponentFixture<WizardOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WizardOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WizardOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
