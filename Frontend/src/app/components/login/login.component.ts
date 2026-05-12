import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { AuthRequest } from '../../models/models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  email = '';
  password = '';
  loading = false;
  error: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/products']);
    }
  }

  login(): void {
    this.error = null;

    if (!this.validateForm()) {
      return;
    }

    this.loading = true;

    const request: AuthRequest = {
      email: this.email,
      password: this.password
    };

    this.authService.login(request).subscribe(
      (response) => {
        this.loading = false;
        alert('Login successful! Redirecting...');
        this.router.navigate(['/products']);
      },
      (error) => {
        this.loading = false;
        this.error = error.error?.message || 'Login failed. Please check your credentials.';
        console.error('Login error:', error);
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
    return true;
  }
}
