import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { JobApplication } from '../../models/application.models';

@Component({
  selector: 'app-application-details',
  templateUrl: './application-details.html',
  styleUrl: './application-details.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationDetails {
  readonly application = input.required<JobApplication>();
  readonly closed = output<void>();
}
