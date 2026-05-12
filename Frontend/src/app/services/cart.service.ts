import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { CartItem, CartState, Product } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private initialCartState: CartState = {
    items: [],
    totalPrice: 0,
    totalItems: 0
  };

  private cartSubject = new BehaviorSubject<CartState>(this.initialCartState);
  public cart$: Observable<CartState> = this.cartSubject.asObservable();

  constructor() {
    this.loadCartFromLocalStorage();
  }

  addToCart(product: Product, quantity: number = 1): void {
    const currentCart = this.cartSubject.value;
    const existingItem = currentCart.items.find(item => item.product.id === product.id);

    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      currentCart.items.push({ product, quantity });
    }

    this.updateCartState();
    this.saveCartToLocalStorage();
  }

  removeFromCart(productId: number): void {
    const currentCart = this.cartSubject.value;
    currentCart.items = currentCart.items.filter(item => item.product.id !== productId);
    this.updateCartState();
    this.saveCartToLocalStorage();
  }

  updateQuantity(productId: number, quantity: number): void {
    const currentCart = this.cartSubject.value;
    const item = currentCart.items.find(item => item.product.id === productId);

    if (item) {
      if (quantity <= 0) {
        this.removeFromCart(productId);
      } else {
        item.quantity = quantity;
        this.updateCartState();
        this.saveCartToLocalStorage();
      }
    }
  }

  clearCart(): void {
    this.cartSubject.next(this.initialCartState);
    localStorage.removeItem('cart');
  }

  getCartItems(): CartItem[] {
    return this.cartSubject.value.items;
  }

  getCartTotal(): number {
    return this.cartSubject.value.totalPrice;
  }

  getTotalItems(): number {
    return this.cartSubject.value.totalItems;
  }

  private updateCartState(): void {
    const currentCart = this.cartSubject.value;
    const totalPrice = currentCart.items.reduce(
      (total, item) => total + (item.product.price * item.quantity),
      0
    );
    const totalItems = currentCart.items.reduce((total, item) => total + item.quantity, 0);

    this.cartSubject.next({
      ...currentCart,
      totalPrice: parseFloat(totalPrice.toFixed(2)),
      totalItems
    });
  }

  private saveCartToLocalStorage(): void {
    localStorage.setItem('cart', JSON.stringify(this.cartSubject.value));
  }

  private loadCartFromLocalStorage(): void {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      try {
        this.cartSubject.next(JSON.parse(savedCart));
      } catch (e) {
        console.error('Error loading cart from localStorage', e);
      }
    }
  }
}
