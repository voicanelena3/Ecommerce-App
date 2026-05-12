import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from './services/cart.service';
import { AuthService } from './services/auth.service';
import { Observable } from 'rxjs';
import { CartState } from './models/models';
import { User } from './models/models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'E-Commerce Store';
  cart$: Observable<CartState>;
  currentUser$: Observable<User | null>;

  constructor(
    private cartService: CartService,
    public authService: AuthService
  ) {
    this.cart$ = this.cartService.cart$;
    this.currentUser$ = this.authService.currentUser$;
  }

  logout(): void {
    this.authService.logout();
  }
}
