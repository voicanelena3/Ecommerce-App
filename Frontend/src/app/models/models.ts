export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl?: string;
  stock: number;
}

export interface CartItem {
  product: Product;
  quantity: number;
}

export interface CartState {
  items: CartItem[];
  totalPrice: number;
  totalItems: number;
}

export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  createdAt: Date;
}

export interface AuthRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface OrderItemRequest {
  productId: number;
  quantity: number;
}

export interface CheckoutRequest {
  shippingAddress: string;
  shippingCity: string;
  shippingState: string;
  shippingZip: string;
  cartItems: OrderItemRequest[];
}

export interface Order {
  id: number;
  userId: number;
  orderDate: Date;
  shippingAddress: string;
  shippingCity: string;
  shippingState: string;
  shippingZip: string;
  totalPrice: number;
  status: string;
  items: OrderItem[];
}

export interface OrderItem {
  productId: number;
  quantity: number;
  price: number;
}
