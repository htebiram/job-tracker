import { TestBed } from '@angular/core/testing';

import { DashboardPage } from './dashboard-page';

describe('DashboardPage', () => {
  it('renders summary data and every dashboard section', async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardPage],
    }).compileComponents();

    const fixture = TestBed.createComponent(DashboardPage);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelectorAll('.summary-card').length).toBe(4);
    expect(element.querySelector('#status-title')).not.toBeNull();
    expect(element.querySelector('#interviews-title')).not.toBeNull();
    expect(element.querySelector('#applications-title')).not.toBeNull();
    expect(element.querySelector('#activity-title')).not.toBeNull();
    expect(element.textContent).toContain('48');
  });
});
