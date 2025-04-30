import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  accessToken: string | null = null;
  instanceUrl: string | null = null;

  setAuthData(accessToken: string, instanceUrl: string): void {
    this.accessToken = accessToken;
    this.instanceUrl = instanceUrl;
  }

  clearAuthData(): void {
    this.accessToken = null;
    this.instanceUrl = null;
  }

  processAuthCallback(): void {
    const hash = window.location.hash;
    if (!hash) {
      console.error('No hash found in the URL');
      return;
    }

    const params = new URLSearchParams(hash.slice(1));
    const accessToken = params.get('access_token');
    const instanceUrl = params.get('instance_url') || '';

    if (!accessToken) {
      console.error('No access token found');
      return;
    }

    console.log('Access Token:', accessToken);
    console.log('Instance URL:', instanceUrl);

    // Save the authentication data BEFORE any redirects or window closes
    this.setAuthData(accessToken, instanceUrl);

    if (window.opener) {
      try {
        window.opener.location.href = 'https://localhost:56519/home';
      } catch (err) {
        console.error('Failed to redirect opener:', err);
      }
    }

    // Now it's safe to close
    window.close();
  }
}
