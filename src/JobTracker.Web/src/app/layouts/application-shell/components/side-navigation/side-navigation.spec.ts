import { provideRouter } from '@angular/router';
import { TestBed } from '@angular/core/testing';

import { SideNavigation } from './side-navigation';

describe('SideNavigation', () => {
  it('renders the available route and marks future features unavailable', async () => {
    await TestBed.configureTestingModule({
      imports: [SideNavigation],
      providers: [provideRouter([])],
    }).compileComponents();

    const fixture = TestBed.createComponent(SideNavigation);
    fixture.componentRef.setInput('isOpen', false);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('a[href="/workspace"]')?.textContent).toContain('Dashboard');
    expect(element.querySelectorAll('[aria-disabled="true"]').length).toBe(1);
    expect(element.textContent).not.toContain('Workspace');
  });
});
