using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Data.Generation
{
	public class Generator
	{
		public string Generate(GenerationParameters generationParameters)
		{
			var databaseAnalyzer = new Analyzer.DatabaseAnalyzer(null);
			
			//var storedProcedureSchema = databaseAnalyzer.GetStoredProcedureSchema("server=localhost;database=npirocanac_unit_test_for_codegeneration;trusted_connection=true;", new Analyzer.SysObjectRecord("ap_RainbowTable_SelectAllRainbowTable", "dbo"));


			var webservice = databaseAnalyzer.GetStoredProcedureSchema("server=tndevsql;database=tnow;trusted_connection=true;", new Analyzer.SysObjectRecord("WebService_SearchTickets", "dbo"));


			return String.Empty;
		}
	}
}
