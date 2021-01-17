using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Dapper;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using Microsoft.Data.SqlClient;

namespace DapperExtensions.Test.IntegrationTests.SqlServer
{
    public class SqlServerBaseFixture
    {
        protected IDatabase Db;

        public SqlServerBaseFixture()
        {
	        var connection = new SqlConnection("Data Source=.;Initial Catalog=dapperTest;Integrated security=True;");
	        var config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect());
	        var sqlGenerator = new SqlGeneratorImpl(config);
	        Db = new Database(connection, sqlGenerator);
	        var files = new List<string>
	        {
		        ReadScriptFile("CreateAnimalTable"),
		        ReadScriptFile("CreateFooTable"),
		        ReadScriptFile("CreateMultikeyTable"),
		        ReadScriptFile("CreatePersonTable"),
		        ReadScriptFile("CreateCarTable")
	        };

	        foreach (string setupFile in files)
	        {
		        connection.Execute(setupFile);
	        }
        }

        public string ReadScriptFile(string name)
        {
            string fileName = GetType().Namespace + ".Sql." + name + ".sql";
            using var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
            // ReSharper disable once AssignNullToNotNullAttribute
            using var sr = new StreamReader(s);
            return sr.ReadToEnd();
        }
    }
}