using Microsoft.Data.SqlClient;

namespace Px.SavedQuery.Backend.DatabaseAccessors
{
    public class MicrosoftSqlServerDatabaseAccessor : ISavedQueryDatabaseAccessor
    {

        private readonly string _connectionString;
        private readonly string _dataSourceType;
        private readonly string _databaseId;

        public MicrosoftSqlServerDatabaseAccessor(string connectionString, string dataSourceType, string databaseId)
        {
            _connectionString = connectionString;
            _dataSourceType = dataSourceType;
            _databaseId = databaseId;
        }

        public string Load(int queryId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("select QueryText from SavedQueryMeta where QueryId = @queryId", conn);
                cmd.Parameters.AddWithValue("queryId", queryId);
                string? query = cmd.ExecuteScalar() as string;

                return query ?? "";
            }
        }

        public int Store(string query)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    @"insert into 
                        SavedQueryMeta
                        (
	                        DataSourceType, 
	                        DatabaseId, 
	                        DataSourceId, 
	                        [Status], 
	                        StatusUse, 
	                        StatusChange, 
	                        OwnerId, 
	                        MyDescription, 
	                        CreatedDate, 
	                        SavedQueryFormat, 
	                        SavedQueryStorage, 
	                        QueryText,
                            Runs,
                            Fails
                        )
                        values
                        (
	                        @databaseType,
	                        @databaseId,
	                        @mainTable,
	                        'A',
	                        'P',
	                        'P',
	                        'Anonymous',
	                        @title,
	                        @creationDate,
	                        'SQA',
	                        'D',
	                        @query,
                            0,
	                        0
                        );
                        SELECT @@IDENTITY AS 'Identity';", conn);
                cmd.Parameters.AddWithValue("databaseType", "N/A");
                cmd.Parameters.AddWithValue("databaseId", "N/A");
                cmd.Parameters.AddWithValue("mainTable", "N/A");
                cmd.Parameters.AddWithValue("title", "");
                cmd.Parameters.AddWithValue("creationDate", DateTime.Now);
                cmd.Parameters.AddWithValue("query", query);
                int newid = Convert.ToInt32(cmd.ExecuteScalar());
                return newid;
            }
        }
    }
}
