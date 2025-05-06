import { Component } from '@angular/core';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginData = {
    email: '',
    password: '',
  };

  onSubmit() {
    console.log('Login attempt:', this.loginData);
    // TODO: Add login logic here (e.g. API call)
  }
}
