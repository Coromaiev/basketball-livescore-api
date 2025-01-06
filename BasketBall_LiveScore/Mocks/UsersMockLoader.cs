using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Services;
using System.Text.Json;

namespace BasketBall_LiveScore.Mocks
{
    public class UsersMockLoader : MockLoader
    {
        private readonly IUserService UserService;

        public UsersMockLoader(IUserService userService) : base("Mocks\\MockData\\users.json")
        {
            UserService = userService;
        }

        public override async Task PopulateDatabase()
        {
            var jsonData = await File.ReadAllTextAsync(PathToMock);
            var mockData = JsonSerializer.Deserialize<List<UserCreateDto>>(jsonData);

            if (mockData is null || mockData.Count == 0)
            {
                Console.WriteLine("No data to add");
                return;
            }

            foreach (var item in mockData)
            {
                await UserService.Create(item);
            }
        }
    }
}
