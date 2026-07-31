import { TestBed } from '@angular/core/testing';
import { CalendarPage } from './calendar-page';
describe('CalendarPage', () => {
  it('renders month, events, and agenda view', async () => {
    await TestBed.configureTestingModule({ imports: [CalendarPage] }).compileComponents();
    const fixture = TestBed.createComponent(CalendarPage);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelectorAll('[role=gridcell]')).toHaveLength(42);
    expect(element.textContent).toContain('Northstar Labs');
    (element.querySelectorAll('.views button')[1] as HTMLButtonElement).click();
    fixture.detectChanges();
    expect(element.querySelectorAll('.agenda article')).toHaveLength(6);
  });
});
