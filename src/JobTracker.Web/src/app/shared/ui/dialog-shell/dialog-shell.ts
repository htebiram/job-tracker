import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  afterNextRender,
  input,
  output,
  viewChild,
} from '@angular/core';

@Component({
  selector: 'app-dialog-shell',
  templateUrl: './dialog-shell.html',
  styleUrl: './dialog-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DialogShell {
  private readonly dialog = viewChild.required<ElementRef<HTMLElement>>('dialog');
  readonly role = input<'dialog' | 'alertdialog'>('dialog');
  readonly labelledBy = input.required<string>();
  readonly maxWidth = input<'compact' | 'regular'>('regular');
  readonly dismissed = output<void>();

  constructor() {
    afterNextRender(() => this.dialog().nativeElement.focus());
  }
}
