using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using WarehouseManagementSystem.Interfaces;
using WarehouseManagementSystem.Services;

namespace WarehouseManagementSystem.Tests1
{
    [TestClass]
    public class ContractorCheckServiceUnitTests
    {
        private Mock<IContractorApiClient> _mockApi;
        private Mock<IContractorRepository> _mockRepo;
        private Mock<INetworkChecker> _mockNetwork;
        private ContractorCheckService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockApi = new Mock<IContractorApiClient>();
            _mockRepo = new Mock<IContractorRepository>();
            _mockNetwork = new Mock<INetworkChecker>();
            _service = new ContractorCheckService(_mockApi.Object, _mockRepo.Object, _mockNetwork.Object);
        }

        [TestMethod]
        public async Task CheckByInn_WhenInternetAvailable_ReturnsResultNotFromCache()
        {
            // Arrange
            string inn = "7707083893";
            _mockNetwork.Setup(n => n.IsInternetAvailable()).ReturnsAsync(true);
            _mockApi.Setup(api => api.FindParty(inn)).ReturnsAsync(new object());

            // Act
            var result = await _service.CheckByInn(inn, 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(inn, result.Inn);
            Assert.IsFalse(result.IsFromCache);
        }

        [TestMethod]
        public async Task CheckByInn_WhenNoInternetButCacheExists_ReturnsCachedResult()
        {
            // Arrange
            string inn = "7707083893";
            _mockNetwork.Setup(n => n.IsInternetAvailable()).ReturnsAsync(false);

            var cachedResult = new ContractorCheckResult
            {
                Inn = inn,
                Status = "RELIABLE",
                Message = "Контрагент надёжный",
                CheckedAt = DateTime.Now.AddDays(-1)
            };
            _mockRepo.Setup(r => r.LoadLatestByInn(inn)).Returns(cachedResult);

            // Act
            var result = await _service.CheckByInn(inn, 1);

            // Assert
            Assert.IsTrue(result.IsFromCache);
            Assert.IsTrue(result.Message.StartsWith("[КЭШ]"));
        }

        [TestMethod]
        public async Task CheckByInn_WhenNoInternetAndNoCache_ReturnsUnknown()
        {
            // Arrange
            string inn = "7707083893";
            _mockNetwork.Setup(n => n.IsInternetAvailable()).ReturnsAsync(false);
            _mockRepo.Setup(r => r.LoadLatestByInn(inn)).Returns((ContractorCheckResult)null);

            // Act
            var result = await _service.CheckByInn(inn, 1);

            // Assert
            Assert.AreEqual("UNKNOWN", result.Status);
            Assert.IsTrue(result.Message.Contains("нет интернета"));
        }

        [TestMethod]
        public async Task CheckByInn_WhenInternetAvailableButApiThrowsError_AndCacheExists_ReturnsCached()
        {
            // Arrange
            string inn = "7707083893";
            _mockNetwork.Setup(n => n.IsInternetAvailable()).ReturnsAsync(true);
            _mockApi.Setup(api => api.FindParty(inn)).ThrowsAsync(new Exception("API Error"));

            var cachedResult = new ContractorCheckResult
            {
                Inn = inn,
                Status = "RELIABLE",
                Message = "Из кэша",
                CheckedAt = DateTime.Now.AddDays(-1)
            };
            _mockRepo.Setup(r => r.LoadLatestByInn(inn)).Returns(cachedResult);

            // Act
            var result = await _service.CheckByInn(inn, 1);

            // Assert
            Assert.IsTrue(result.IsFromCache);
        }

        [TestMethod]
        public async Task CheckByInn_WithEmptyInn_ReturnsUnknown()
        {
            // Arrange
            _mockNetwork.Setup(n => n.IsInternetAvailable()).ReturnsAsync(true);
            _mockApi.Setup(api => api.FindParty("")).ReturnsAsync(new object());

            // Act
            var result = await _service.CheckByInn("", 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UNKNOWN", result.Status);
        }
    }
}