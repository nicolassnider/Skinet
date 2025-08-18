import { Component, inject } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatBadge } from '@angular/material/badge';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BusyService } from '../../core/services/busy.service';
import { MatProgressBar } from '@angular/material/progress-bar';
import { CartService } from '../../core/services/cart.service';
import { AccountService } from '../../core/services/account.service';
import { MatDivider } from "@angular/material/divider";
import { MatMenu, MatMenuTrigger } from "@angular/material/menu";
import { IsAdmin } from '../../shared/directives/is-admin';

@Component({
  selector: 'app-header',
  imports: [MatIcon, MatBadge, RouterLink, RouterLinkActive, MatProgressBar, MatDivider, MatMenu, MatMenuTrigger,IsAdmin],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  busyService = inject(BusyService);
  cartService = inject(CartService);
  accountService = inject(AccountService);
  private router = inject(Router);

  logout(){
    this.accountService.logout().subscribe({
      next:()=>{
        this.accountService.currentUser.set(null);
        this.router.navigateByUrl('/'); 
      }
    });
  }
  
}
