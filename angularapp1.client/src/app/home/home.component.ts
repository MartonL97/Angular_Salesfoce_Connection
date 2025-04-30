import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  @Input() accessToken!: string;
  @Input() instanceUrl!: string;

  constructor() {
    console.log(this.accessToken, this.instanceUrl); // Accessing values
  }

}
