using System;
using System.Collections.Generic;
using System.Text;
using RightPoint.Data.Generation.Analyzer;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace RightPoint.Data.Generation
{
    // http://ryanfarley.com/blog/archive/2004/03/23/465.aspx
    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {
        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        public WindowWrapper() : this(GetDesktopWindow())
        {

        }

        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }




    public partial class DALTemplate : IDALTemplate
    {
        public readonly GenerationParameters GenerationParameters;
		public readonly Analyzer.IDatabaseAnalyzer DatabaseAnalyzer;

        public DALTemplate(GenerationParameters generationParameters)
        {
            GenerationParameters = generationParameters;

            switch (generationParameters.DatabaseType)
            {
                case DatabaseType.SqlServer:
            DatabaseAnalyzer = new DatabaseAnalyzer(this);
                    break;

                case DatabaseType.MySQL:
                    DatabaseAnalyzer = new MySqlDatabaseAnalyzer(this);
                    break;

                default:
                    throw new NotSupportedException(generationParameters.DatabaseType.ToString());
            }

            

            //var webservice = DatabaseAnalyzer.GetStoredProcedureSchema("server=tndevsql;database=tnow;trusted_connection=true;", new Analyzer.SysObjectRecord("WebService_SearchTickets", "dbo"));
        }

        public string GenerateDataLayerCode()
        {
            StringBuilder dataLayerCode = new StringBuilder();

            UI.Progress progress = new UI.Progress();
            progress.label_ProcessedProcCount.Text = "0 / " + GenerationParameters.StoredProcedures.Count.ToString();
            progress.label_StoredProcName.Text = "";
            progress.progressBar_Progress.Minimum = 0;
            progress.progressBar_Progress.Maximum = GenerationParameters.StoredProcedures.Count + 1;

            WindowWrapper wrapper = new WindowWrapper();
            progress.Show(wrapper);
            progress.BringToFront();

            int counter = 0;
            string currentProcName = String.Empty;

            try
            {
                foreach (var storedProcedure in GenerationParameters.StoredProcedures)
                {
                    counter++;

                    currentProcName = storedProcedure;

                    progress.label_ProcessedProcCount.Text = counter.ToString() + " / " + GenerationParameters.StoredProcedures.Count.ToString();
                    progress.label_StoredProcName.Text = storedProcedure;
                    progress.progressBar_Progress.Value = counter;
                    progress.Invalidate();
                    progress.Update();
                    progress.Refresh();

                    var storedProcedureSchema = DatabaseAnalyzer.GetStoredProcedureSchema(GenerationParameters.ConnectionString, new Analyzer.SysObjectRecord(storedProcedure, GenerationParameters.DatabaseObjectOwner));

                    dataLayerCode.AppendFormat("#region [{0}]{1}", storedProcedure, System.Environment.NewLine);

                    switch (storedProcedureSchema.StoredProcedureType)
                    {
                        case Analyzer.StoredProcedureType.SingleRow:
                            HandleOutputEntity(storedProcedureSchema, dataLayerCode);
                            break;

                        case Analyzer.StoredProcedureType.BothSingleAndMultiRow:
                        case Analyzer.StoredProcedureType.MultiRow:
                        case Analyzer.StoredProcedureType.SingleRecordSet:
                            if (storedProcedureSchema.HasOutputParameters == true)
                            {
                                dataLayerCode.AppendLine("////////////////////////////////////////////////////////////////////////////////////////////////////////////");
                                dataLayerCode.AppendLine("//The stored procedure had output parameters, but they are being ignored in favor of the record set.");
                                dataLayerCode.AppendLine("////////////////////////////////////////////////////////////////////////////////////////////////////////////");
                            }

                            HandleResultSetCollection(storedProcedureSchema, dataLayerCode);
                            break;

                        case Analyzer.StoredProcedureType.NoResultSet:
                            HandleReturnValue(storedProcedureSchema, dataLayerCode);
                            break;

                        case Analyzer.StoredProcedureType.SingleValue:
                            HandleSimpleType(storedProcedureSchema, dataLayerCode);
                            break;

                        //throw new ApplicationException( "The stored procedure \"" + storedProcedure + "\" cannot be automatically generated, please correct the procedure or remove it from the list of procedures to auto-generate." );
                        default:
                            this.WriteLine("Invalid output type." + storedProcedureSchema.StoredProcedureType);
                            System.Diagnostics.Debugger.Break();
                            throw new ApplicationException("Cannot render for type \"" + storedProcedureSchema.StoredProcedureType + "\"");
                    }

                    dataLayerCode.AppendLine("#endregion");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(wrapper, currentProcName + " " + ex.ToString(), "DAL Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

            progress.Close();

            return dataLayerCode.ToString();
        }

        public void HandleOutputEntity(StoredProcedureSchema storedProcedure, StringBuilder dataLayerCode)
        {
            RenderEntity(GeneratedTypes.OutputEntity, storedProcedure, dataLayerCode);
            RenderHelperMethod(GeneratedTypes.OutputEntity, storedProcedure, dataLayerCode);
        }

        public void HandleResultSetCollection(StoredProcedureSchema storedProcedure, StringBuilder dataLayerCode)
        {
            RenderEntity(GeneratedTypes.ResultSetCollection, storedProcedure, dataLayerCode);
            RenderEntityCollection(storedProcedure, dataLayerCode);
            RenderHelperMethod(GeneratedTypes.ResultSetCollection, storedProcedure, dataLayerCode);
        }

        public void HandleReturnValue(StoredProcedureSchema storedProcedure, StringBuilder dataLayerCode)
        {
            RenderHelperMethod(GeneratedTypes.ReturnValue, storedProcedure, dataLayerCode);
        }

        public void HandleSimpleType(StoredProcedureSchema storedProcedure, StringBuilder dataLayerCode)
        {
            RenderHelperMethod(GeneratedTypes.SimpleType, storedProcedure, dataLayerCode);
        }

        public void RenderEntity(GeneratedTypes generateType, StoredProcedureSchema storedProcedure, StringBuilder dataLayerCode)
        {
            DALEntity entity = new DALEntity(storedProcedure);

            dataLayerCode.AppendLine(entity.TransformText());

            //Entity entity = new Entity();
            //entity.EntityType = generateType;
            //entity.StoredProcedure = new SchemaExplorer.CommandSchema(Database, storedProcedure, DatabaseObjectOwner, DateTime.Now);
            //Templates.ParsedStoredProcedureName parsedNames = TemplateHelper.ParseStoredProcedureName(entity.StoredProcedure);
            //entity.EntityName = parsedNames.EntityName;
            //entity.Database = Database;
            //entity.Render(this.Response);
        }

        public void RenderEntityCollection(StoredProcedureSchema storedProcedure, StringBuilder dataLayerCode)
        {
            DALEntityCollection entityCollection = new DALEntityCollection(storedProcedure);

            dataLayerCode.AppendLine(entityCollection.TransformText());

            //EntityCollection entityCollection = new EntityCollection();

            //entityCollection.StoredProcedure = new SchemaExplorer.CommandSchema(Database, storedProcedure, DatabaseObjectOwner, DateTime.Now);
            //Templates.ParsedStoredProcedureName parsedNames = TemplateHelper.ParseStoredProcedureName(entityCollection.StoredProcedure);
            //entityCollection.EntityName = parsedNames.EntityName;
            //entityCollection.Database = Database;
            //entityCollection.Render(this.Response);
        }

        public void RenderHelperMethod(GeneratedTypes generateType, StoredProcedureSchema storedProcedure, StringBuilder dataLayerCode)
        {
            DALHelperMethod helperMethod = new DALHelperMethod(GenerationParameters, storedProcedure, generateType);

            dataLayerCode.AppendLine(helperMethod.TransformText());

            //HelperMethod helperMethod = new HelperMethod();
            //helperMethod.EntityType = generateType;
            //helperMethod.StoredProcedure = new SchemaExplorer.CommandSchema(Database, storedProcedure, DatabaseObjectOwner, DateTime.Now);
            //Templates.ParsedStoredProcedureName parsedNames = TemplateHelper.ParseStoredProcedureName(helperMethod.StoredProcedure);
            //helperMethod.EntityName = parsedNames.EntityName;
            //helperMethod.MethodName = parsedNames.HelperMethodName;
            //helperMethod.Database = Database;
            //helperMethod.CommandTimeout = CommandTimeout;
            //helperMethod.ConnectionKey = ConnectionKey;
            //helperMethod.DatabaseObjectOwner = DatabaseObjectOwner;
            //helperMethod.Render(this.Response);
        }
    }
}
