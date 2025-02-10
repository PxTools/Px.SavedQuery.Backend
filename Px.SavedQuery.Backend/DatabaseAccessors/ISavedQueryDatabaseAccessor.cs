namespace Px.SavedQuery.Backend.DatabaseAccessors
{
    public interface ISavedQueryDatabaseAccessor
    {
        string Load(int queryId);
        int Store(string query);

    }
}
