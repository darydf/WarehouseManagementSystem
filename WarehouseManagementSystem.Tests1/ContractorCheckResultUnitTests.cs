using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WarehouseManagementSystem.Services;

namespace WarehouseManagementSystem.Tests1
{
    [TestClass]
    public class ContractorCheckResultUnitTests
    {
        [TestMethod]
        public void ContractorCheckResult_DefaultConstructor_CreatesEmptyObject()
        {
            // Act
            var result = new ContractorCheckResult();

            // Assert
            Assert.IsNull(result.Inn);
            Assert.IsNull(result.Status);
            Assert.IsNull(result.Message);
            Assert.IsNull(result.CounterpartyName);
            Assert.IsFalse(result.IsFromCache);
            Assert.IsFalse(result.HasTaxDebt);
            Assert.IsFalse(result.IsBankrupt);
            Assert.IsFalse(result.IsDirectorDisqual);
            Assert.AreEqual(DateTime.MinValue, result.CheckedAt);
        }

        [TestMethod]
        public void ContractorCheckResult_CanSetAllProperties()
        {
            // Arrange
            var expectedDate = DateTime.Now;

            // Act
            var result = new ContractorCheckResult
            {
                Inn = "7707083893",
                Status = "RELIABLE",
                Message = "Контрагент надёжный",
                CounterpartyName = "ПАО СБЕРБАНК",
                IsFromCache = false,
                HasTaxDebt = false,
                IsBankrupt = false,
                IsDirectorDisqual = false,
                CheckedAt = expectedDate
            };

            // Assert
            Assert.AreEqual("7707083893", result.Inn);
            Assert.AreEqual("RELIABLE", result.Status);
            Assert.AreEqual("Контрагент надёжный", result.Message);
            Assert.AreEqual("ПАО СБЕРБАНК", result.CounterpartyName);
            Assert.IsFalse(result.IsFromCache);
            Assert.IsFalse(result.HasTaxDebt);
            Assert.IsFalse(result.IsBankrupt);
            Assert.IsFalse(result.IsDirectorDisqual);
            Assert.AreEqual(expectedDate, result.CheckedAt);
        }

        [TestMethod]
        public void ContractorCheckResult_StatusCanBeBlacklisted()
        {
            // Act
            var result = new ContractorCheckResult
            {
                Inn = "1234567890",
                Status = "BLACKLISTED",
                Message = "Контрагент в чёрном списке",
                IsBankrupt = true
            };

            // Assert
            Assert.AreEqual("BLACKLISTED", result.Status);
            Assert.IsTrue(result.IsBankrupt);
        }

        [TestMethod]
        public void ContractorCheckResult_StatusCanBeUnknown()
        {
            // Act
            var result = new ContractorCheckResult
            {
                Inn = "9999999999",
                Status = "UNKNOWN",
                Message = "Контрагент не найден"
            };

            // Assert
            Assert.AreEqual("UNKNOWN", result.Status);
            Assert.AreEqual("Контрагент не найден", result.Message);
        }

        [TestMethod]
        public void ContractorCheckResult_IsFromCache_DefaultIsFalse()
        {
            // Act
            var result = new ContractorCheckResult();

            // Assert
            Assert.IsFalse(result.IsFromCache);
        }
    }
}