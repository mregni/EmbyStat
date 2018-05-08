import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigurationEmbyComponent } from './configuration-emby.component';

describe('ConfigurationEmbyComponent', () => {
  let component: ConfigurationEmbyComponent;
  let fixture: ComponentFixture<ConfigurationEmbyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigurationEmbyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigurationEmbyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
