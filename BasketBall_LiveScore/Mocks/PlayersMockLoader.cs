
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Services;
using System.Text.Json;

namespace BasketBall_LiveScore.Mocks
{
    public class PlayersMockLoader : MockLoader
    {
        private readonly IPlayerService PlayerService;

        public PlayersMockLoader(IPlayerService playerService) : base("Mocks\\MockData\\players.json")
        {
            PlayerService = playerService;
        }

        public override async Task PopulateDatabase()
        {
            var jsonData = await File.ReadAllTextAsync(PathToMock);
            var mockData = JsonSerializer.Deserialize<List<PlayerCreateDto>>(jsonData);

            if (mockData is null || mockData.Count == 0)
            {
                Console.WriteLine("No data to add");
                return;
            }

            foreach (var item in mockData)
            {
                await PlayerService.Create(item);
            }
        }
    }
}
