import { DOCUMENT } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { AppFooter } from './components/app-footer/app-footer';
import { SideNavigation } from './components/side-navigation/side-navigation';
import { TopNavigation } from './components/top-navigation/top-navigation';
import { DemoNotice } from './components/demo-notice/demo-notice';
import { DialogShell } from '@shared/ui/dialog-shell/dialog-shell';

@Component({
  selector: 'app-application-shell',
  imports: [AppFooter, DemoNotice, DialogShell, RouterOutlet, SideNavigation, TopNavigation],
  templateUrl: './application-shell.html',
  styleUrl: './application-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationShell {
  private readonly document = inject(DOCUMENT);
  protected readonly isNavigationOpen = signal(false);
  protected readonly isResetConfirmationOpen = signal(false);

  protected toggleNavigation(): void {
    this.isNavigationOpen.update((isOpen) => !isOpen);
  }

  protected closeNavigation(): void {
    this.isNavigationOpen.set(false);
  }

  protected resetDemo(): void {
    this.document.defaultView?.location.reload();
  }
}
