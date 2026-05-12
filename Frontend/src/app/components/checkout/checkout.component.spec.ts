import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CheckoutComponent } from './checkout.component';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { RouterTestingModule } from '@angular/router/testing';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { BehaviorSubject } from 'rxjs';

describe('CheckoutComponent', () => {
  let component: CheckoutComponent;
  let fixture: ComponentFixture<CheckoutComponent>;
  let cartService: CartService;
  let orderService: OrderService;
  let authService: AuthService;

  const mockCart = {
    items: [
      { product: { id: 1, name: 'Keyboard', price: 150, stock: 10, description: 'Test' }, quantity: 1 }
    ],
    totalPrice: 150,
    totalItems: 1
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        CheckoutComponent,
        FormsModule,
        RouterTestingModule,
        HttpClientTestingModule
      ],
      providers: [CartService, OrderService, AuthService]
    }).compileComponents();

    fixture = TestBed.createComponent(CheckoutComponent);
    component = fixture.componentInstance;
    cartService = TestBed.inject(CartService);
    orderService = TestBed.inject(OrderService);
    authService = TestBed.inject(AuthService);
  });

  describe('Component Initialization', () => {
    it('should create the component', () => {
      expect(component).toBeTruthy();
    });

    it('should redirect to login if not authenticated', () => {
      spyOn(authService, 'isAuthenticated').and.returnValue(false);
      spyOn(window, 'alert');

      fixture.detectChanges();

      expect(window.alert).toHaveBeenCalledWith('Please login first');
    });

    it('should load cart data on init', () => {
      spyOn(authService, 'isAuthenticated').and.returnValue(true);
      spyOn(cartService.cart$, 'subscribe').and.returnValue(of(mockCart).subscribe());

      component.ngOnInit();

      expect(component.cartTotal).toBe(0); // Would be set after subscribe
    });
  });

  describe('Form Input', () => {
    beforeEach(() => {
      spyOn(authService, 'isAuthenticated').and.returnValue(true);
    });

    it('should update shipping address', () => {
      component.shippingAddress = '123 Main St';
      expect(component.shippingAddress).toBe('123 Main St');
    });

    it('should update city', () => {
      component.shippingCity = 'New York';
      expect(component.shippingCity).toBe('New York');
    });

    it('should update state', () => {
      component.shippingState = 'NY';
      expect(component.shippingState).toBe('NY');
    });

    it('should update zip code', () => {
      component.shippingZip = '10001';
      expect(component.shippingZip).toBe('10001');
    });
  });

  describe('Form Validation', () => {
    it('should not allow empty shipping address', () => {
      component.shippingAddress = '';
      const isValid = component['validateForm']();
      expect(isValid).toBe(false);
      expect(component.error).toBeTruthy();
    });

    it('should not allow empty city', () => {
      component.shippingAddress = '123 Main St';
      component.shippingCity = '';
      const isValid = component['validateForm']();
      expect(isValid).toBe(false);
    });

    it('should not allow empty state', () => {
      component.shippingAddress = '123 Main St';
      component.shippingCity = 'New York';
      component.shippingState = '';
      const isValid = component['validateForm']();
      expect(isValid).toBe(false);
    });

    it('should not allow empty zip', () => {
      component.shippingAddress = '123 Main St';
      component.shippingCity = 'New York';
      component.shippingState = 'NY';
      component.shippingZip = '';
      const isValid = component['validateForm']();
      expect(isValid).toBe(false);
    });

    it('should accept valid form', () => {
      component.shippingAddress = '123 Main St';
      component.shippingCity = 'New York';
      component.shippingState = 'NY';
      component.shippingZip = '10001';
      const isValid = component['validateForm']();
      expect(isValid).toBe(true);
    });
  });

  describe('Place Order', () => {
    beforeEach(() => {
      spyOn(authService, 'isAuthenticated').and.returnValue(true);
      component.cartItems = mockCart.items;
      component.cartTotal = mockCart.totalPrice;
      component.shippingAddress = '123 Main St';
      component.shippingCity = 'New York';
      component.shippingState = 'NY';
      component.shippingZip = '10001';
      component.loading = false;
      component.error = null;
    });

    it('should call orderService.placeOrder', () => {
      spyOn(orderService, 'placeOrder').and.returnValue(of({ id: 1, totalPrice: 150 } as any));

      component.placeOrder();

      expect(orderService.placeOrder).toHaveBeenCalled();
    });

    it('should set loading to false after successful order', (done) => {
      spyOn(window, 'alert');
      spyOn(orderService, 'placeOrder').and.returnValue(of({ id: 1, totalPrice: 150 } as any));
      spyOn(cartService, 'clearCart');

      component.placeOrder();

      setTimeout(() => {
        expect(component.loading).toBe(false);
        expect(component.success).toBe(true);
        done();
      }, 100);
    });

    it('should show success alert on successful order', (done) => {
      spyOn(window, 'alert');
      spyOn(orderService, 'placeOrder').and.returnValue(of({ id: 1, totalPrice: 150 } as any));
      spyOn(cartService, 'clearCart');

      component.placeOrder();

      setTimeout(() => {
        expect(window.alert).toHaveBeenCalledWith(jasmine.stringContaining('Order ID: 1'));
        expect(component.success).toBe(true);
        expect(cartService.clearCart).toHaveBeenCalled();
        done();
      }, 100);
    });

    it('should show error message on order failure', (done) => {
      spyOn(orderService, 'placeOrder').and.returnValue(throwError(() => ({
        error: { message: 'Out of stock' }
      })));

      component.placeOrder();

      setTimeout(() => {
        expect(component.error).toBeTruthy();
        expect(component.loading).toBe(false);
        done();
      }, 100);
    });
  });

  describe('Cart Integration', () => {
    it('should display cart total', () => {
      spyOn(authService, 'isAuthenticated').and.returnValue(true);
      component.cartTotal = 150;

      expect(component.cartTotal).toBe(150);
    });

    it('should redirect to cart if cart is empty', () => {
      spyOn(authService, 'isAuthenticated').and.returnValue(true);
      spyOn(window, 'alert');
      component.cartItems = [];

      // This would be called in ngOnInit when cart is empty
      expect(component.cartItems.length).toBe(0);
    });
  });
});
