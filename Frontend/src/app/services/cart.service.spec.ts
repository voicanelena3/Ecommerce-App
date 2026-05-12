import { TestBed } from '@angular/core/testing';
import { CartService } from './cart.service';
import { CartState } from '../models/models';

describe('CartService', () => {
  let service: CartService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CartService]
    });

    service = TestBed.inject(CartService);
    localStorage.clear();
  });

  afterEach(() => {
    localStorage.clear();
  });

  describe('addToCart', () => {
    it('should add product to cart', (done) => {
      const product = { id: 1, name: 'Test Product', price: 50, stock: 10, description: 'Test' };

      service.addToCart(product);

      service.cart$.subscribe(cart => {
        expect(cart.items.length).toBe(1);
        expect(cart.items[0].product.id).toBe(1);
        expect(cart.items[0].quantity).toBe(1);
        done();
      });
    });

    it('should increment quantity if product already in cart', (done) => {
      const product = { id: 1, name: 'Test', price: 50, stock: 10, description: 'Test' };

      service.addToCart(product);
      service.addToCart(product);

      service.cart$.subscribe(cart => {
        expect(cart.items.length).toBe(1);
        expect(cart.items[0].quantity).toBe(2);
        done();
      });
    });

    it('should update total price', (done) => {
      const product = { id: 1, name: 'Test', price: 50, stock: 10, description: 'Test' };

      service.addToCart(product);

      service.cart$.subscribe(cart => {
        expect(cart.totalPrice).toBe(50);
        done();
      });
    });
  });

  describe('removeFromCart', () => {
    it('should remove product from cart', (done) => {
      const product = { id: 1, name: 'Test', price: 50, stock: 10, description: 'Test' };
      service.addToCart(product);
      service.removeFromCart(1);

      service.cart$.subscribe(cart => {
        expect(cart.items.length).toBe(0);
        expect(cart.totalPrice).toBe(0);
        done();
      });
    });
  });

  describe('updateQuantity', () => {
    it('should update quantity of product', (done) => {
      const product = { id: 1, name: 'Test', price: 50, stock: 10, description: 'Test' };
      service.addToCart(product);
      service.updateQuantity(1, 5);

      service.cart$.subscribe(cart => {
        expect(cart.items[0].quantity).toBe(5);
        expect(cart.totalPrice).toBe(250);
        done();
      });
    });

    it('should remove if quantity is 0', (done) => {
      const product = { id: 1, name: 'Test', price: 50, stock: 10, description: 'Test' };
      service.addToCart(product);
      service.updateQuantity(1, 0);

      service.cart$.subscribe(cart => {
        expect(cart.items.length).toBe(0);
        done();
      });
    });
  });

  describe('clearCart', () => {
    it('should clear all items from cart', () => {
      const product = { id: 1, name: 'Test', price: 50, stock: 10, description: 'Test' };
      service.addToCart(product);
      
      // Verify item was added and saved to localStorage
      localStorage.setItem('cart', JSON.stringify({ items: [{ product, quantity: 1 }], totalPrice: 50, totalItems: 1 }));
      expect(service.getCartItems().length).toBeGreaterThan(0);

      // Call clearCart
      service.clearCart();
      
      // Verify localStorage was cleared
      const savedCart = localStorage.getItem('cart');
      expect(savedCart).toBeNull();
    });
  });

  describe('Persistence', () => {
    it('should persist cart to localStorage', (done) => {
      const product = { id: 1, name: 'Test', price: 50, stock: 10, description: 'Test' };
      service.addToCart(product);

      setTimeout(() => {
        const savedCart = localStorage.getItem('cart');
        expect(savedCart).toBeTruthy();
        const parsed = JSON.parse(savedCart!);
        expect(parsed.items.length).toBe(1);
        done();
      }, 100);
    });

    it('should load cart from localStorage on init', () => {
      const mockCart = {
        items: [{ product: { id: 1, name: 'Test', price: 50, stock: 10, description: 'Test' }, quantity: 2 }],
        totalPrice: 100,
        totalItems: 2
      };
      localStorage.setItem('cart', JSON.stringify(mockCart));

      const newService = new CartService();
      newService.cart$.subscribe(cart => {
        expect(cart.items.length).toBe(1);
        expect(cart.totalPrice).toBe(100);
      });
    });
  });
});
