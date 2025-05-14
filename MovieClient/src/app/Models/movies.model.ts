export interface Movie {
  id?: {
    timestamp?: number;
    creationTime?: string;
  };
  plot?: string;
  genres?: string[];
  runtime?: number;
  cast?: string[];
  poster?: string;
  title?: string;
  fullplot?: string;
  languages?: string[];
  released?: string;
  directors?: string[];
  rated?: string;
  awards?: {
    wins?: number;
    nominations?: number;
    text?: string;
  };
  lastupdated?: string;
  year?: number;
  imdb?: {
    rating?: number;
    votes?: number;
    id?: number;
  };
  countries?: string[];
  type?: string;
  tomatoes?: {
    viewer?: {
      rating?: number;
      numReviews?: number;
      meter?: number;
    };
    fresh?: number;
    critic?: {
      rating?: number;
      numReviews?: number;
      meter?: number;
    };
    consensus?: string;
    dvd?: string;
    production?: string;
    website?: string;
    boxOffice?: string;
    rotten?: number;
    lastUpdated?: Date;
  };
  numMflixComments?: number;

  [key: string]: number | string | string[] | undefined | object; // Dynamic field to accept any type
}
