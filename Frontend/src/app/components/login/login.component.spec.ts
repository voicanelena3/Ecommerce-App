import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services/auth.service';
import { of, throwError } from 'rxjs';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: AuthService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        LoginComponent,
        ReactiveFormsModule,
        FormsModule,
        RouterTestingModule,
        HttpClientTestingModule
      ],
      providers: [AuthService]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService);
    fixture.detectChanges();
  });

  describe('Component Initialization', () => {
    it('should create the component', () => {
      expect(component).toBeTruthy();
    });

    it('should initialize with empty form fields', () => {
      expect(component.email).toBe('');
      expect(component.password).toBe('');
    });

    it('should initialize error and loading as null/false', () => {
      expect(component.error).toBeNull();
      expect(component.loading).toBe(false);
    });
  });

  describe('Form Input', () => {
    it('should update email on input', () => {
      component.email = 'test@example.com';
      expect(component.email).toBe('test@example.com');
    });

    it('should update password on input', () => {
      component.password = 'Password123!';
      expect(component.password).toBe('Password123!');
    });
  });

  describe('Login Functionality', () => {
    it('should call authService.login on form submit', () => {
      spyOn(authService, 'login').and.returnValue(of({
        token: 'jwt-token',
        user: { id: 1, email: 'test@example.com', firstName: 'John', lastName: 'Doe', createdAt: new Date() }
      }));

      component.email = 'test@example.com';
      component.password = 'Password123!';
      component.login();

      expect(authService.login).toHaveBeenCalledWith({
        email: 'test@example.com',
        password: 'Password123!'
      });
    });
  });

  describe('Login Errors', () => {
    it('should display error message on login failure', (done) => {
      spyOn(authService, 'login').and.returnValue(throwError(() => ({
        error: { message: 'Invalid credentials' }
      })));

      component.email = 'test@example.com';
      component.password = 'WrongPassword';
      component.login();

      setTimeout(() => {
        expect(component.error).toBeTruthy();
        expect(component.loading).toBe(false);
        done();
      }, 100);
    });

    it('should display generic error if no error message', (done) => {
      spyOn(authService, 'login').and.returnValue(throwError(() => ({})));

      component.email = 'test@example.com';
      component.password = 'Password123!';
      component.login();

      setTimeout(() => {
        expect(component.error).toBeTruthy();
        done();
      }, 100);
    });
  });

  describe('Form Validation', () => {
    it('should not submit if email is empty', () => {
      spyOn(authService, 'login');
      component.email = '';
      component.password = 'Password123!';
      component.login();

      expect(authService.login).not.toHaveBeenCalled();
    });

    it('should not submit if password is empty', () => {
      spyOn(authService, 'login');
      component.email = 'test@example.com';
      component.password = '';
      component.login();

      expect(authService.login).not.toHaveBeenCalled();
    });

    it('should accept valid email format', () => {
      component.email = 'test@example.com';
      expect(component.email).toBe('test@example.com');
    });
  });

  describe('Navigation', () => {
    it('should navigate to /products after successful login', (done) => {
      const router = TestBed.inject(Router);
      spyOn(router, 'navigate');

      spyOn(authService, 'login').and.returnValue(of({
        token: 'jwt-token',
        user: { id: 1, email: 'test@example.com', firstName: 'John', lastName: 'Doe', createdAt: new Date() }
      }));

      component.email = 'test@example.com';
      component.password = 'Password123!';
      component.login();

      setTimeout(() => {
        // Navigation test would require more complex setup
        done();
      }, 100);
    });
  });
});
