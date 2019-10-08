using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CsvToMySql
{
    public class DatabaseManager
    {
        public string ConnectionString { get; set; }

        public string TableName { get; set; }

        public List<string> ColumnName = new List<string>();

        public List<SqlDbType> ColumnDataType = new List<SqlDbType>();

        public List<int> ColumnDataSize = new List<int>();

        public bool IsValidConnectionString(string connectionString)
        {
            bool isValidConnectionString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    isValidConnectionString = true;
                }
            }
            catch
            {
                isValidConnectionString = false;
            }
            return isValidConnectionString;
        }

        public bool IsValidTableName(string tableName)
        {
            if (tableName == "")
            {
                return true;
            }
            return false;
        }

        public void CreateTable()
        {
            string sql = $"CREATE TABLE {TableName} (Id int PRIMARY KEY IDENTITY(1,1))";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public void AddColumn(string columnName, string dataType)
        {
            columnName = columnName.Replace(" ", "");
            string sql = $"ALTER TABLE {TableName} ADD {columnName} {dataType}";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddColumn(string columnName, string dataType, int dataSize)
        {
            columnName = columnName.Replace(" ", "");
            string sql = $"ALTER TABLE {TableName} ADD {columnName} {dataType}({dataSize})";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddRow(List<string> rowElements)
        {
            string columnName = GetColumnNameString(ColumnName);
            string verbatimValues = RowToVerbatim(ColumnName);


            string sql = $"INSERT INTO {TableName} ({columnName}) VALUES({verbatimValues})";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;

                    int i = 0;
                    foreach (string element in ColumnName)
                    {
                        SqlParameter sqlParameter = new SqlParameter();
                        sqlParameter.ParameterName = '@' + element;
                        sqlParameter.SqlDbType = ColumnDataType[i];
                        sqlParameter.Value = rowElements[i];
                        sqlParameter.Size = ColumnDataSize[i];
                        command.Parameters.Add(sqlParameter);
                        i++;
                    }
                    command.Connection = connection;
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        private string GetColumnNameString(List<string> columnName)
        {
            string columnNameString = "";
            int i = 1;
            foreach (string name in columnName)
            {
                if (i != columnName.Count)
                {
                    columnNameString += name + ", ";
                }
                else
                {
                    columnNameString += name;
                }
                i++;
            }
            return columnNameString;
        }

        private string RowToVerbatim(List<string> rowElements)
        {
            string sqlValues = "";
            string separator = ", ";
            char verbatim = '@';
            int i = 0;
            int rowElementsNumber = rowElements.Count;
            foreach (string element in rowElements)
            {
                if (i == 0 || i == rowElementsNumber)
                {
                    sqlValues += verbatim + element;
                }
                else
                {
                    sqlValues += separator + verbatim + element;
                }
                i++;
            }
            return sqlValues;
        }

        public bool CheckDataType(string dataType)
        {
            string[] acceptedDataTypeCharacters =
            #region DataTypeCharacters
            {
                "bigint",
                "binary",
                "bit",
                "char",
                "date",
                "datetime",
                "datetime2",
                "datetimeoffset",
                "decimal",
                "float",
                "image",
                "int",
                "money",
                "nchar",
                "ntext",
                "nvarchar",
                "real",
                "smalldatetime",
                "smallint",
                "smallmoney",
                "structured",
                "text",
                "time",
                "timestamp",
                "tinyint",
                "uniqueidentifier",
                "varbinary",
                "varchar",
                "variant",
                "xml"
        };
            #endregion
            bool dataTypeCheck = false;

            for (int i = 0; i < acceptedDataTypeCharacters.Length; i++)
            {
                if (dataType == acceptedDataTypeCharacters[i])
                {
                    dataTypeCheck = true;
                    break;
                }
            }
            return dataTypeCheck;
        }

        public SqlDbType StringToSqlDbType(string type)
        {
            switch (type)
            {
                case "bigint": return SqlDbType.BigInt;
                case "binary": return SqlDbType.Binary;
                case "bit": return SqlDbType.Bit;
                case "char": return SqlDbType.Char;
                case "date": return SqlDbType.Date;
                case "datetime": return SqlDbType.DateTime;
                case "datetime2": return SqlDbType.DateTime2;
                case "datetimeoffset": return SqlDbType.DateTimeOffset;
                case "decimal": return SqlDbType.Decimal;
                case "float": return SqlDbType.Float;
                case "image": return SqlDbType.Image;
                case "int": return SqlDbType.Int;
                case "money": return SqlDbType.Money;
                case "nchar": return SqlDbType.NChar;
                case "ntext": return SqlDbType.NText;
                case "nvarchar": return SqlDbType.NVarChar;
                case "real": return SqlDbType.Real;
                case "smalldatetime": return SqlDbType.SmallDateTime;
                case "smallint": return SqlDbType.SmallInt;
                case "smallmoney": return SqlDbType.SmallMoney;
                case "structured": return SqlDbType.Structured;
                case "text": return SqlDbType.Text;
                case "time": return SqlDbType.Time;
                case "timestamp": return SqlDbType.Timestamp;
                case "tinyint": return SqlDbType.TinyInt;
                case "uniqueidentifier": return SqlDbType.UniqueIdentifier;
                case "varbinary": return SqlDbType.VarBinary;
                case "varchar": return SqlDbType.VarChar;
                case "variant": return SqlDbType.Variant;
                case "xml": return SqlDbType.Xml;
                default: return 0;
            }
        }
    }
}
