import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { PublicLayout } from './public-layout';

describe('PublicLayout', () => {
  it('renders public actions without application navigation', async () => {
    await TestBed.configureTestingModule({
      imports: [PublicLayout],
      providers: [provideRouter([])],
    }).compileComponents();

    const fixture = TestBed.createComponent(PublicLayout);
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('nav[aria-label="Public navigation"]')).not.toBeNull();
    expect(element.querySelectorAll('a[href="/workspace"]').length).toBe(2);
    expect(element.querySelector('nav[aria-label="Primary navigation"]')).toBeNull();
    expect(element.querySelector('aside')).toBeNull();
  });
});
