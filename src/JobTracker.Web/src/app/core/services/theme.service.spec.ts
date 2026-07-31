import { DOCUMENT } from '@angular/common';
import { TestBed } from '@angular/core/testing';

import { ThemeService } from './theme.service';

describe('ThemeService', () => {
  let document: Document;
  let service: ThemeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    document = TestBed.inject(DOCUMENT);
    service = TestBed.inject(ThemeService);
  });

  it('applies a supported initial theme to the document', () => {
    expect(['light', 'dark']).toContain(service.theme());
    expect(document.documentElement.dataset['theme']).toBe(service.theme());
  });

  it('toggles the theme and document color scheme', () => {
    const initialTheme = service.theme();

    service.toggle();

    const expectedTheme = initialTheme === 'light' ? 'dark' : 'light';
    expect(service.theme()).toBe(expectedTheme);
    expect(document.documentElement.dataset['theme']).toBe(expectedTheme);
    expect(document.documentElement.style.colorScheme).toBe(expectedTheme);
  });
});
