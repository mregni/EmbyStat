import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PluginOverviewComponent } from './plugin-overview.component';

describe('PluginOverviewComponent', () => {
  let component: PluginOverviewComponent;
  let fixture: ComponentFixture<PluginOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PluginOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PluginOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
