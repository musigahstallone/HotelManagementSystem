import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {WeatherForecast} from '../weatherForecast';
import {NgForOf, NgIf} from '@angular/common';

@Component({
  selector: 'app-weather',
  templateUrl: './weather.component.html',
  imports: [
    NgIf,
    NgForOf
  ],
  styleUrl: './weather.component.css'
})
export class WeatherComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getForecasts();
  }

  getForecasts() {
    this.http.get<WeatherForecast[]>('/api/weatherforecast').subscribe(
      (result) => {
        this.forecasts = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }
}
