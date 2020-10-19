namespace RightPoint.Data.Generation.Analyzer
{
    public interface IDatabaseAnalyzer
    {
        string GetDatabaseName(string connectionString);
        StoredProcedureSchema GetStoredProcedureSchema(string connectionString, SysObjectRecord storedProcedure);
    }
}