import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Product } from '../../models/models';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private productService: ProductService,
    private cartService: CartService
  ) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.error = null;
    this.productService.getAllProducts().subscribe(
      (data: Product[]) => {
        this.products = data;
        this.loading = false;
      },
      (error) => {
        this.error = 'Failed to load products';
        this.loading = false;
        console.error('Error loading products:', error);
      }
    );
  }

  addToCart(product: Product, quantity: number = 1): void {
    if (quantity > 0) {
      this.cartService.addToCart(product, quantity);
      alert(`${product.name} added to cart!`);
    }
  }

  getQuantityForProduct(productId: number): number {
    const items = this.cartService.getCartItems();
    const item = items.find(i => i.product.id === productId);
    return item ? item.quantity : 0;
  }
}
