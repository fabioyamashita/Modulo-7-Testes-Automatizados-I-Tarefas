using AutoFixture;
using Bogus;
using Bogus.DataSets;
using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using Moq;
using Moq.AutoMock;
using Xunit;

// https://hamidmosalla.com/2020/02/02/xunit-part-5-share-test-context-with-iclassfixture-and-icollectionfixture/
// Using ICollectionFixture is useful because:
// 

namespace MockingUnitTestsDemoApp.Tests.Services_AutoMockerFixture
{
    [CollectionDefinition(nameof(PlayerServiceCollection))]
    public class PlayerServiceCollection : ICollectionFixture<PlayerServiceTestsFixture>
    { }

    public class PlayerServiceTestsFixture : IDisposable
    {
        public PlayerService PlayerService;
        public AutoMocker Mocker;

        public PlayerService CreatePlayerService()
        {
            Mocker = new AutoMocker();
            PlayerService = Mocker.CreateInstance<PlayerService>();

            return PlayerService;
        }

        #region AutoFixture Methods
        public Player CreatePlayerUsingAutoFixture()
        {
            return new Fixture().Create<Player>();
        }

        public List<Team> CreateListOfTeamsUsingAutoFixture(int numberOfTeams)
        {
            var teams = new List<Team>();

            for (int i = 0; i < numberOfTeams; i++)
            {
                Team team = new Fixture().Create<Team>();
                teams.Add(team);
            }

            return teams;
        }

        public List<Player> CreateListOfPlayersUsingAutoFixture(int numberOfPlayers)
        {
            var players = new List<Player>();

            for (int i = 0; i < numberOfPlayers; i++)
            {
                Player player = CreatePlayerUsingAutoFixture();
                players.Add(player);
            }

            return players;
        }
        #endregion

        #region Bogus Methods
        public Player CreatePlayerUsingBogus()
        {
            Player player = new Faker<Player>("pt_BR")
                .CustomInstantiator(f => new Player
                {
                    ID = f.Random.Int(1, 10),
                    FirstName = f.Name.FirstName(Name.Gender.Male),
                    LastName = f.Name.LastName(Name.Gender.Male),
                    DateOfBirth = f.Date.Between(new DateTime(1980, 1, 1), new DateTime(2000, 1, 1)),
                    TeamID = f.Random.Int(1, 5)

                });

            return player;
        }
        public List<Team> CreateListOfTeamsUsingBogus(int numberOfTeams)
        {
            var teams = new List<Team>();

            for (int i = 0; i < numberOfTeams; i++)
            {
                Team team = new Faker<Team>("pt_BR")
                    .CustomInstantiator(f => new Team
                    {
                        ID = f.Random.Int(1, numberOfTeams),
                        Name = f.Name.FirstName(Name.Gender.Male),
                        LeagueID = f.Random.Int(1, 5),
                        FoundingDate = f.Date.Between(new DateTime(1980, 1, 1), new DateTime(2000, 1, 1))
                    });
                teams.Add(team);
            }

            return teams;
        }

        public List<Player> CreateListOfPlayersUsingBogus(int numberOfPlayers)
        {
            var players = new List<Player>();

            for (int i = 0; i < numberOfPlayers; i++)
            {
                Player player = new Faker<Player>("pt_BR")
                    .CustomInstantiator(f => new Player
                    {
                        ID = f.Random.Int(1, numberOfPlayers),
                        FirstName = f.Name.FirstName(Name.Gender.Male),
                        LastName = f.Name.LastName(Name.Gender.Male),
                        DateOfBirth = f.Date.Between(new DateTime(1980, 1, 1), new DateTime(2000, 1, 1)),
                        TeamID = f.Random.Int(1, 5)
                    });
                players.Add(player);
            }

            return players;
        }
        #endregion

        public void Dispose()
        {
        }
    }
}
