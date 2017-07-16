using System;
using System.Data.SqlClient;
using System.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis.TokenSeparatorHandlers;

namespace NBi.Core.DataManipulation.SqlServer
{
    class BulkLoadCommand : IDecorationCommandImplementation
    {
        private readonly string connectionString;
        private readonly string tableName;
        private readonly string fileName;

        public BulkLoadCommand(ILoadCommand command, SqlConnection connection)
        {
            connectionString = connection.ConnectionString;
            tableName = command.TableName;
            fileName = command.FileName;
        }

        public void Execute()
        {
            Execute(connectionString, tableName, fileName);
        }

        internal void Execute(string connectionString, string tableName, string filename)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                // make sure to enable triggers
                // more on triggers in next post
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                    (
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null
                    );

                if (!filename.EndsWith(".xlsx"))
                {
                    // set the destination table name
                    bulkCopy.DestinationTableName = tableName;
                    connection.Open();
                    // write the data in the "dataTable"
                    var fileReader = new CsvReader();
                    var dataTable = fileReader.Read(filename, false);
                    bulkCopy.WriteToServer(dataTable);

                    connection.Close();
                }

                if (filename.EndsWith(".xlsx"))
                {
                    string sheetname;
                    if (tableName.StartsWith("#"))
                    {
                        sheetname = tableName;
                    }
                    else
                    {
                        sheetname = tableName.Replace(".", "#");
                    }

                    ExcelDefinition excelDefinition = new ExcelDefinition();
                    excelDefinition.SheetName = sheetname;
                    // write the data in the "dataTable"
                    var fileReader = new ExcelReader(excelDefinition);
                    var dataTable = fileReader.Read(filename, false);

                    if (dataTable.ExtendedProperties["NBi::BulkIdentityInsert"].Equals(true))
                    {
                        bulkCopy =
                            new SqlBulkCopy
                            (
                                connection,
                                SqlBulkCopyOptions.TableLock |
                                SqlBulkCopyOptions.UseInternalTransaction |
                                SqlBulkCopyOptions.KeepIdentity,
                                null
                            );
                    }
                    // set the destination table name
                    bulkCopy.DestinationTableName = tableName;
                    connection.Open();

                    bulkCopy.WriteToServer(dataTable);

                    connection.Close();
                }
            }
        }
    }
}
