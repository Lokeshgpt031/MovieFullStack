import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Movie } from '../Models/movies.model';
import { catchError, throwError } from 'rxjs';

const API_URL = 'http://localhost:5075/';
@Injectable({ providedIn: 'root' })
export class MovieService {

  defaultImage = signal('');
  constructor(private httpClient: HttpClient) {}

  getAllMovies() {
    return this.httpClient.get<Movie[]>(API_URL + 'api/Movies').pipe(
      catchError((error) => {
        console.error('Error fetching movies:', error);
        return throwError(() => new Error('Failed to fetch movies'));
      })
    );
  }
  getMovieById(id: { timestamp?: number; creationTime?: string }) {
    return this.httpClient.get<Movie>(API_URL + `api/Movies/${id}`).pipe(
      catchError((error) => {
        console.error('Error fetching movie details:', error);
        return throwError(() => new Error('Failed to fetch movie details'));
      })
    );
  }
  getMoviesBySearch(searchTerm: string) {
    return this.httpClient
      .get<Movie[]>(API_URL + `api/movies/search?query=${searchTerm}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching movies:', error);
          return throwError(() => new Error('Failed to fetch movies'));
        })
      );
  }
  getMoviesByPagination(page: number, limit: number) {
    return this.httpClient
      .get<Movie[]>(API_URL + `api/Movies/page/${page}/size/${limit}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching movies:', error);
          return throwError(() => new Error('Failed to fetch movies'));
        })
      );
  }

  setDefaultImage(imageUrl: string) {
    this.defaultImage.set(imageUrl);
  }
  getDefaultImage() {
    return this.defaultImage;
  }

  getMovieByTitle(sortOrder: 'asc' | 'desc', page: number, limit: number) {
    return this.httpClient
      .get<Movie[]>(API_URL + `api/Movies/sort/title/${sortOrder}?page=${page}&pageSize=${limit}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching movies:', error);
          return throwError(() => new Error('Failed to fetch movies'));
        })
      );
  }
  getMoviesByReleaseDate(sortOrder: 'asc' | 'desc', page: number, limit: number) {
    return this.httpClient
      .get<Movie[]>(API_URL + `api/Movies/sort/released/${sortOrder}?page=${page}&pageSize=${limit}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching movies:', error);
          return throwError(() => new Error('Failed to fetch movies'));
        })
      );
  }
  getMoviesByRating(sortOrder: 'asc' | 'desc', page: number, limit: number) {
    return this.httpClient
      .get<Movie[]>(API_URL + `api/Movies/sort/rating/${sortOrder}?page=${page}&pageSize=${limit}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching movies:', error);
          return throwError(() => new Error('Failed to fetch movies'));
        })
      );
  }

  getMovieByNameYear(name: string, year: number) {
    return this.httpClient
      .get<Movie>(API_URL + `api/Movies/searchByNameYear?name=${name}&year=${year}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching movies:', error);
          return throwError(() => new Error('Failed to fetch movies'));
        })
      );
  }
}
