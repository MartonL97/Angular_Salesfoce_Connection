import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  forecasts: any[] = [];

  constructor(private http: HttpClient) { }

  ngOnInit() {

  }
}
