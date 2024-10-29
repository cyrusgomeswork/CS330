namespace Fall2024_Assignment3_chgomes.Models
{
    public class MovieDetailsView
    {
        public Movie Movie { get; set; }

        public List<(string Review, double Sentiment)> ReviewsWithSentiment { get; set; }

        public double AverageSentiment { get; set; }

        public List<Actor> Actors { get; set; }
    }
}
