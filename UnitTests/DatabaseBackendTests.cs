using Moq;
using Px.SavedQuery.Backend;
using Px.SavedQuery.Backend.DatabaseAccessors;
using PxWeb.Api2.Server.Models;

namespace UnitTests
{
    [TestClass]
    public class DatabaseBackendTests
    {
        [TestMethod]
        public void StoreSavedQuery_ReturnsAId()
        {
            // Arrange
            var query = new SavedQuery();

            var accessor = new Mock<ISavedQueryDatabaseAccessor>();

            var backend = new DatabaseBackend(accessor.Object);
            // Act
            var result = backend.StoreSavedQuery(query);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(query.Id, result);

        }

        [TestMethod]
        public void LoadSavedQuery_ReturnsSameSavedQueryAsStored()
        {
            // Arrange
            var query = new SavedQuery() { Language = "sv" };

            string data = "olle";
            var accessor = new Mock<ISavedQueryDatabaseAccessor>();
            accessor.Setup(x => x.Store(It.IsAny<string>())).
                Callback<string>(sq =>
                {
                    data = sq;
                }).Returns(1);
            accessor.Setup(x => x.Load(It.IsAny<int>())).Returns(() => data);

            var backend = new DatabaseBackend(accessor.Object);
            // Act
            var result = backend.StoreSavedQuery(query);
            var actual = backend.LoadSavedQuery(result);
            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(query.Language, actual.Language);
            Assert.AreEqual("1", actual.Id);
        }
    }
}
