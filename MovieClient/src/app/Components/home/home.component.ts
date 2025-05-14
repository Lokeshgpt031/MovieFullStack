import { Component, HostListener, NgModule } from '@angular/core';
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
  sortBy: 'rating' | 'title' | 'releaseDate' = 'rating';
  sortByOptions: string[] = ['rating', 'title', 'releaseDate'];
  sortOrder: 'asc' | 'desc' = 'asc';

  constructor(moviesService: MovieService) {
    this.moviesService = moviesService;
    this.sortMovies();
  }


  sortMovies(arg0: "title" | "rating" | "releaseDate" = 'rating') {
    this.movies = [];
    this.sortBy = arg0;
    this.getMoviesList();
  }


  getMoviesList() {
    if (this.sortBy === 'rating') {
      this.moviesService.getMoviesByRating(this.sortOrder,this.page,this.pageSize).subscribe(
        (data: Movie[]) => {
          this.movies = [...this.movies,...data];
          console.log('Movies sorted by rating:', data);
        },
        (error: any) => {
          console.error('Error sorting movies by rating:', error);
        }
      );
    } else if (this.sortBy === 'title') {
      this.moviesService.getMovieByTitle(this.sortOrder,this.page,this.pageSize).subscribe(
        (data: Movie[]) => {
          this.movies = [...this.movies, ...data];
          console.log('Movies sorted by title:', data);
        },
        (error: any) => {
          console.error('Error sorting movies by title:', error);
        }
      );
    } else if (this.sortBy === 'releaseDate') {
      this.moviesService.getMoviesByReleaseDate(this.sortOrder,this.page,this.pageSize).subscribe(
        (data: Movie[]) => {
          this.movies = [...this.movies,...data];
          console.log('Movies sorted by release date:', data);
        },
        (error: any) => {
          console.error('Error sorting movies by release date:', error);
        }
      );
    }
  }

  loadMore() {
    this.page += 1;
    this.sortMovies();
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
