import { ChangeDetectionStrategy, Component, output } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-demo-notice',
  imports: [RouterLink],
  templateUrl: './demo-notice.html',
  styleUrl: './demo-notice.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DemoNotice {
  readonly resetRequested = output<void>();
}
