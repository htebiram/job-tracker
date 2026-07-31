import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  template: `
    @if (marker(); as marker) {
      <span class="marker" aria-hidden="true">{{ marker }}</span>
    }
    <h2>{{ title() }}</h2>
    <p>{{ description() }}</p>
    <ng-content />
  `,
  styles: `
    :host {
      display: block;
      padding: var(--space-16);
      text-align: center;
    }
    .marker {
      display: grid;
      place-items: center;
      inline-size: 3rem;
      block-size: 3rem;
      margin: 0 auto var(--space-4);
      border-radius: 50%;
      background: var(--color-success-soft);
      color: var(--color-success);
    }
    h2 {
      margin-block-start: 0;
      font-size: var(--font-size-lg);
    }
    p {
      margin: 0.5rem 0;
      color: var(--color-text-muted);
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmptyState {
  readonly title = input.required<string>();
  readonly description = input.required<string>();
  readonly marker = input<string>();
}
