namespace Fall2024_Assignment3_chgomes.Models
{
    public class ActorDetailsView
    {
        public Actor Actor { get; set; }

        public List<(string Username, string Tweet, double Sentiment)> TweetWithSentiment { get; set; }

        public double AverageSentiment { get; set; }

        public List<Movie> Movies { get; set; }
    }
}
