import { Component, OnInit } from '@angular/core';
import { AuthService } from '../service/auth.service';

@Component({
  selector: 'app-callback',
  standalone: false,
  templateUrl: './callback.component.html',
  styleUrl: './callback.component.css'
})
export class CallbackComponent implements OnInit {

  accessToken: string = '';
  instanceUrl: string = '';

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.authService.processAuthCallback();

    this.accessToken = this.authService.accessToken || '';
    this.accessToken = this.authService.instanceUrl || '';
  }
}
