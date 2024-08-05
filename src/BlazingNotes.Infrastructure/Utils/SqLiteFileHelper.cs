using System.Data.Common;

namespace BlazingNotes.Infrastructure.Utils;

public static class SqLiteFileHelper
{
    public static string ResolveAndPrepareDatabaseFile(string connectionString)
    {
        var builder = new DbConnectionStringBuilder();
        builder.ConnectionString = connectionString;
        var dataSourceKey = "Data Source";
        if (!builder.ContainsKey(dataSourceKey))
            throw new Exception("Please provide a connection string in the form of 'Data Source=path/to/sqlite.file'");

        var dataSource = builder[dataSourceKey].ToString()!;
        dataSource = ResolveAppData(dataSource);
        var fileInfo = new FileInfo(dataSource);
        fileInfo.Directory!.Create();
        // file will be created by the DB driver

        builder[dataSourceKey] = fileInfo.FullName;

        return builder.ToString();
    }

    private static string ResolveAppData(string path)
    {
        var appDataValue = Environment.GetEnvironmentVariable("APPDATA");
        path = path.Replace("%appdata%", appDataValue, StringComparison.OrdinalIgnoreCase);
        return path;
    }
}