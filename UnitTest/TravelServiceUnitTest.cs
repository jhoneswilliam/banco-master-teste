using AutoMapper;
using Core.Services;
using Domain.DTO.Request;
using Domain.DTO.Response;
using Domain.Entities;
using Domain.Interfaces.Repository;
using Domain.Interfaces.Services;
using Infrastructure.Repositories;
using Moq;

namespace UnitTest
{
    [TestClass]
    public class TravelServiceUnitTest
    {
        private IMapper? _mapper;
        private Mock<IHeuristicService>? _heuristicServiceMock;
        private Mock<ITravelCostFileRepository>? _travelCostFileRepository;
        private TravelService? _travelService;
        private IList<ImportTravelsCostRequest>? _travelCostsListRequest;
        private IList<TravelCost>? _travelCostList;

        [TestInitialize]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ImportTravelsCostRequest, TravelCost>();
                cfg.CreateMap<TravelCost, BestCostTravelResponse>();
            });

            _mapper = config.CreateMapper();

            _heuristicServiceMock = new Mock<IHeuristicService>();
            _travelCostFileRepository = new Mock<ITravelCostFileRepository>();
            _travelService = new TravelService(_travelCostFileRepository.Object, _heuristicServiceMock.Object, _mapper);

            _travelCostsListRequest = new List<ImportTravelsCostRequest>
            {
                new ImportTravelsCostRequest { Departure = "GRU", Arrival = "BRC", Cost = 10 },
                new ImportTravelsCostRequest { Departure = "BRC", Arrival = "SCL", Cost = 5 },
                new ImportTravelsCostRequest { Departure = "GRU", Arrival = "CDG", Cost = 75 },
                new ImportTravelsCostRequest { Departure = "GRU", Arrival = "SCL", Cost = 20 },
                new ImportTravelsCostRequest { Departure = "GRU", Arrival = "ORL", Cost = 56 },
                new ImportTravelsCostRequest { Departure = "ORL", Arrival = "CDG", Cost = 5 },
                new ImportTravelsCostRequest { Departure = "SCL", Arrival = "ORL", Cost = 20 }
            };

            _travelCostList = new List<TravelCost>
            {
                new TravelCost { Departure = "GRU", Arrival = "BRC", Cost = 10 },
                new TravelCost { Departure = "BRC", Arrival = "SCL", Cost = 5 },
                new TravelCost { Departure = "GRU", Arrival = "CDG", Cost = 75 },
                new TravelCost { Departure = "GRU", Arrival = "SCL", Cost = 20 },
                new TravelCost { Departure = "GRU", Arrival = "ORL", Cost = 56 },
                new TravelCost { Departure = "ORL", Arrival = "CDG", Cost = 5 },
                new TravelCost { Departure = "SCL", Arrival = "ORL", Cost = 20 }
            };
        }


        #region GetBestCostTravel        
        [TestMethod]
        public async Task Test_GetBestCostTravel_Sucess()
        {
            // Arrange
            var departure = "GRU";
            var arrival = "CDG";

            var expectedPath = new List<string> { "GRU", "BRC", "SCL", "ORL", "CDG" };
            var expectedResponsePath = string.Join('-', expectedPath);
            var expectedCost = 40;

            _heuristicServiceMock!.Setup(x => x.DijkstraAlgorithm(It.IsAny<DijkstraRequest>()))
                .Returns(new DijkstraResponse
                {
                    Path = expectedPath,
                    TotalCost = expectedCost,
                    Message = string.Empty
                });

            _travelCostFileRepository!.Setup(x => x.GetAllTravels()).Returns(Task.FromResult(_travelCostList!));

            // Act
            var result = await _travelService!.GetBestCostTravel(departure, arrival, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCost, result.TotalCost);
            Assert.AreEqual(expectedResponsePath, result.Route);
        }

        [TestMethod]
        public async Task Test_GetBestCostTravel_NotPath()
        {
            // Arrange
            var departure = "SCL";
            var arrival = "GRU";

            _heuristicServiceMock!.Setup(x => x.DijkstraAlgorithm(It.IsAny<DijkstraRequest>()))
                .Returns(new DijkstraResponse
                {
                    Path = new List<string>(),
                    TotalCost = int.MaxValue,
                    Message = "Não foi encontrato caminho!"
                });

            _travelCostFileRepository!.Setup(x => x.GetAllTravels()).Returns(Task.FromResult(_travelCostList!));

            // Act
            var result = await _travelService!.GetBestCostTravel(departure, arrival, CancellationToken.None);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Test_GetBestCostTravel_IOError()
        {
            // Arrange
            Exception? exception = null;
            _travelCostFileRepository!.Setup(x => x.GetAllTravels())
                .Throws(new IOException("The process cannot access the file because it is being used by another process"));

            var departure = "GRU";
            var arrival = "CDG";

            // Act
            try
            {
                await _travelService!.GetBestCostTravel(departure, arrival, CancellationToken.None);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception is IOException);
        }
        #endregion


        #region ImportCostTravel      
        [TestMethod]
        public async Task Test_ImportCostTravel_Sucesso()
        {
            // Arrange
            var listToImport = _travelCostsListRequest;

            // Act
            var task = _travelService!.ImportCostTravel(listToImport);
            await task;

            // Assert
            Assert.IsTrue(task.IsCompleted);

        }

        [TestMethod]
        public async Task Test_ImportCostTravel_Erro()
        {
            // Arrange
            var listToImport = _travelCostsListRequest;
            Exception? exception = null;
            _travelCostFileRepository!.Setup(x => x.UpsertBulk(It.IsAny<IList<TravelCost>>()))
                .Throws(new IOException("The process cannot access the file because it is being used by another process"));

            // Act
            try
            {
                await _travelService!.ImportCostTravel(listToImport);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception is IOException);
        }
        #endregion

        #region RemoveAllSavedTavelCosts
        [TestMethod]
        public void Test_RemoveAllSavedTavelCosts_Sucesso()
        {
            // Arrange
            Exception? exception = null;

            // Act
            try
            {
                _travelService!.RemoveAllSavedTavelCosts();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsNull(exception);
        }

        [TestMethod]
        public void Test_RemoveAllSavedTavelCosts_Error()
        {
            // Arrange
            Exception? exception = null;
            _travelCostFileRepository!.Setup(x => x.RemoveAll())
                .Throws(new IOException("The process cannot access the file because it is being used by another process"));

            // Act
            try
            {
                _travelService!.RemoveAllSavedTavelCosts();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception is IOException);
        }
        #endregion
    }
}