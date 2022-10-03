using AutoFixture;
using Bogus;
using Bogus.DataSets;
using FluentAssertions;
using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using Moq;
using Xunit;

namespace MockingUnitTestsDemoApp.Tests.Services
{
    public class PlayerServiceTests
    {
        private readonly PlayerService _subject;
        private readonly Mock<ILeagueRepository> _mockLeagueRepository;
        private readonly Mock<IPlayerRepository> _mockPlayerRepository;
        private readonly Mock<ITeamRepository> _mockTeamRepository;

        public PlayerServiceTests()
        {
            _mockLeagueRepository = new Mock<ILeagueRepository>();
            _mockPlayerRepository = new Mock<IPlayerRepository>();
            _mockTeamRepository = new Mock<ITeamRepository>();

            _subject = new PlayerService(_mockPlayerRepository.Object,
                _mockTeamRepository.Object, _mockLeagueRepository.Object);
        }

        #region GetByID TESTS 
        // USING AUTOFIXTURE TO CREATE A FAKE PLAYER OBJECT
        [Fact(DisplayName = "GetByID Method when ID is valid")]
        [Trait("Ex.2", "PlayerService Tests")]
        public void GetByID_IdIsValid_ShouldReturnPlayer()
        {
            // Arrange
            Player player = CreatePlayerUsingAutoFixture();

            _mockPlayerRepository.Setup(pr => pr.GetByID(player.ID))
                .Returns(player);

            // Act
            var actual = _subject.GetByID(player.ID);

            // Assert
            actual.Should().BeEquivalentTo(player);
        }

        // USING BOGUS TO CREATE A FAKE PLAYER OBJECT
        [Fact(DisplayName = "GetByID Method when ID is invalid")]
        [Trait("Ex.2", "PlayerService Tests")]
        public void GetByID_IdIsNotValid_ShouldBeNull()
        {
            // Arrange
            Player player = CreatePlayerUsingBogus();

            var invalidId = player.ID - 100;

            _mockPlayerRepository.Setup(pr => pr.GetByID(invalidId))
                .Returns<Player>(null);

            // Act
            var actual = _subject.GetByID(invalidId);

            // Assert
            actual.Should().BeNull();
        }
        #endregion

        #region GetForLeague TESTS 
        [Fact(DisplayName = "GetForLeague Method is successful")]
        [Trait("Ex.2", "PlayerService Tests")]
        public void GetForLeague_IsSuccessful_ShouldReturnListOfPlayer()
        {
            // Arrange
            var validLeagueID = 1;
            var numberOfTeams = 2;
            var numberOfPlayersPerTeam = 2;
            var totalPlayers = numberOfTeams * numberOfPlayersPerTeam;
            var teams = CreateListOfTeams(numberOfTeams);
            var players = CreateListOfPlayers(numberOfPlayersPerTeam);

            _mockLeagueRepository.Setup(lr => lr.IsValid(validLeagueID)).Returns(true);

            _mockTeamRepository.Setup(tr => tr.GetForLeague(validLeagueID)).Returns(teams);

            _mockPlayerRepository.Setup(pr => pr.GetForTeam(It.IsAny<int>())).Returns(players);

            // Act
            var actual = _subject.GetForLeague(validLeagueID);

            // Assert
            actual.Should().HaveCount(totalPlayers);
            _mockTeamRepository.Verify(tr => tr.GetForLeague(validLeagueID), Times.Once);
            _mockPlayerRepository.Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Exactly(numberOfTeams));
        }

        [Fact(DisplayName = "GetForLeague Method with Invalid League ID")]
        [Trait("Ex.2", "PlayerService Tests")]
        public void GetForLeague_LeagueIDIsNotValid_ShouldReturnAnEmptyPlayerList()
        {
            // Arrange
            var invalidLeagueID = 2;
            _mockLeagueRepository.Setup(lr => lr.IsValid(invalidLeagueID)).Returns(false);

            // Act
            var actual = _subject.GetForLeague(invalidLeagueID);

            // Assert
            actual.Should().HaveCount(0);
            _mockTeamRepository.Verify(tr => tr.GetForLeague(invalidLeagueID), Times.Never);
            _mockPlayerRepository.Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Never);
        }

        [Fact(DisplayName = "GetForLeague Method with Valid League ID But No Team Is Found")]
        [Trait("Ex.2", "PlayerService Tests")]
        public void GetForLeague_LeagueIDIsValidButNoTeamIsFound_ShouldThrowANullReferenceException()
        {
            // Arrange
            var leagueID = 1;
            _mockLeagueRepository.Setup(lr => lr.IsValid(leagueID)).Returns(true);
            _mockTeamRepository.Setup(tr => tr.GetForLeague(leagueID)).Returns<List<Team>>(null);

            // Act
            Action act = () => _subject.GetForLeague(leagueID);

            // Assert
            act.Should().Throw<NullReferenceException>();
            _mockPlayerRepository.Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Never);
        }
        #endregion

        #region Helpers Methods
        private Player CreatePlayerUsingAutoFixture()
        {
            return new Fixture().Create<Player>();
        }

        private Player CreatePlayerUsingBogus()
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

        private List<Team> CreateListOfTeams(int numberOfTeams)
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

        private List<Player> CreateListOfPlayers(int numberOfPlayers)
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
    }
}
