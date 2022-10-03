using AutoFixture;
using FluentAssertions;
using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MockingUnitTestsDemoApp.Tests.Services
{
    public class PlayerServiceTests {

        private readonly PlayerService _context;
        private readonly Mock<ILeagueRepository> _mockedLeagueRepository;
        private readonly Mock<IPlayerRepository> _mockedPlayerRepository;
        private readonly Mock<ITeamRepository> _mockedTeamRepository;

        public PlayerServiceTests()
        {
            _mockedLeagueRepository = new Mock<ILeagueRepository>();
            _mockedPlayerRepository = new Mock<IPlayerRepository>();
            _mockedTeamRepository = new Mock<ITeamRepository>();

            _context = new PlayerService(_mockedPlayerRepository.Object, _mockedTeamRepository.Object, _mockedLeagueRepository.Object);
        }

        private List<Player> ListPlayers()
        {
            return new List<Player>
            {
                //Cruzeiro
                new Player {ID = 1, FirstName = "Dirceu", LastName = "Lopes", DateOfBirth = new DateTime(1946, 09, 03), TeamID = 1},
                new Player {ID = 2, FirstName = "Eduardo", LastName = "Andrade", DateOfBirth = new DateTime(1947, 01, 25), TeamID = 1},
                new Player {ID = 3, FirstName = "Ronaldo", LastName = "Nazário", DateOfBirth = new DateTime(1976, 09, 18), TeamID = 1},
                new Player {ID = 4, FirstName = "Roberto", LastName = "Perfumo", DateOfBirth = new DateTime(1942, 10, 03), TeamID = 1},
                new Player {ID = 5, FirstName = "Fábio", LastName = "Maciel", DateOfBirth = new DateTime(1980, 09, 30), TeamID = 1}
            };
        }

        private List<Team> ListTeams()
        {
            return new List<Team>
            {
                new Team {ID = 1, Name = "Cruzeiro", LeagueID = 1, FoundingDate = new DateTime()}
            };
        }

        [Theory]
        [InlineData(1)]
        public void GetForLeague_Successful_ReturnPlayerList(int id)
        {
            //Arrange
            _mockedLeagueRepository.Setup(mock => mock.IsValid(id)).Returns(true);
            _mockedPlayerRepository.Setup(mock => mock.GetForTeam(id)).Returns(ListPlayers());
            _mockedTeamRepository.Setup(mock => mock.GetForLeague(id)).Returns(ListTeams());

            //Act
            var players = _context.GetForLeague(id);

            //Assert
            players.Should().AllBeAssignableTo<Player>().And.NotBeNull();
        }

        [Theory]
        [InlineData(0)]
        public void GetForLeague_unsuccessful_DoesNotReturnPlayerList(int id)
        {
            //Arrange
            _mockedLeagueRepository.Setup(mock => mock.IsValid(id)).Returns(true);
            _mockedPlayerRepository.Setup(mock => mock.GetForTeam(id)).Returns(ListPlayers());
            _mockedTeamRepository.Setup(mock => mock.GetForLeague(id)).Returns(ListTeams());

            //Act
            var players = _context.GetForLeague(id);

            //Assert
            players.Should().AllBeAssignableTo<Player>().And.NotBeNull();
        }

        [Fact]
        public void GetByID_OnlyValidId_ReturnPlayer()
        {
            // Arrange
            Player player = new Fixture().Create<Player>();

            _mockedPlayerRepository.Setup(x => x.GetByID(player.ID)).Returns(player);

            // Act
            var actualPlayer = _context.GetByID(player.ID);

            // Assert
            actualPlayer.Should().BeEquivalentTo(player);
        }

        [Fact]
        public void GetByID_OnlyInvalidId_NotReturnPlayer()
        {
            // Arrange
            Player player = new Fixture().Create<Player>();
            var id = 0;

            _mockedPlayerRepository.Setup(x => x.GetByID(player.ID)).Returns(player);

            // Act
            var actualPlayer = _context.GetByID(id);

            // Assert
            actualPlayer.Should().BeEquivalentTo(player);
        }

        [Fact]
        public void GetByID_WrongObject_ReturnErrorComparingPlayer()
        {
            // Arrange
            Player player1 = new Player()
            {

                ID = 1,
                FirstName = "Fábio",
                LastName = "Garcia",
                DateOfBirth = new DateTime(2003, 01, 27),
                TeamID = 1

            };

            Player player2 = new Fixture().Create<Player>();

            var fakeId = 0;

            _mockedPlayerRepository.Setup(x => x.GetByID(player2.ID)).Returns(player2);

            // Act
            var actualPlayer = _context.GetByID(player2.ID);

            // Assert
            player1.Should().BeEquivalentTo(player2);
        }

    }

}
