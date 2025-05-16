import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../services/auth.service'; 
import { PlayerStats } from '../../Model/layer-stats.model';



@Component({
  selector: 'app-success-page',
  standalone: false,
  templateUrl: './success-page.component.html',
  styleUrls: ['./success-page.component.css']
})
export class SuccessPageComponent implements OnInit {
  playerData: PlayerStats | undefined;
  loading = true;

  constructor(
    private router: Router,
    private http: HttpClient,
    private authService: AuthService
  ) { }

  ngOnInit() {
    const token = this.authService.getToken();

    if (token) {
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });

      this.http.get('/Salesforce/player', { headers }).subscribe(
        (response: any) => {
          this.playerData = response.records[0];
          this.loading = false;
          console.log('Player response successful:', this.playerData?.Name);
        },
        (error) => {
          this.loading = false;
          console.error('Error calling store API:', error);
        }
      );
    } else {
      console.warn('No token found. Redirecting to login.');
      this.router.navigate(['/login']);
    }
  }

  onLogout() {
    this.authService.removeToken();
    this.router.navigate(['/login']);
  }
}
