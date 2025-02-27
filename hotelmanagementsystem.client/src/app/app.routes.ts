import {Routes} from '@angular/router';
import {TodosComponent} from './todos/todos.component';
import {WeatherComponent} from './weather/weather.component';

export const routes: Routes = [
  {path: "", component: TodosComponent},
  {path: "todos", component: TodosComponent},
  {path: "weather", component: WeatherComponent}
];
