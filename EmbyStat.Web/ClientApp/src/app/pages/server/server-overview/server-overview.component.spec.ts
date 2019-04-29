import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServerOverviewComponent } from './server-overview.component';

describe('ServerOverviewComponent', () => {
  let component: ServerOverviewComponent;
  let fixture: ComponentFixture<ServerOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServerOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServerOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
