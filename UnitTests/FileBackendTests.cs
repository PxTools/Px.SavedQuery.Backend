using Px.SavedQuery.Backend;
using PxWeb.Api2.Server.Models;

namespace UnitTests
{
    [TestClass]
    public class FileBackendTests
    {
        [TestMethod]
        public void StoreSavedQuery_ReturnsAId()
        {
            // Arrange
            var query = new SavedQuery();
            var backend = new FileBackend(Path.GetTempPath());
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
            var backend = new FileBackend(Path.GetTempPath());
            // Act
            var result = backend.StoreSavedQuery(query);
            var actual = backend.LoadSavedQuery(result);
            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(query.Language, actual.Language);
        }
    }
}
