using FluentAssertions;
using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using Moq;
using Xunit;

namespace MockingUnitTestsDemoApp.Tests.Services_AutoMockerFixture
{
    [Collection(nameof(PlayerServiceCollection))]
    public class PlayerServiceAutoMockerFixtureTests
    {
        private readonly PlayerService _subject;
        private readonly PlayerServiceTestsFixture _playerServiceTestsFixture;

        public PlayerServiceAutoMockerFixtureTests(PlayerServiceTestsFixture playerServiceTestsFixture)
        {
            _playerServiceTestsFixture = playerServiceTestsFixture;
            _subject = _playerServiceTestsFixture.CreatePlayerService();
        }

        #region GetByID TESTS 
        // USING AUTOFIXTURE TO CREATE A FAKE PLAYER OBJECT
        [Fact(DisplayName = "GetByID Method when ID is valid")]
        [Trait("Ex.2", "PlayerServiceAutoMockerFixture Tests")]
        public void GetByID_IdIsValid_ShouldReturnPlayer()
        {
            // Arrange
            Player player = _playerServiceTestsFixture.CreatePlayerUsingAutoFixture();

            _playerServiceTestsFixture.Mocker.GetMock<IPlayerRepository>()
                .Setup(pr => pr.GetByID(player.ID))
                .Returns(player);

            // Act
            var actual = _subject.GetByID(player.ID);

            // Assert
            actual.Should()
                .BeEquivalentTo(player);
        }

        // USING BOGUS TO CREATE A FAKE PLAYER OBJECT
        [Fact(DisplayName = "GetByID Method when ID is invalid")]
        [Trait("Ex.2", "PlayerServiceAutoMockerFixture Tests")]
        public void GetByID_IdIsNotValid_ShouldThrowAnException()
        {
            // Arrange
            Player player = _playerServiceTestsFixture.CreatePlayerUsingBogus();

            var invalidId = player.ID - 100;

            _playerServiceTestsFixture.Mocker.GetMock<IPlayerRepository>()
                .Setup(pr => pr.GetByID(invalidId))
                .Throws(new Exception("Id does not exists!"));

            // Act
            Action act = () => _subject.GetByID(invalidId);

            // Assert
            act.Should().Throw<Exception>()
                .WithMessage("Id does not exists!");
        }
        #endregion

        #region GetForLeague TESTS 
        [Fact(DisplayName = "GetForLeague Method is successful")]
        [Trait("Ex.2", "PlayerServiceAutoMockerFixture Tests")]
        public void GetForLeague_IsSuccessful_ShouldReturnListOfPlayer()
        {
            // Arrange
            var validLeagueID = 1;
            var numberOfTeams = 2;
            var numberOfPlayersPerTeam = 2;
            var totalPlayers = numberOfTeams * numberOfPlayersPerTeam;

            _playerServiceTestsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(lr => lr.IsValid(validLeagueID)).Returns(true);

            var teams = _playerServiceTestsFixture.CreateListOfTeamsUsingBogus(numberOfTeams);
            _playerServiceTestsFixture.Mocker.GetMock<ITeamRepository>()
                .Setup(tr => tr.GetForLeague(validLeagueID)).Returns(teams);

            var players = _playerServiceTestsFixture.CreateListOfPlayersUsingBogus(numberOfPlayersPerTeam);
            _playerServiceTestsFixture.Mocker.GetMock<IPlayerRepository>()
                .Setup(pr => pr.GetForTeam(It.IsAny<int>())).Returns(players);

            // Act
            var actual = _subject.GetForLeague(validLeagueID);

            // Assert
            actual.Should().NotBeNullOrEmpty();
            actual.Should().HaveCount(totalPlayers);

            _playerServiceTestsFixture.Mocker.GetMock<ITeamRepository>()
                .Verify(tr => tr.GetForLeague(validLeagueID), Times.Once);

            _playerServiceTestsFixture.Mocker.GetMock<IPlayerRepository>()
                .Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Exactly(numberOfTeams));
        }

        [Fact(DisplayName = "GetForLeague Method with Invalid League ID")]
        [Trait("Ex.2", "PlayerServiceAutoMockerFixture Tests")]
        public void GetForLeague_LeagueIDIsNotValid_ShouldReturnAnEmptyPlayerList()
        {
            // Arrange
            var invalidLeagueID = 2;
            _playerServiceTestsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(ml => ml.IsValid(invalidLeagueID)).Returns(false);

            // Act
            var actual = _subject.GetForLeague(invalidLeagueID);

            // Assert
            actual.Should().HaveCount(0);

            _playerServiceTestsFixture.Mocker.GetMock<ITeamRepository>()
                .Verify(tr => tr.GetForLeague(invalidLeagueID), Times.Never);

            _playerServiceTestsFixture.Mocker.GetMock<IPlayerRepository>()
                .Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Never);
        }
        #endregion
    }
}
