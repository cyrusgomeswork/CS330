namespace Fall2024_Assignment3_chgomes.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Imdb { get; set; }
        public byte[]? Photo { get; set; }

        public List<MovieActor> MovieActors { get; set; }
    }
}
