import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { CheckoutRequest, OrderItemRequest } from '../../models/models';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {
  shippingAddress = '';
  shippingCity = '';
  shippingState = '';
  shippingZip = '';
  loading = false;
  error: string | null = null;
  success = false;

  cartTotal = 0;
  cartItems: any[] = [];

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    // Check if user is authenticated
    if (!this.authService.isAuthenticated()) {
      alert('Please login first');
      this.router.navigate(['/login']);
      return;
    }

    // Get cart data
    this.cartService.cart$.subscribe(cart => {
      this.cartTotal = cart.totalPrice;
      this.cartItems = cart.items;

      // Redirect if cart is empty
      if (cart.items.length === 0) {
        alert('Your cart is empty');
        this.router.navigate(['/cart']);
      }
    });
  }

  placeOrder(): void {
    if (!this.validateForm()) {
      return;
    }

    this.loading = true;
    this.error = null;

    // Transform cartItems to match backend format (productId only, not full product object)
    const transformedItems: OrderItemRequest[] = this.cartItems.map(item => ({
      productId: item.product.id,
      quantity: item.quantity
    }));

    const request: CheckoutRequest = {
      shippingAddress: this.shippingAddress,
      shippingCity: this.shippingCity,
      shippingState: this.shippingState,
      shippingZip: this.shippingZip,
      cartItems: transformedItems
    };

    this.orderService.placeOrder(request).subscribe(
      (order) => {
        this.loading = false;
        this.success = true;
        this.cartService.clearCart();
        alert('Order placed successfully! Order ID: ' + order.id);
        this.router.navigate(['/products']);
      },
      (error) => {
        this.loading = false;
        this.error = error.error?.message || 'Failed to place order. Please try again.';
        console.error('Error placing order:', error);
      }
    );
  }

  private validateForm(): boolean {
    if (!this.shippingAddress.trim()) {
      this.error = 'Please enter a shipping address';
      return false;
    }
    if (!this.shippingCity.trim()) {
      this.error = 'Please enter a city';
      return false;
    }
    if (!this.shippingState.trim()) {
      this.error = 'Please enter a state';
      return false;
    }
    if (!this.shippingZip.trim()) {
      this.error = 'Please enter a zip code';
      return false;
    }
    return true;
  }
}
