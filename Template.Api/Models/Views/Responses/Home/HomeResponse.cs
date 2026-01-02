namespace Template.Api.Models.Views.Responses.Home
{
    public class HomeResponse
    {
        public required string AppName { get; set; }
        public required string Version { get; set; }
        public required string Environment { get; set; }
        public required string IssuerIp { get; set; }
    }
}
