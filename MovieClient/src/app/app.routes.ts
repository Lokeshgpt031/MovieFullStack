import { Routes } from '@angular/router';
import { HomeComponent } from './Components/home/home.component';
import { MovieComponent } from './Components/movie/movie.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
  },
  {
    path: 'movie/:title/:year',
    component: MovieComponent,
  },
  {
    path: 'home',
    component: HomeComponent,
  },
  {
    path: '**',
    redirectTo: '',
  },
];
