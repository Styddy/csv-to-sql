using System;
using System.Collections.Generic;

namespace CsvToMySql
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string tableName;
            List<string> columns;
            FileManager fileManager = new FileManager();
            DatabaseManager databaseManager = new DatabaseManager();

            Console.Title = "CsvToSql";

            if (args.Length > 0)
            {
                fileManager.FilePath = args[0];
            }
            else
            {
                do
                {
                    string filePath;
                    Console.Write("Paste the .csv path: ");
                    filePath = Console.ReadLine();

                    if (fileManager.IsValidFilePath(filePath))
                    {
                        fileManager.FilePath = filePath;
                        break;
                    }
                    Console.WriteLine("The file in the specified path does not exist!");
                } while (true);
            }

            do
            {
                string connectionString;
                Console.Write("Paste the database connection string: ");
                connectionString = Console.ReadLine();

                if (databaseManager.IsValidConnectionString(connectionString))
                {
                    databaseManager.ConnectionString = connectionString;
                    break;
                }
                Console.WriteLine("Indicated string is not valid!");
            } while (true);

            do
            {
                Console.Write("Enter the table name: ");
                tableName = Console.ReadLine();

                if (databaseManager.IsValidTableName(tableName))
                {
                    Console.WriteLine("Invalid table name!");
                }
                else
                {
                    try
                    {
                        databaseManager.TableName = tableName;
                        databaseManager.CreateTable();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            } while (true);

            columns = fileManager.RowReader(0);

            foreach (string column in columns)
            {
                bool error;

                do
                {
                    Console.Write("Enter the data type of the column {0}: ", column);
                    string dataType = Console.ReadLine().ToLower();

                    if (databaseManager.CheckDataType(dataType))
                    {
                        Console.Write("Specify the data size of {0} (Enter 0 if {0} does not need a size): ", dataType);
                        int dataSize = int.Parse(Console.ReadLine());

                        if (dataSize != 0)
                        {
                            databaseManager.AddColumn(column, dataType, dataSize);
                        }
                        else
                        {
                            databaseManager.AddColumn(column, dataType);
                        }

                        databaseManager.ColumnName.Add(column.Replace(" ", ""));
                        databaseManager.ColumnDataType.Add(databaseManager.StringToSqlDbType(dataType));
                        databaseManager.ColumnDataSize.Add(dataSize);

                        error = false;
                    }
                    else
                    {
                        Console.WriteLine("The data type \"{0}\" does not exist!", dataType);

                        error = true;
                    }
                } while (error);
            }

            int nRow = fileManager.RowCounter() - 1;
            try
            {
                for (int i = 1; i <= nRow; i++)
                {
                    Console.WriteLine("Loading row {0} of {1}", i, nRow);
                    databaseManager.AddRow(fileManager.RowReader(i));
                }
                Console.WriteLine("File loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
