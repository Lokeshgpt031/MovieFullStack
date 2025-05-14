import { Component, Input, input } from '@angular/core';
import { Movie } from '../../Models/movies.model';
import { MovieService } from '../../Services/movies.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-movie',
  imports: [CommonModule],
  templateUrl: './movie.component.html',
  styleUrl: './movie.component.css',
})
export class MovieComponent {
  @Input() title: string = '';
  @Input() year: number = 0;

  @Input() defaultImage: string = '/sample.jpg';

  constructor(
    private movieService: MovieService,
    private route: ActivatedRoute
  ) {
    this.route.params.subscribe((params: any) => {
      this.title = params['title'];
      this.year = params['year'];
      console.log('Route parameters:', this.title, this.year);
    });

    this.movieService.getMovieByNameYear(this.title, this.year).subscribe(
      (data: Movie) => {
        this.movie = data;
        console.log('Movie details:', data);
      },
      (error: any) => {
        console.error('Error fetching movie details:', error);
      }
    );
  }

  onImageError(event: Event) {
    const imgElement = event.target as HTMLImageElement;
    imgElement.src = '/sample.jpg';
    // Prevent infinite error loop if sample.jpg also fails to load
    imgElement.onerror = null;
  }
  movie!: Movie;
}
