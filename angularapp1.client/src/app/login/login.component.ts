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

  constructor(private http: HttpClient, private router: Router, private authService: AuthService) { }

  onSubmit() {
    const payload = {
      SalesforceUserName: this.loginData.email,
      Password: this.loginData.password,
    };

    this.http.post<{ token: string }>('http://localhost:5282/api/Auth/login', payload)
      .subscribe(
        (response) => {
          console.log('Login success:', response);
          this.router.navigate(['/success']);

          const token = response.token;
          this.authService.saveToken(token);

          const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
          });
        },
        (error) => {
          console.error('Login failed:', error);
        }
      );
  }
}
