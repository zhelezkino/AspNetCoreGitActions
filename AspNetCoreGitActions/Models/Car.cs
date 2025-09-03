namespace AspNetCoreGitActions.Models
{
    public class Car
    {
        public int Id { get; set; }
        public required string Make { get; set; }
        public required string Model { get; set; }
        public int Year { get; set; } = 2000;
        public required string LicensePlate { get; set; }
    }
}
