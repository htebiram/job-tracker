import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { ApplicationsPage } from './applications-page';
import { ApplicationRepository } from '../data/application.repository';
import { MockApplicationRepository } from '../data/mock-application.repository';

describe('ApplicationsPage', () => {
  it('renders applications, filters, and CRUD entry points', async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicationsPage],
      providers: [provideRouter([])],
    }).compileComponents();
    const fixture = TestBed.createComponent(ApplicationsPage);
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;
    expect(fixture.debugElement.injector.get(ApplicationRepository)).toBe(
      TestBed.inject(MockApplicationRepository),
    );
    expect(element.querySelectorAll('tbody tr')).toHaveLength(5);
    expect(element.querySelectorAll('.filters select')).toHaveLength(2);
    expect(element.textContent).toContain('6 applications');
    (element.querySelector('.primary') as HTMLButtonElement).click();
    fixture.detectChanges();
    expect(element.querySelector('[role="dialog"]')).not.toBeNull();
  });

  it('opens the create dialog from a route intent', async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicationsPage],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { queryParamMap: convertToParamMap({ action: 'create' }) } },
        },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(ApplicationsPage);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('[role="dialog"]')).not.toBeNull();
  });
});
