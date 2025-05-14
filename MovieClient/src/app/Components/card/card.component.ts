import { Component, Input } from '@angular/core';
import { Movie } from '../../Models/movies.model';
import { CommonModule } from '@angular/common';
import { MovieComponent } from "../movie/movie.component";
import { routes } from '../../app.routes';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.css'],
  standalone: true,
  imports: [CommonModule, MovieComponent],
})
export class CardComponent {
  @Input() movie!: Movie;

  constructor(private router: Router) {}

  ngOnInit() {
    console.log('Movie details:', this.movie);
  }

  onImageError(event: Event) {
    const imgElement = event.target as HTMLImageElement;
    imgElement.src = '/sample.jpg';
    // Prevent infinite error loop if sample.jpg also fails to load
    imgElement.onerror = null;
  }

  renderMoviesComponent(movie: Movie) {
    console.log('Rendering movie component with movie:', movie);
    // Navigate to the movie details page with the movie title and year as parameters
    
    this.router.navigate(['/movie', movie.title, movie.year]);
  }
}
