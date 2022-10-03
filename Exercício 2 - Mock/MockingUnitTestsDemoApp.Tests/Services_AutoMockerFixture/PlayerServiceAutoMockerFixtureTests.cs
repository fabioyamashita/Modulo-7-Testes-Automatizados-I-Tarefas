using FluentAssertions;
using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using Moq;
using Xunit;

namespace MockingUnitTestsDemoApp.Tests.Services_AutoMockerFixture
{
    [Collection("Tests Fixture")]
    public class PlayerServiceAutoMockerFixtureTests
    {
        private readonly PlayerService _subject;
        private readonly TestsFixture _testsFixture;

        public PlayerServiceAutoMockerFixtureTests(TestsFixture testsFixture)
        {
            _testsFixture = testsFixture;
            _subject = _testsFixture.CreatePlayerService();
        }

        #region GetByID TESTS 
        [Fact(DisplayName = "GetByID Method when ID is valid")]
        [Trait("Ex.2", "PlayerServiceAutoMockerFixture Tests")]
        public void GetByID_IdIsValid_ShouldReturnPlayer()
        {
            // Arrange
            Player player = _testsFixture.CreatePlayerUsingAutoFixture();

            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Setup(pr => pr.GetByID(player.ID))
                .Returns(player);

            // Act
            var actual = _subject.GetByID(player.ID);

            // Assert
            actual.Should()
                .BeEquivalentTo(player);
        }

        [Fact(DisplayName = "GetByID Method when ID is invalid")]
        [Trait("Ex.2", "PlayerServiceAutoMockerFixture Tests")]
        public void GetByID_IdIsNotValid_ShouldBeNull()
        {
            // Arrange
            Player player = _testsFixture.CreatePlayerUsingBogus();

            var invalidId = player.ID - 100;

            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Setup(pr => pr.GetByID(invalidId))
                .Returns<Player>(null);

            // Act
            var action = _subject.GetByID(invalidId);

            // Assert
            action.Should().BeNull();
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
            var teams = _testsFixture.CreateListOfTeamsUsingBogus(numberOfTeams);
            var players = _testsFixture.CreateListOfPlayersUsingBogus(numberOfPlayersPerTeam);

            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(lr => lr.IsValid(validLeagueID))
                .Returns(true);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Setup(tr => tr.GetForLeague(validLeagueID))
                .Returns(teams);

            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Setup(pr => pr.GetForTeam(It.IsAny<int>()))
                .Returns(players);

            // Act
            var actual = _subject.GetForLeague(validLeagueID);

            // Assert
            actual.Should().HaveCount(totalPlayers);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Verify(tr => tr.GetForLeague(validLeagueID), Times.Once);

            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Exactly(numberOfTeams));
        }

        [Fact(DisplayName = "GetForLeague Method with Invalid League ID")]
        [Trait("Ex.2", "PlayerServiceAutoMockerFixture Tests")]
        public void GetForLeague_LeagueIDIsNotValid_ShouldReturnAnEmptyPlayerList()
        {
            // Arrange
            var invalidLeagueID = 2;

            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(ml => ml.IsValid(invalidLeagueID))
                .Returns(false);

            // Act
            var actual = _subject.GetForLeague(invalidLeagueID);

            // Assert
            actual.Should().HaveCount(0);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Verify(tr => tr.GetForLeague(invalidLeagueID), Times.Never);

            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Never);
        }

        [Fact(DisplayName = "GetForLeague Method with Valid League ID But No Team Is Found")]
        [Trait("Ex.2", "PlayerServiceAutoMockerFixture Tests")]
        public void GetForLeague_LeagueIDIsValidButNoTeamIsFound_ShouldThrowANullReferenceException()
        {
            // Arrange
            var leagueID = 1;

            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(lr => lr.IsValid(leagueID))
                .Returns(true);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Setup(tr => tr.GetForLeague(leagueID))
                .Returns<List<Team>>(null);

            // Act
            Action act = () => _subject.GetForLeague(leagueID);

            // Assert
            act.Should().Throw<NullReferenceException>();

            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Never);
        }
        #endregion
    }
}
