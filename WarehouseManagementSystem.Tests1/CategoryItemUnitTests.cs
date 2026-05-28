using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarehouseManagementSystem.Tests1
{
    // Если у тебя нет класса CategoryItem - создай его или удали этот файл
    [TestClass]
    public class CategoryItemUnitTests
    {
        [TestMethod]
        public void CategoryItem_ToString_ReturnsName()
        {
            // Arrange
            var item = new CategoryItem { Id = 1, Name = "Тестовая категория" };

            // Act
            var result = item.ToString();

            // Assert
            Assert.AreEqual("Тестовая категория", result);
        }

        [TestMethod]
        public void CategoryItem_CanSetIdAndName()
        {
            // Act
            var item = new CategoryItem
            {
                Id = 5,
                Name = "Электроника"
            };

            // Assert
            Assert.AreEqual(5, item.Id);
            Assert.AreEqual("Электроника", item.Name);
        }

        [TestMethod]
        public void CategoryItem_EmptyName_ToStringReturnsEmpty()
        {
            // Arrange
            var item = new CategoryItem { Id = 2, Name = "" };

            // Act
            var result = item.ToString();

            // Assert
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void CategoryItem_NullName_ToStringReturnsNull()
        {
            // Arrange
            var item = new CategoryItem { Id = 3, Name = null };

            // Act
            var result = item.ToString();

            // Assert
            Assert.IsNull(result);
        }
    }
}