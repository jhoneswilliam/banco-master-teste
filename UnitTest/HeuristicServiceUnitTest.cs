using Core.Services;
using Domain.DTO.Request;

namespace UnitTest
{
    [TestClass]
    public class HeuristicServiceUnitTest
    {
        private HeuristicService? _heuristicService;
        private Dictionary<string, List<(string, int)>>? _graph;

        [TestInitialize]
        public void SetUp()
        {
            _heuristicService = new HeuristicService();
            _graph = new Dictionary<string, List<(string, int)>>
            {
                { "GRU", new List<(string, int)>
                    {
                        ("BRC", 10),
                        ("CDG", 75),
                        ("SCL", 20),
                        ("ORL", 56)
                    }
                },
                { "BRC", new List<(string, int)>
                    {
                        ("SCL", 5)
                    }
                },
                { "ORL", new List<(string, int)>
                    {
                        ("CDG", 5)
                    }
                },
                { "SCL", new List<(string, int)>
                    {
                        ("ORL", 20)
                    }
                }
            };
        }

        [TestMethod]
        public void Test_DijkstraAlgorithm_Sucess()
        {
            // Arrange
            var expectedPath = "SCL-ORL-CDG";

            var request = new DijkstraRequest
            {
                Graph = _graph!,
                Source = "SCL",
                Destination = "CDG"
            };

            // Act
            var resultado = _heuristicService!.DijkstraAlgorithm(request);

            // Assert
            Assert.AreNotEqual(int.MaxValue, resultado.TotalCost);
            Assert.AreEqual(expectedPath, string.Join('-', resultado.Path));
        }

        [TestMethod]
        public void Test_DijkstraAlgorithm_NotPath()
        {
            // Arrange
            var request = new DijkstraRequest
            {
                Graph = _graph!,
                Source = "SCL",
                Destination = "GRU"
            };

            // Act
            var resultado = _heuristicService!.DijkstraAlgorithm(request);

            // Assert
            Assert.AreEqual(int.MaxValue, resultado.TotalCost);
        }
    }
}