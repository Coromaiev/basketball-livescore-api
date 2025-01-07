namespace BasketBall_LiveScore.Mocks
{
    public abstract class MockLoader
    {
        protected string PathToMock { get; }

        protected MockLoader(string pathToMock)
        {
            PathToMock = pathToMock;
        }

        public abstract Task PopulateDatabase();
    }
}
