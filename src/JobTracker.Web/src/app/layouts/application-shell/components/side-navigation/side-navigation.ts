import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { PRIMARY_NAVIGATION } from '../../../../core/constants/navigation.constants';

@Component({
  selector: 'app-side-navigation',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './side-navigation.html',
  styleUrl: './side-navigation.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SideNavigation {
  readonly isOpen = input.required<boolean>();
  readonly navigationSelected = output<void>();
  protected readonly primaryNavigation = PRIMARY_NAVIGATION;
}
