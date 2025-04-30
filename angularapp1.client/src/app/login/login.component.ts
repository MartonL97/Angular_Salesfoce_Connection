import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {

  constructor(private http: HttpClient) { }

  ngOnInit(): void {

    this.getStores();

    this.getToken();
  }

  loginWithSalesforce(): void {
    const clientId = '3MVG9dAEux2v1sLsnNSYuh_c2dIUqNq8wcOdjOmg8DDRwwgaTzn6FM1tTVReo77uAehnAQN4J6uIZkzl6T5eq';
    const redirectUri = 'http://localhost:4200/callback';
    const authUrl = `https://login.salesforce.com/services/oauth2/authorize?response_type=token&client_id=${clientId}&redirect_uri=${redirectUri}`;

    // Open a new window for Salesforce login
    const loginWindow = window.open(authUrl, 'Salesforce Login', 'width=600,height=600');

    if (!loginWindow) {
      console.error('Popup blocked or failed to open');
      return;
    }

    // Optional: You can monitor if the user closes the window without logging in
    const timer = setInterval(() => {
      if (loginWindow.closed) {
        clearInterval(timer);
        console.log('Login window closed by user.');
        // You can show an error or retry message here if needed
      }
    }, 500);
  }

  getToken() {

    this.http.get<string>('/user').subscribe(
      (result) => {
        console.log(result);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  getStores() {

    this.http.get<string>('/salesforce/store').subscribe(
      (result) => {
        console.log(result);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  }


