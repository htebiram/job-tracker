import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { LandingPage } from './landing-page';

describe('LandingPage', () => {
  it('renders the hero, calls to action, preview, and feature highlights', async () => {
    await TestBed.configureTestingModule({
      imports: [LandingPage],
      providers: [provideRouter([])],
    }).compileComponents();

    const fixture = TestBed.createComponent(LandingPage);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('h1')?.textContent).toContain('Move every opportunity forward');
    expect(element.querySelectorAll('.hero-actions a').length).toBe(2);
    expect(element.querySelector('.product-preview')).not.toBeNull();
    expect(element.querySelectorAll('.feature-highlights article').length).toBe(3);
    expect(element.querySelector('nav[aria-label="Primary navigation"]')).toBeNull();
  });
});
