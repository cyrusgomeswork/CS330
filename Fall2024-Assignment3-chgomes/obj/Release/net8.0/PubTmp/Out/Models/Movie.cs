namespace Fall2024_Assignment3_chgomes.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public string Imdb { get; set; }
        public byte[]? Poster { get; set; }

        public ICollection<MovieActor>? MovieActors { get; set; }

    }
}
