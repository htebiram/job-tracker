import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

interface FeatureHighlight {
  readonly marker: string;
  readonly title: string;
  readonly description: string;
}

@Component({
  selector: 'app-landing-page',
  imports: [RouterLink],
  templateUrl: './landing-page.html',
  styleUrl: './landing-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LandingPage {
  protected readonly highlights: readonly FeatureHighlight[] = [
    {
      marker: '01',
      title: 'See every opportunity',
      description: 'Keep applications, contacts, and next steps together.',
    },
    {
      marker: '02',
      title: 'Stay ahead',
      description: 'Turn interviews and follow-ups into a clear daily plan.',
    },
    {
      marker: '03',
      title: 'Learn what works',
      description: 'Understand your pipeline and focus on meaningful progress.',
    },
  ];
}
