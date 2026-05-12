import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RegisterRequest } from '../../models/models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  email = '';
  password = '';
  confirmPassword = '';
  firstName = '';
  lastName = '';
  loading = false;
  error: string | null = null;
  success = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/products']);
    }
  }

  register(): void {
    this.error = null;

    if (!this.validateForm()) {
      return;
    }

    this.loading = true;

    const request: RegisterRequest = {
      email: this.email,
      password: this.password,
      firstName: this.firstName,
      lastName: this.lastName
    };

    this.authService.register(request).subscribe(
      (response) => {
        this.loading = false;
        this.success = true;
        alert('Registration successful! Redirecting to products...');
        this.router.navigate(['/products']);
      },
      (error) => {
        this.loading = false;
        this.error = error.error?.message || 'Registration failed. Please try again.';
        console.error('Registration error:', error);
      }
    );
  }

  private validateForm(): boolean {
    if (!this.email.trim()) {
      this.error = 'Please enter an email';
      return false;
    }
    if (!this.email.includes('@')) {
      this.error = 'Please enter a valid email';
      return false;
    }
    if (!this.password.trim()) {
      this.error = 'Please enter a password';
      return false;
    }
    if (this.password.length < 6) {
      this.error = 'Password must be at least 6 characters';
      return false;
    }
    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return false;
    }
    if (!this.firstName.trim()) {
      this.error = 'Please enter your first name';
      return false;
    }
    if (!this.lastName.trim()) {
      this.error = 'Please enter your last name';
      return false;
    }
    return true;
  }
}
