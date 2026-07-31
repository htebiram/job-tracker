import { DOCUMENT } from '@angular/common';
import { Injectable, inject, signal } from '@angular/core';

export type ColorTheme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private readonly currentTheme = signal<ColorTheme>('light');

  readonly theme = this.currentTheme.asReadonly();

  constructor() {
    this.applyTheme(this.getPreferredTheme());
  }

  toggle(): void {
    this.applyTheme(this.currentTheme() === 'light' ? 'dark' : 'light');
  }

  private getPreferredTheme(): ColorTheme {
    const view = this.document.defaultView;
    return view?.matchMedia?.('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }

  private applyTheme(theme: ColorTheme): void {
    this.currentTheme.set(theme);
    this.document.documentElement.dataset['theme'] = theme;
    this.document.documentElement.style.colorScheme = theme;
  }
}
