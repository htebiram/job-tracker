import { ChangeDetectionStrategy, Component, input } from '@angular/core';

export type BadgeTone = 'primary' | 'info' | 'success' | 'danger' | 'neutral';

@Component({
  selector: 'app-badge',
  template: `<span [class]="tone()"><ng-content /></span>`,
  styles: `
    :host {
      display: inline-flex;
    }
    span {
      display: inline-flex;
      padding: 0.25rem 0.55rem;
      border-radius: 2rem;
      background: var(--color-primary-soft);
      color: var(--color-primary-strong);
      font-size: var(--font-size-xs);
      font-weight: 700;
    }
    .info {
      background: var(--color-info-soft);
      color: var(--color-info);
    }
    .success {
      background: var(--color-success-soft);
      color: var(--color-success);
    }
    .danger {
      background: var(--color-danger-soft);
      color: var(--color-danger);
    }
    .neutral {
      background: var(--color-background);
      color: var(--color-text-muted);
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Badge {
  readonly tone = input<BadgeTone>('primary');
}
