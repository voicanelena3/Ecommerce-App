import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HttpClient } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';
import { AuthService } from './auth.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

describe('AuthInterceptor', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;
  let authService: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
      ]
    });

    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);
    authService = TestBed.inject(AuthService);
  });

  afterEach(() => {
    httpTestingController.verify();
    localStorage.clear();
  });

  describe('JWT Token Injection', () => {
    it('should add Authorization header with Bearer token', () => {
      const token = 'test-jwt-token-xyz';
      localStorage.setItem('token', token);

      httpClient.get('/api/orders').subscribe();

      const req = httpTestingController.expectOne('/api/orders');
      expect(req.request.headers.has('Authorization')).toBe(true);
      expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);

      req.flush({});
    });

    it('should not add Authorization header if no token', () => {
      localStorage.removeItem('token');

      httpClient.get('/api/products').subscribe();

      const req = httpTestingController.expectOne('/api/products');
      expect(req.request.headers.has('Authorization')).toBe(false);

      req.flush({});
    });

    it('should add token to POST requests', () => {
      const token = 'test-jwt-token';
      localStorage.setItem('token', token);

      httpClient.post('/api/orders', { data: 'test' }).subscribe();

      const req = httpTestingController.expectOne('/api/orders');
      expect(req.request.method).toBe('POST');
      expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);

      req.flush({});
    });

    it('should add token to PUT requests', () => {
      const token = 'test-jwt-token';
      localStorage.setItem('token', token);

      httpClient.put('/api/orders/1', { data: 'test' }).subscribe();

      const req = httpTestingController.expectOne('/api/orders/1');
      expect(req.request.method).toBe('PUT');
      expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);

      req.flush({});
    });

    it('should add token to DELETE requests', () => {
      const token = 'test-jwt-token';
      localStorage.setItem('token', token);

      httpClient.delete('/api/orders/1').subscribe();

      const req = httpTestingController.expectOne('/api/orders/1');
      expect(req.request.method).toBe('DELETE');
      expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);

      req.flush({});
    });
  });

  describe('Token Format', () => {
    it('should use Bearer scheme for JWT', () => {
      const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9';
      localStorage.setItem('token', token);

      httpClient.get('/api/test').subscribe();

      const req = httpTestingController.expectOne('/api/test');
      const authHeader = req.request.headers.get('Authorization');
      expect(authHeader).toMatch(/^Bearer /);

      req.flush({});
    });
  });

  describe('Multiple Requests', () => {
    it('should add token to all requests', () => {
      const token = 'test-jwt-token';
      localStorage.setItem('token', token);

      httpClient.get('/api/products').subscribe();
      httpClient.get('/api/orders').subscribe();
      httpClient.get('/api/cart').subscribe();

      const requests = httpTestingController.match(req => true);
      expect(requests.length).toBe(3);

      requests.forEach(req => {
        expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
        req.flush({});
      });
    });
  });

  describe('Error Handling', () => {
    it('should pass through 401 errors', () => {
      const token = 'invalid-token';
      localStorage.setItem('token', token);

      httpClient.get('/api/protected').subscribe(
        () => fail('should have failed'),
        error => {
          expect(error.status).toBe(401);
        }
      );

      const req = httpTestingController.expectOne('/api/protected');
      expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
      req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });
    });

    it('should pass through 403 errors', () => {
      const token = 'valid-token';
      localStorage.setItem('token', token);

      httpClient.get('/api/admin').subscribe(
        () => fail('should have failed'),
        error => {
          expect(error.status).toBe(403);
        }
      );

      const req = httpTestingController.expectOne('/api/admin');
      req.flush('Forbidden', { status: 403, statusText: 'Forbidden' });
    });
  });
});
