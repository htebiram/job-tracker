import { provideRouter } from '@angular/router';
import { TestBed } from '@angular/core/testing';

import { ApplicationShell } from './application-shell';

describe('ApplicationShell', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicationShell],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('renders the expected application landmarks', () => {
    const fixture = TestBed.createComponent(ApplicationShell);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('header')).not.toBeNull();
    expect(element.querySelector('nav[aria-label="Primary navigation"]')).not.toBeNull();
    expect(element.querySelector('main#main-content')).not.toBeNull();
    expect(element.querySelector('aside[aria-label="Workspace widgets"]')).toBeNull();
    expect(element.querySelector('aside[aria-label="Demo mode"]')?.textContent).toContain(
      'local mock data',
    );
    expect(
      element.querySelector('aside[aria-label="Demo mode"] a[href="/"]')?.textContent,
    ).toContain('Exit demo');
    expect(element.querySelector('footer')).not.toBeNull();
  });

  it('opens and closes mobile navigation using accessible controls', () => {
    const fixture = TestBed.createComponent(ApplicationShell);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const toggle = element.querySelector<HTMLButtonElement>('.menu-button');
    expect(toggle?.getAttribute('aria-expanded')).toBe('false');

    toggle?.click();
    fixture.detectChanges();

    expect(toggle?.getAttribute('aria-expanded')).toBe('true');
    const backdrop = element.querySelector<HTMLButtonElement>('.navigation-backdrop');
    expect(backdrop).not.toBeNull();

    backdrop?.click();
    fixture.detectChanges();

    expect(toggle?.getAttribute('aria-expanded')).toBe('false');
    expect(element.querySelector('.navigation-backdrop')).toBeNull();
  });

  it('provides a skip link targeting the main content', () => {
    const fixture = TestBed.createComponent(ApplicationShell);
    fixture.detectChanges();

    const skipLink = fixture.nativeElement.querySelector('.skip-link') as HTMLAnchorElement;
    expect(skipLink.getAttribute('href')).toBe('#main-content');
  });

  it('confirms before resetting demo data', () => {
    const fixture = TestBed.createComponent(ApplicationShell);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    element.querySelector<HTMLButtonElement>('app-demo-notice button')?.click();
    fixture.detectChanges();

    expect(element.querySelector('[role="alertdialog"]')?.textContent).toContain(
      'Reset demo data?',
    );

    element.querySelector<HTMLButtonElement>('.reset-actions button')?.click();
    fixture.detectChanges();

    expect(element.querySelector('[role="alertdialog"]')).toBeNull();
  });
});
