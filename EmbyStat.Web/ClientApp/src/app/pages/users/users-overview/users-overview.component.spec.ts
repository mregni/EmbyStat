import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersOverviewComponent } from './users-overview.component';

describe('UsersOverviewComponent', () => {
  let component: UsersOverviewComponent;
  let fixture: ComponentFixture<UsersOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsersOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsersOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
