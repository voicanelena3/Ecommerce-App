import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { CartState, CartItem } from '../../models/models';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cart$: Observable<CartState>;

  constructor(public cartService: CartService) {
    this.cart$ = this.cartService.cart$;
  }

  ngOnInit(): void {
  }

  removeItem(productId: number): void {
    this.cartService.removeFromCart(productId);
  }

  updateQuantity(productId: number, quantity: number): void {
    const num = parseInt(quantity.toString(), 10);
    if (num > 0) {
      this.cartService.updateQuantity(productId, num);
    }
  }

  onQuantityChange(event: any, productId: number): void {
    const value = event.target.value;
    const quantity = parseInt(value, 10);
    if (quantity > 0) {
      this.cartService.updateQuantity(productId, quantity);
    } else if (quantity === 0) {
      this.removeItem(productId);
    }
  }

  clearCart(): void {
    if (confirm('Are you sure you want to clear your cart?')) {
      this.cartService.clearCart();
    }
  }
}
