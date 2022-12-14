using FluentAssertions;
using MockingUnitTestsDemoApp.Impl.Enums;
using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using Moq;
using Xunit;

namespace MockingUnitTestsDemoApp.Tests.Services_AutoMockerFixture
{
    [Collection("Tests Fixture")]
    public class TeamServiceAutoMockerFixtureTests
    {
        private readonly TeamService _subject;
        private readonly TestsFixture _testsFixture;
        public TeamServiceAutoMockerFixtureTests(TestsFixture testsFixture)
        {
            _testsFixture = testsFixture;
            _subject = _testsFixture.CreateTeamService();
        }

        #region Search TESTS 
        [Fact(DisplayName = "Search Method is successful With Newer Than Search")]
        [Trait("Ex.2", "TeamServiceAutoMockerFixtureTests Tests")]
        public void Search_IsSuccessfulWithNewerThanSearch_ShouldReturnListOfTeam()
        {
            // Arrange
            var teamSearch = _testsFixture.CreateTeamSearchUsingBogus(SearchDateDirection.NewerThan);
            teamSearch.FoundingDate = new DateTime(1950, 1, 1);

            var numberOfTeams = 2;

            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(lr => lr.IsValid(teamSearch.LeagueID))
                .Returns(true);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Setup(tr => tr.GetForLeague(teamSearch.LeagueID))
                .Returns(_testsFixture.CreateListOfTeamsUsingBogus(numberOfTeams));

            // Act
            var actual = _subject.Search(teamSearch);

            // Assert
            actual.Should().HaveCount(numberOfTeams);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Verify(tr => tr.GetForLeague(teamSearch.LeagueID), Times.Once);
        }

        [Fact(DisplayName = "Search Method is successful With Older Than Search")]
        [Trait("Ex.2", "TeamServiceAutoMockerFixtureTests Tests")]
        public void Search_IsSuccessfulWithOlderThanSearch_ShouldReturnAnEmptyListOfTeam()
        {
            // Arrange
            var teamSearch = _testsFixture.CreateTeamSearchUsingBogus(SearchDateDirection.OlderThan);
            teamSearch.FoundingDate = new DateTime(1950, 1, 1);

            var numberOfTeams = 2;
 
            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(lr => lr.IsValid(teamSearch.LeagueID))
                .Returns(true);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Setup(tr => tr.GetForLeague(teamSearch.LeagueID))
                .Returns(_testsFixture.CreateListOfTeamsUsingBogus(numberOfTeams));

            // Act
            var actual = _subject.Search(teamSearch);

            // Assert
            actual.Should().HaveCount(0);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Verify(tr => tr.GetForLeague(teamSearch.LeagueID), Times.Once);
        }

        [Fact(DisplayName = "Search Method with Invalid League ID")]
        [Trait("Ex.2", "TeamServiceAutoMockerFixtureTests Tests")]
        public void Search_LeagueIDIsNotValid_ShouldReturnAnEmptyTeamList()
        {
            // Arrange
            var teamSearch = _testsFixture.CreateTeamSearchUsingBogus(SearchDateDirection.NewerThan);
   
            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(lr => lr.IsValid(teamSearch.LeagueID))
                .Returns(false);

            // Act
            var actual = _subject.Search(teamSearch);

            // Assert
            actual.Should().HaveCount(0);
            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Verify(tr => tr.GetForLeague(teamSearch.LeagueID), Times.Never);
        }

        [Fact(DisplayName = "Search Method with Valid League ID But No Team Is Found")]
        [Trait("Ex.2", "TeamServiceAutoMockerFixtureTests Tests")]
        public void Search_LeagueIDIsValidButNoTeamIsFound_ShouldThrowArgumentNullException()
        {
            // Arrange
            var teamSearch = _testsFixture.CreateTeamSearchUsingBogus(SearchDateDirection.NewerThan);

            _testsFixture.Mocker.GetMock<ILeagueRepository>()
                .Setup(lr => lr.IsValid(teamSearch.LeagueID))
                .Returns(true);

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Setup(tr => tr.GetForLeague(teamSearch.LeagueID))
                .Returns<List<Team>>(null);

            // Act
            Action act = () => _subject.Search(teamSearch);

            // Assert
            act.Should().Throw<ArgumentNullException>();

            _testsFixture.Mocker.GetMock<ITeamRepository>()
                .Verify(tr => tr.GetForLeague(teamSearch.LeagueID), Times.Once);
        }
        #endregion

    }
}
