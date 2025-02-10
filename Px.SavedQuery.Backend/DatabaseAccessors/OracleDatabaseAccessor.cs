using Oracle.ManagedDataAccess.Client;

namespace Px.SavedQuery.Backend.DatabaseAccessors
{
    public class OracleDatabaseAccessor : ISavedQueryDatabaseAccessor
    {

        private readonly string _connectionString;
        private readonly string _savedQueryTableOwner;
        private readonly string _dataSourceType;
        private readonly string _databaseId;
        public OracleDatabaseAccessor(string connectionString, string dataSourceType, string databaseId, string tableOwner)
        {
            _connectionString = connectionString;
            _savedQueryTableOwner = tableOwner;
            _dataSourceType = dataSourceType;
            _databaseId = databaseId;
        }

        public string Load(int queryId)
        {
            using (var conn = new OracleConnection(_connectionString))
            {
                conn.Open();

                var cmd = new OracleCommand("select QueryText from " + _savedQueryTableOwner + ".SavedQueryMeta where QueryId = :queryId", conn);
                cmd.Parameters.Add("queryId", queryId);
                string? query = cmd.ExecuteScalar() as string;

                return query ?? "";
            }


        }

        public int Store(string query)
        {
            using (var conn = new OracleConnection(_connectionString))
            {
                int? id = null;
                string insertSQL = @"BEGIN
                        insert into 
                        {3}.SavedQueryMeta
                        (
                            {0}
                            DataSourceType, 
	                        DatabaseId, 
	                        DataSourceId, 
	                        ""STATUS"", 
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
                            {1}
	                        :databaseType,
	                        :databaseId,
	                        :mainTable,
	                        'A',
	                        'P',
	                        'P',
	                        'Anonymous',
	                        :title,
	                        sysdate,
	                        'PXSJSON',
	                        'D',
	                        :query,
                            0,
	                        0
                        ) {2};
                        END;";

                string queryIdPartCol = "";
                string queryIdPartValue = "";
                string returningPart = "returning queryid into :identity";


                insertSQL = string.Format(insertSQL, queryIdPartCol, queryIdPartValue, returningPart, _savedQueryTableOwner);

                conn.Open();
                var cmd = new OracleCommand(insertSQL, conn);
                cmd.BindByName = true;
                cmd.Parameters.Add("databaseType", _dataSourceType);
                cmd.Parameters.Add("databaseId", _databaseId);
                cmd.Parameters.Add("mainTable", "");
                cmd.Parameters.Add("title", " ");
                cmd.Parameters.Add("query", OracleDbType.Clob, query, System.Data.ParameterDirection.Input);
                cmd.Parameters.Add("identity", OracleDbType.Int16, System.Data.ParameterDirection.ReturnValue);

                if (id != null)
                {
                    cmd.Parameters.Add("queryId", id.Value);
                }

                cmd.ExecuteNonQuery();

                if (id == null)
                {
                    var newIdString = cmd.Parameters["identity"].Value.ToString();
                    if (newIdString is null)
                    {
                        return -1;
                    }
                    int newId = int.Parse(newIdString);
                    return newId;
                }
                else
                {
                    return id.Value;
                }
            }

        }
    }
}
