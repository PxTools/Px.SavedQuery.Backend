
using Newtonsoft.Json;

namespace Px.SavedQuery.Backend
{
    public class FileBackend : ISavedQueryBackend
    {
        private readonly string _basePath;
        public FileBackend(string path)
        {
            _basePath = path;
        }

        public string StoreSavedQuery(PxWeb.Api2.Server.Models.SavedQuery query)
        {
            var id = Guid.NewGuid().ToString();
            var path = Path.Combine(_basePath, id) + ".sqa";
            //TODO check if file exists
            query.Id = id;
            File.WriteAllText(path, JsonConvert.SerializeObject(query));
            return id;
        }

        public PxWeb.Api2.Server.Models.SavedQuery? LoadSavedQuery(string queryId)
        {
            var path = Path.Combine(_basePath, queryId) + ".sqa";
            if (!File.Exists(path))
            {
                return null;
            }

            var content = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<PxWeb.Api2.Server.Models.SavedQuery>(content);
        }
    }
}
