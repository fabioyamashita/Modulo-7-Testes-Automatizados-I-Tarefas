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
            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Setup(pr => pr.GetByID(It.IsAny<int>()))
                .Returns<Player>(null);

            // Act
            var action = _subject.GetByID(It.IsAny<int>());

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
            var numberOfTeams = 2;
            var numberOfPlayersPerTeam = 2;
            var totalPlayers = numberOfTeams * numberOfPlayersPerTeam;

            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(lr => lr.IsValid(It.IsAny<int>()))
                .Returns(true);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Setup(tr => tr.GetForLeague(It.IsAny<int>()))
                .Returns(_testsFixture.CreateListOfTeamsUsingBogus(numberOfTeams));

            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Setup(pr => pr.GetForTeam(It.IsAny<int>()))
                .Returns(_testsFixture.CreateListOfPlayersUsingBogus(numberOfPlayersPerTeam));

            // Act
            var actual = _subject.GetForLeague(It.IsAny<int>());

            // Assert
            actual.Should().HaveCount(totalPlayers);

            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Verify(lr => lr.IsValid(It.IsAny<int>()), Times.Once);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Verify(tr => tr.GetForLeague(It.IsAny<int>()), Times.Once);

            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Exactly(numberOfTeams));
        }

        [Fact(DisplayName = "GetForLeague Method with Invalid League ID")]
        [Trait("Ex.2", "PlayerServiceAutoMockerFixture Tests")]
        public void GetForLeague_LeagueIDIsNotValid_ShouldReturnAnEmptyPlayerList()
        {
            // Arrange
            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(ml => ml.IsValid(It.IsAny<int>()))
                .Returns(false);

            // Act
            var actual = _subject.GetForLeague(It.IsAny<int>());

            // Assert
            actual.Should().HaveCount(0);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Verify(tr => tr.GetForLeague(It.IsAny<int>()), Times.Never);

            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Never);
        }

        [Fact(DisplayName = "GetForLeague Method with Valid League ID But No Team Is Found")]
        [Trait("Ex.2", "PlayerServiceAutoMockerFixture Tests")]
        public void GetForLeague_LeagueIDIsValidButNoTeamIsFound_ShouldThrowANullReferenceException()
        {
            // Arrange
            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(lr => lr.IsValid(It.IsAny<int>()))
                .Returns(true);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Setup(tr => tr.GetForLeague(It.IsAny<int>()))
                .Returns<List<Team>>(null);

            // Act
            Action act = () => _subject.GetForLeague(It.IsAny<int>());

            // Assert
            act.Should().Throw<NullReferenceException>();

            _testsFixture.Mocker.GetMock<IPlayerRepository>()
                .Verify(pr => pr.GetForTeam(It.IsAny<int>()), Times.Never);
        }
        #endregion
    }
}
