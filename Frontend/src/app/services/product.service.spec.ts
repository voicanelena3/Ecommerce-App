import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProductService } from './product.service';
import { environment } from '../../environments/environment';
import { Product } from '../models/models';

describe('ProductService', () => {
  let service: ProductService;
  let httpMock: HttpTestingController;

  const mockProducts: Product[] = [
    { id: 1, name: 'Mechanical Keyboard', price: 149.99, stock: 10, description: 'RGB Keyboard' },
    { id: 2, name: 'Wireless Mouse', price: 29.99, stock: 20, description: 'USB Mouse' },
    { id: 3, name: 'USB Hub', price: 39.99, stock: 0, description: '4-port Hub' }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProductService]
    });

    service = TestBed.inject(ProductService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('getAllProducts', () => {
    it('should fetch all products', () => {
      service.getAllProducts().subscribe(products => {
        expect(products.length).toBe(3);
        expect(products[0].name).toBe('Mechanical Keyboard');
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/products`);
      expect(req.request.method).toBe('GET');
      req.flush(mockProducts);
    });

    it('should handle empty product list', () => {
      service.getAllProducts().subscribe(products => {
        expect(products.length).toBe(0);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/products`);
      req.flush([]);
    });
  });

  describe('getProductById', () => {
    it('should fetch product by ID', () => {
      service.getProductById(1).subscribe(product => {
        expect(product.id).toBe(1);
        expect(product.name).toBe('Mechanical Keyboard');
        expect(product.price).toBe(149.99);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/products/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockProducts[0]);
    });

    it('should handle product not found', () => {
      service.getProductById(999).subscribe(
        () => fail('should have failed'),
        error => {
          expect(error.status).toBe(404);
        }
      );

      const req = httpMock.expectOne(`${environment.apiUrl}/products/999`);
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
    });
  });

  describe('searchProducts', () => {
    it('should search products by keyword', () => {
      const searchResults = [mockProducts[0]];

      service.searchProducts('keyboard').subscribe(products => {
        expect(products.length).toBe(1);
        expect(products[0].name).toBe('Mechanical Keyboard');
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/products/search?q=keyboard`);
      expect(req.request.method).toBe('GET');
      req.flush(searchResults);
    });

    it('should return empty array for no matches', () => {
      service.searchProducts('nonexistent').subscribe(products => {
        expect(products.length).toBe(0);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/products/search?q=nonexistent`);
      req.flush([]);
    });

    it('should handle special characters in search', () => {
      service.searchProducts('USB & Port').subscribe(products => {
        expect(products.length).toBe(1);
      });

      // The service builds URL with string interpolation, so match the full URL
      const req = httpMock.expectOne(`${environment.apiUrl}/products/search?q=USB & Port`);
      expect(req.request.method).toBe('GET');
      req.flush([mockProducts[2]]);
    });
  });

  describe('Out of stock products', () => {
    it('should include out of stock products in results', () => {
      service.getAllProducts().subscribe(products => {
        const outOfStock = products.filter(p => p.stock === 0);
        expect(outOfStock.length).toBe(1);
        expect(outOfStock[0].id).toBe(3);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/products`);
      req.flush(mockProducts);
    });
  });
});
