import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import { AuthResponse, RegisterRequest } from '../models/models';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  describe('Register', () => {
    it('should register user and save token', () => {
      const request: RegisterRequest = {
        email: 'test@example.com',
        password: 'Password123!',
        firstName: 'John',
        lastName: 'Doe'
      };

      const mockResponse: AuthResponse = {
        token: 'jwt-token-xyz',
        user: { id: 1, email: 'test@example.com', firstName: 'John', lastName: 'Doe', createdAt: new Date() }
      };

      service.register(request).subscribe(response => {
        expect(response.token).toBe('jwt-token-xyz');
        expect(localStorage.getItem('token')).toBe('jwt-token-xyz');
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/auth/register`);
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);
    });
  });

  describe('Login', () => {
    it('should login user and save token', () => {
      const loginRequest = { email: 'test@example.com', password: 'Password123!' };
      const mockResponse: AuthResponse = {
        token: 'jwt-token-xyz',
        user: { id: 1, email: 'test@example.com', firstName: 'John', lastName: 'Doe', createdAt: new Date() }
      };

      service.login(loginRequest).subscribe(response => {
        expect(response.token).toBe('jwt-token-xyz');
        expect(localStorage.getItem('token')).toBe('jwt-token-xyz');
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);
    });
  });

  describe('Logout', () => {
    it('should clear token from localStorage', () => {
      localStorage.setItem('token', 'test-token');
      service.logout();
      expect(localStorage.getItem('token')).toBeNull();
    });
  });

  describe('isAuthenticated', () => {
    it('should return true if token exists', () => {
      localStorage.setItem('token', 'test-token');
      expect(service.isAuthenticated()).toBe(true);
    });

    it('should return false if no token', () => {
      localStorage.removeItem('token');
      expect(service.isAuthenticated()).toBe(false);
    });
  });

  describe('getToken', () => {
    it('should return token from localStorage', () => {
      localStorage.setItem('token', 'my-jwt-token');
      expect(service.getToken()).toBe('my-jwt-token');
    });

    it('should return null if no token', () => {
      localStorage.removeItem('token');
      expect(service.getToken()).toBeNull();
    });
  });
});
