import { Component, input, OnInit, output } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-empty-state',
  imports: [MatIcon, MatButton, RouterLink],
  templateUrl: './empty-state.component.html',
  styleUrl: './empty-state.component.scss'
})
export class EmptyStateComponent {
  
  message = input.required<string>();
  icon = input.required<string>();
  actionText = input.required<string>();
  action = output<void>()

  onAction(): void {
    this.action.emit();
  }

}
