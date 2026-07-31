import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { AppFooter } from './components/app-footer/app-footer';
import { SideNavigation } from './components/side-navigation/side-navigation';
import { TopNavigation } from './components/top-navigation/top-navigation';

@Component({
  selector: 'app-application-shell',
  imports: [AppFooter, RouterOutlet, SideNavigation, TopNavigation],
  templateUrl: './application-shell.html',
  styleUrl: './application-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationShell {
  protected readonly isNavigationOpen = signal(false);

  protected toggleNavigation(): void {
    this.isNavigationOpen.update((isOpen) => !isOpen);
  }

  protected closeNavigation(): void {
    this.isNavigationOpen.set(false);
  }
}
