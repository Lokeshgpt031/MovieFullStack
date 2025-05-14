import { Component, Input, input } from '@angular/core';
import { Movie } from '../../Models/movies.model';
import { MovieService } from '../../Services/movies.service';

@Component({
  selector: 'app-movie',
  imports: [],
  templateUrl: './movie.component.html',
  styleUrl: './movie.component.css',
})
export class MovieComponent {
  timestamp:number=0;
  creationTime:string='';
  @Input() set queryParam(param: string) {
    const parsedParam = JSON.parse(param);
    console.log('Parsed param:', parsedParam);
    
    if (parsedParam && parsedParam.timestamp) {
      this.timestamp = parsedParam.timestamp;
    }
    if (parsedParam && parsedParam.creationTime) {
      this.creationTime = parsedParam.creationTime;
    }
    
  }

  constructor(private movieService:MovieService) {
    movieService.getMovieById({creationTime:this.creationTime, timestamp: this.timestamp}).subscribe((data: Movie) => {
      this.movie = data;
    });
  }




  onImageError(event: Event) {
    const imgElement = event.target as HTMLImageElement;
    imgElement.src = '/sample.jpg';
    // Prevent infinite error loop if sample.jpg also fails to load
    imgElement.onerror = null;
  }
   movie!: Movie;
}
