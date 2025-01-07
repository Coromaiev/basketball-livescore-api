using BasketBall_LiveScore.Services;
using System.Text.Json;

namespace BasketBall_LiveScore.Mocks
{
    public class TeamsMockLoader : MockLoader
    {
        private readonly ITeamService TeamService;

        public TeamsMockLoader(ITeamService teamService) : base("Mocks\\MockData\\teams.json")
        {
            TeamService = teamService;
        }

        public override async Task PopulateDatabase()
        {
            var jsonData = await File.ReadAllTextAsync(PathToMock);
            var teamNames = JsonSerializer.Deserialize<List<string>>(jsonData);

            if (teamNames is null || teamNames.Count == 0)
            {
                Console.WriteLine("No data to add");
                return;
            }

            foreach (var teamName in teamNames)
            {
                await TeamService.Create(teamName);
            }
        }
    }
}
