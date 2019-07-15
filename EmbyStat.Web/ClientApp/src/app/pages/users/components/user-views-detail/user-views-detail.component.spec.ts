import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserViewsDetailComponent } from './user-views-detail.component';

describe('UserViewsDetailComponent', () => {
  let component: UserViewsDetailComponent;
  let fixture: ComponentFixture<UserViewsDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserViewsDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserViewsDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
