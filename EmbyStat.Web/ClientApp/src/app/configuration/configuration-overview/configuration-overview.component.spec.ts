import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurationOverviewComponent } from './configuration-overview.component';

describe('ConfigurationOverviewComponent', () => {
  let component: ConfigurationOverviewComponent;
  let fixture: ComponentFixture<ConfigurationOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigurationOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigurationOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
