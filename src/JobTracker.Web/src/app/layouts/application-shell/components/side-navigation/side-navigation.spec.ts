import { provideRouter } from '@angular/router';
import { TestBed } from '@angular/core/testing';

import { SideNavigation } from './side-navigation';

describe('SideNavigation', () => {
  it('renders every available workspace route', async () => {
    await TestBed.configureTestingModule({
      imports: [SideNavigation],
      providers: [provideRouter([])],
    }).compileComponents();

    const fixture = TestBed.createComponent(SideNavigation);
    fixture.componentRef.setInput('isOpen', false);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('a[href="/workspace"]')?.textContent).toContain('Dashboard');
    expect(element.querySelector('a[href="/workspace/settings"]')?.textContent).toContain(
      'Settings',
    );
    expect(element.querySelectorAll('[aria-disabled="true"]')).toHaveLength(0);
    expect(element.textContent).not.toContain('Workspace');
  });
});
