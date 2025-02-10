namespace Px.SavedQuery.Backend
{
    public interface ISavedQueryBackend
    {
        PxWeb.Api2.Server.Models.SavedQuery? LoadSavedQuery(string queryId);

        string StoreSavedQuery(PxWeb.Api2.Server.Models.SavedQuery query);
    }
}
