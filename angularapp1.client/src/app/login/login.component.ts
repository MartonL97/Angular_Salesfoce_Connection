import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData = {
    email: '',  // You may still need the email for the UI
    password: '', // Same with password
  };

  constructor(private http: HttpClient) { }

  onSubmit() {
    const payload = {
      SalesforceUserName: this.loginData.email,
      JwtKey: this.loginData.password,
    };

    this.http.post('api/Auth/login', payload)
      .subscribe(
        (response) => {
          console.log('Login success:', response);
          // Handle success (e.g., store token, navigate, etc.)
        },
        (error) => {
          console.log('Login failed:', error);
          // Handle error (e.g., show message to user)
        }
      );
  }
}
