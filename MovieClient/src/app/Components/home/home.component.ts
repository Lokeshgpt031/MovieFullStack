import { Component, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Movie } from '../../Models/movies.model';
import { MovieService } from '../../Services/movies.service';
import { CardComponent } from '../card/card.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  standalone: true,
  imports: [CardComponent, CommonModule, FormsModule],
  providers: [MovieService, NgModule],
})
export class HomeComponent {
  page = 1;
  pageSize = 6;
  movies: Movie[] = [];
  searchResults: Movie[] = [];
  moviesService: MovieService;

  defaultImage: string = '/sample.jpg';
  searchTerm: any;

  constructor(moviesService: MovieService) {
    this.moviesService = moviesService;
    this.getData();
  }

  getData() {
    this.moviesService
      .getMoviesByPagination(this.page, this.pageSize)
      .subscribe(
        (data: Movie[]) => {
          console.log('Movies fetched successfully:', data);
          this.movies = [...this.movies, ...data];
        },
        (error: any) => {
          console.error('Error fetching movies:', error);
        }
      );
  }

  loadMore() {
    this.page += 1;
    this.getData();
  }
  searchMovies(searchTerm: string): void {
    this.moviesService.getMoviesBySearch(searchTerm).subscribe(
      (data: Movie[]) => {
        this.searchResults = data;
        console.log('Movies fetched successfully:', data);
      },
      (error: any) => {
        console.error('Error fetching movies:', error);
      }
    );
    // Add logic to handle the search functionality here
  }
}
