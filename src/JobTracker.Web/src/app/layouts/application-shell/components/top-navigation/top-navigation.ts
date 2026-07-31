import { ChangeDetectionStrategy, Component, inject, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';

import { ThemeService } from '@core/services/theme.service';

@Component({
  selector: 'app-top-navigation',
  imports: [RouterLink],
  templateUrl: './top-navigation.html',
  styleUrl: './top-navigation.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopNavigation {
  private readonly themeService = inject(ThemeService);

  readonly isNavigationOpen = input.required<boolean>();
  readonly navigationToggle = output<void>();
  protected readonly theme = this.themeService.theme;

  protected toggleTheme(): void {
    this.themeService.toggle();
  }
}
