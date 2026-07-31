import { TestBed } from '@angular/core/testing';
import { ApplicationsPage } from './applications-page';

describe('ApplicationsPage', () => {
  it('renders applications, filters, and CRUD entry points', async () => {
    await TestBed.configureTestingModule({ imports: [ApplicationsPage] }).compileComponents();
    const fixture = TestBed.createComponent(ApplicationsPage);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelectorAll('tbody tr')).toHaveLength(5);
    expect(element.querySelectorAll('.filters select')).toHaveLength(2);
    expect(element.textContent).toContain('6 applications');
    (element.querySelector('.primary') as HTMLButtonElement).click();
    fixture.detectChanges();
    expect(element.querySelector('[role="dialog"]')).not.toBeNull();
  });
});
