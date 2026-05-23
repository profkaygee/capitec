import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FraudDashboard } from './fraud-dashboard';

describe('FraudDashboard', () => {
  let component: FraudDashboard;
  let fixture: ComponentFixture<FraudDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FraudDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FraudDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
