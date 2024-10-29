using System.ClientModel;
using System.Text.Json.Nodes;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using VaderSharp2;
using System.Text.Json;

namespace Fall2024_Assignment3_chgomes.Services
{
    public class OpenAIService
    {
        private readonly string _apiKey;
        private const string ApiEndpoint = "https://spring2024-assignment1-cyrusg20240923145153resourcegroup.openai.azure.com/";
        private const string DeploymentName = "gpt-4o-mini";
        private ApiKeyCredential _apiCredential;

        public OpenAIService(IConfiguration configuration)
        {
            _apiKey = configuration["ApiSettings:ApiKey"] ?? throw new Exception("OpenAI API key not found in the configuration.");
            _apiCredential = new(_apiKey);
        }

        public async Task<List<(string Review, double Sentiment)>> GetMovieReviews(string movieName, int movieYear)
        {
            var reviewsWithSentiment = new List<(string Review, double Sentiment)>();
            ChatClient chatClient = new AzureOpenAIClient(new Uri(ApiEndpoint), _apiCredential).GetChatClient(DeploymentName);

            string[] personas = { "is harsh", "loves romance", "loves comedy", "loves thrillers", "loves fantasy", "is a sci-fi fan", "adores historical dramas", "enjoys indie films", "loves action-packed blockbusters", "appreciates artistic and experimental films" };
            var reviews = new List<string>();
            foreach (string persona in personas)
            {
                var messages = new ChatMessage[]
                {
                    new SystemChatMessage($"You are a film reviewer and film critic who {persona}."),
                    new UserChatMessage($"How would you rate the movie {movieName} released in {movieYear} out of 10 in less than 105 words?")
                };
                var chatCompletionOptions = new ChatCompletionOptions
                {
                    MaxOutputTokenCount = 200,
                };
                ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages, chatCompletionOptions);

                reviews.Add(result.Value.Content[0].Text);
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }

            var analyzer = new SentimentIntensityAnalyzer();
            foreach (var review in reviews)
            {
                var sentiment = analyzer.PolarityScores(review);
                reviewsWithSentiment.Add((Review: review, Sentiment: sentiment.Compound));
            }

            return reviewsWithSentiment;
        }

        public async Task<List<(string Username, string Tweet, double Sentiment)>> GenerateTweets(string actorName)
        {
            ChatClient client = new AzureOpenAIClient(new Uri(ApiEndpoint), _apiCredential).GetChatClient(DeploymentName);

            var messages = new ChatMessage[]
            {
                new SystemChatMessage("You represent the Twitter social media platform. Generate an answer with a valid JSON formatted array of objects containing the tweet and username. The response should start with [ and end with ]. No additional text."),
                new UserChatMessage($"Generate 20 tweets from a variety of users about the actor {actorName}.")
            };

            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);

            string tweetsJsonString = result.Value.Content.FirstOrDefault()?.Text ?? "[]";
            int startIndex = tweetsJsonString.IndexOf('[');
            int endIndex = tweetsJsonString.LastIndexOf(']');

            if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
            {
                throw new InvalidOperationException("The API response does not contain valid JSON.");
            }

            string cleanedJsonString = tweetsJsonString.Substring(startIndex, (endIndex - startIndex) + 1);
            Console.WriteLine("Cleaned JSON: " + cleanedJsonString);

            JsonArray json;
            try
            {
                json = JsonNode.Parse(cleanedJsonString)!.AsArray();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse the response as JSON: " + cleanedJsonString, ex);
            }

            var analyzer = new SentimentIntensityAnalyzer();
            var tweetsWithSentiment = new List<(string Username, string Tweet, double Sentiment)>();

            foreach (var tweetNode in json)
            {
                string username = tweetNode!["username"]?.ToString() ?? "Unknown";
                string tweet = tweetNode!["tweet"]?.ToString() ?? "";

                var sentiment = analyzer.PolarityScores(tweet);

                tweetsWithSentiment.Add((username, tweet, sentiment.Compound));
            }

            return tweetsWithSentiment;
        }
    }
}
