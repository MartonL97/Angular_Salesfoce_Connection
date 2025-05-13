import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

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

  constructor(private http: HttpClient) { }

  onSubmit() {
    const payload = {
      SalesforceUserName: this.loginData.email,
      Password: this.loginData.password,
    };

    this.http.post<{ token: string }>('/api/Auth/login', payload)
      .subscribe(
        (response) => {
          console.log('Login success:', response);

          const token = response.token;
          localStorage.setItem('authToken', token);

          const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
          });

          this.http.get('/Salesforce/store', { headers })
            .subscribe(
              (storeResponse) => {
                console.log('Store response:', storeResponse);
              },
              (storeError) => {
                console.error('Error calling store API:', storeError);
              }
            );
        },
        (error) => {
          console.error('Login failed:', error);
        }
      );
  }
}
