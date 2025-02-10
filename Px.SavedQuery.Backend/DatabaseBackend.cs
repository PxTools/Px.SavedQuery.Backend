using Newtonsoft.Json;
using Px.SavedQuery.Backend.DatabaseAccessors;

namespace Px.SavedQuery.Backend
{
    public class DatabaseBackend : ISavedQueryBackend
    {
        private readonly ISavedQueryDatabaseAccessor _databaseAccessor;

        public DatabaseBackend(ISavedQueryDatabaseAccessor databaseAccessor)
        {
            _databaseAccessor = databaseAccessor;
        }

        public string StoreSavedQuery(PxWeb.Api2.Server.Models.SavedQuery query)
        {
            var queryString = JsonConvert.SerializeObject(query);
            var id = _databaseAccessor.Store(queryString);
            query.Id = id.ToString();
            return query.Id;
        }

        public PxWeb.Api2.Server.Models.SavedQuery? LoadSavedQuery(string queryId)
        {
            if (!int.TryParse(queryId, out int id))
            {
                return null;
            }
            var queryString = _databaseAccessor.Load(id);
            var query = JsonConvert.DeserializeObject<PxWeb.Api2.Server.Models.SavedQuery>(queryString);

            if (query is null)
            {
                return null;
            }

            // Set the query id
            query.Id = queryId;

            return query;
        }
    }
}
