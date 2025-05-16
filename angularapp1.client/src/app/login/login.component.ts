import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router'
import { AuthService } from '../services/auth.service'; // Adjust path as needed


@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData = {
    email: '',
    password: '',
  };

  loginFailed = false;
  errorMessage = '';
  loading = false;

  constructor(private http: HttpClient, private router: Router, private authService: AuthService) { }

  onSubmit() {
    this.loading = true;
    this.loginFailed = false;

    const payload = {
      SalesforceUserName: this.loginData.email,
      Password: this.loginData.password,
    };

    this.http.post<{ token: string }>('/api/Auth/login', payload)
      .subscribe(
        (response) => {
          console.log('Login success');
          this.loading = false;
          this.router.navigate(['/success']);

          const token = response.token;
          this.authService.saveToken(token);
        },
        (error) => {
          console.error('Login failed:', error);
          this.loading = false;
          this.loginFailed = true;
          this.errorMessage = 'Invalid credentials';
        }
      );
  }
}


