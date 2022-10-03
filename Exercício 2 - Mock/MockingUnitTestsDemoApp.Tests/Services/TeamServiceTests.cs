using Bogus;
using Bogus.DataSets;
using FluentAssertions;
using MockingUnitTestsDemoApp.Impl.Enums;
using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MockingUnitTestsDemoApp.Tests.Services
{
    public class TeamServiceTests
    {
        private readonly TeamService _subject;
        private readonly Mock<ILeagueRepository> _mockLeagueRepository;
        private readonly Mock<ITeamRepository> _mockTeamRepository;

        public TeamServiceTests()
        {
            _mockLeagueRepository = new Mock<ILeagueRepository>();
            _mockTeamRepository = new Mock<ITeamRepository>();

            _subject = new TeamService(_mockTeamRepository.Object,
                _mockLeagueRepository.Object);
        }

        #region Search TESTS 
        [Fact(DisplayName = "Search Method is successful With Newer Than Search")]
        [Trait("Ex.2", "TeamService Tests")]
        public void Search_IsSuccessfulWithNewerThanSearch_ShouldReturnListOfTeam()
        {
            // Arrange
            var teamSearch = CreateTeamSearch(SearchDateDirection.NewerThan);
            var validLeagueID = teamSearch.LeagueID;
            teamSearch.FoundingDate = new DateTime(1950, 1, 1);

            var numberOfTeams = 2;
            var teams = CreateListOfTeams(numberOfTeams);

            _mockLeagueRepository.Setup(lr => lr.IsValid(validLeagueID)).Returns(true);

            _mockTeamRepository.Setup(tr => tr.GetForLeague(validLeagueID)).Returns(teams);

            // Act
            var actual = _subject.Search(teamSearch);

            // Assert
            actual.Should().HaveCount(numberOfTeams);
            _mockTeamRepository.Verify(tr => tr.GetForLeague(validLeagueID), Times.Once);
        }

        [Fact(DisplayName = "Search Method is successful With Older Than Search")]
        [Trait("Ex.2", "TeamService Tests")]
        public void Search_IsSuccessfulWithOlderThanSearch_ShouldReturnAnEmptyListOfTeam()
        {
            // Arrange
            var teamSearch = CreateTeamSearch(SearchDateDirection.OlderThan);
            var validLeagueID = teamSearch.LeagueID;
            teamSearch.FoundingDate = new DateTime(1950, 1, 1);

            var numberOfTeams = 2;
            var teams = CreateListOfTeams(numberOfTeams);

            _mockLeagueRepository.Setup(lr => lr.IsValid(validLeagueID)).Returns(true);

            _mockTeamRepository.Setup(tr => tr.GetForLeague(validLeagueID)).Returns(teams);

            // Act
            var actual = _subject.Search(teamSearch);

            // Assert
            actual.Should().HaveCount(0);
            _mockTeamRepository.Verify(tr => tr.GetForLeague(validLeagueID), Times.Once);
        }

        [Fact(DisplayName = "Search Method with Invalid League ID")]
        [Trait("Ex.2", "TeamService Tests")]
        public void Search_LeagueIDIsNotValid_ShouldReturnAnEmptyTeamList()
        {
            // Arrange
            var teamSearch = CreateTeamSearch(SearchDateDirection.NewerThan);
            var invalidLeagueID = teamSearch.LeagueID;
            _mockLeagueRepository.Setup(lr => lr.IsValid(invalidLeagueID)).Returns(false);

            // Act
            var actual = _subject.Search(teamSearch);

            // Assert
            actual.Should().HaveCount(0);
            _mockTeamRepository.Verify(tr => tr.GetForLeague(invalidLeagueID), Times.Never);
        }

        [Fact(DisplayName = "Search Method with Valid League ID But No Team Is Found")]
        [Trait("Ex.2", "TeamService Tests")]
        public void Search_LeagueIDIsValidButNoTeamIsFound_ShouldThrowArgumentNullException()
        {
            // Arrange
            var teamSearch = CreateTeamSearch(SearchDateDirection.NewerThan);
            var leagueID = teamSearch.LeagueID;
            _mockLeagueRepository.Setup(lr => lr.IsValid(leagueID)).Returns(true);
            _mockTeamRepository.Setup(tr => tr.GetForLeague(leagueID)).Returns<List<Team>>(null);

            // Act
            Action act = () => _subject.Search(teamSearch);

            // Assert
            act.Should().Throw<ArgumentNullException>();
            _mockTeamRepository.Verify(tr => tr.GetForLeague(leagueID), Times.Once);
        }
        #endregion

        #region Helper Methods
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
        private TeamSearch CreateTeamSearch(SearchDateDirection dateDirection)
        {
            TeamSearch teamSearch = new Faker<TeamSearch>("pt_BR")
                .CustomInstantiator(f => new TeamSearch
                {
                    LeagueID = f.Random.Int(1, 1000),
                    FoundingDate = f.Date.Between(new DateTime(1980, 1, 1), new DateTime(2000, 1, 1)),
                    Direction = dateDirection,
                    Results = CreateListOfTeams(f.Random.Int(1, 5))
                });

            return teamSearch;
        }
        #endregion
    }
}
