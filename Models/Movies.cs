using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieApi.Models
{
    [BsonIgnoreExtraElements]

    public class Movies
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("plot")]
        public string? Plot { get; set; }
        [BsonElement("genres")]
        [BsonIgnoreIfNull]
        public List<string>? Genres { get; set; }
        [BsonElement("runtime")]
        public int Runtime { get; set; }
        [BsonElement("cast")]
        public List<string>? Cast { get; set; }
        [BsonElement("poster")]
        public string? Poster { get; set; }
        [BsonElement("title")]
        public string? Title { get; set; }
        [BsonElement("fullplot")]
        public string? Fullplot { get; set; }
        [BsonElement("languages")]
        public List<string>? Languages { get; set; }
        [BsonElement("released")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonIgnoreIfDefault]
        public DateTime Released { get; set; }
        [BsonElement("directors")]
        public List<string>? Directors { get; set; }
        [BsonElement("rated")]
        public string? Rated { get; set; }
        [BsonElement("awards")]
        [BsonIgnoreIfNull]
        public Awards? Awards { get; set; }
        [BsonElement("lastupdated")]
        public string? Lastupdated { get; set; }
        [BsonIgnoreIfDefault]
        [BsonElement("year")]

        public object? Year { get; set; }

        [BsonElement("imdb")]
        public Imdb? Imdb { get; set; }
        [BsonElement("countries")]
        public List<string>? Countries { get; set; }
        [BsonElement("type")]
        public string? Type { get; set; }
        [BsonElement("tomatoes")]
        [BsonIgnoreIfNull]
        public Tomatoes? Tomatoes { get; set; }

        [BsonElement("num_mflix_comments")]
        public int NumMflixComments { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class Awards
    {
        [BsonElement("wins")]
        public int Wins { get; set; }
        [BsonElement("nominations")]
        public int Nominations { get; set; }
        [BsonElement("text")]
        public string? Text { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class Imdb
    {
        [BsonElement("rating")]
        public object? Rating { get; set; } // Accepts both double? and string
        [BsonIgnoreIfNull]
        [BsonElement("votes")]
        public object? Votes { get; set; }
        [BsonElement("id")]
        public int Id { get; set; }
    }
    public class Tomatoes
    {
        [BsonElement("viewer")]
        [BsonIgnoreIfNull]
        public Viewer? Viewer { get; set; }
        [BsonElement("fresh")]
        public int Fresh { get; set; }
        [BsonElement("critic")]
        public Critic? Critic { get; set; }
        [BsonElement("consensus")]
        public string? Consensus { get; set; }
        [BsonElement("dvd")]
        public DateTime? Dvd { get; set; }
        [BsonElement("production")]
        public string? Production { get; set; }
        [BsonElement("website")]
        public string? Website { get; set; }
        [BsonElement("boxOffice")]
        public object? BoxOffice { get; set; }
        [BsonElement("rotten")]
        public int Rotten { get; set; }
        [BsonElement("lastUpdated")]
        public DateTime? LastUpdated { get; set; }
        [BsonExtraElements]
        public BsonDocument? AdditionalFields { get; set; }
    }
    public class Viewer
    {
        [BsonElement("rating")]
        public double Rating { get; set; }
        [BsonElement("numReviews")]
        public int NumReviews { get; set; }
        [BsonElement("meter")]
        [BsonIgnoreIfDefault]
        public int Meter { get; set; }
    }
    public class Critic
    {
        [BsonElement("rating")]
        public double Rating { get; set; }
        [BsonElement("numReviews")]
        public int NumReviews { get; set; }
        [BsonElement("meter")]
        public int Meter { get; set; }
    }
}