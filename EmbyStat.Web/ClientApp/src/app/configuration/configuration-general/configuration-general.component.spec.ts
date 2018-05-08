import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurationGeneralComponent } from './configuration-general.component';

describe('ConfigurationGeneralComponent', () => {
  let component: ConfigurationGeneralComponent;
  let fixture: ComponentFixture<ConfigurationGeneralComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigurationGeneralComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigurationGeneralComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
