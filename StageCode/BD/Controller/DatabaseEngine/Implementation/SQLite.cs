using CodeExceptionManager.Model.Objects;
using CodeExceptionManager.Model.Objects.QueryParameters;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeExceptionManager.Controller.DatabaseEngine.Implementation
{
    public class SQLite : IDatabaseEngine
    {
        private SQLiteConnection _dbConnection;
        private SQLiteCommand _sqlCommandExecuter;
        private const string DATABASE_NAME = "ErrorLogs";
        private const string DATABASE_FOLDER = "Datas\\Databases";
        private const string dbPw = "Pr0cess.23.Ortho";

        public SQLite()
        {
            ConnexionStatus = false;
        }

        public bool ConnexionStatus { get; set; }

        public void Connect()
        {
            try
            {
                //FDB : 11/04/2023 OS-155
                //DirectoryInfo parentDir = Directory.GetParent(AppContext.BaseDirectory).Parent;
                DirectoryInfo parentDir = Directory.GetParent(AppContext.BaseDirectory);
                if (!parentDir.FullName.Contains("Core"))
                {
                    parentDir = parentDir.Parent;
                }
                if (!Directory.Exists(parentDir + "\\" + DATABASE_FOLDER))
                {
                    Directory.CreateDirectory(parentDir.FullName + "\\" + DATABASE_FOLDER);
                }
                _dbConnection = new SQLiteConnection("Data Source=" + parentDir.FullName + "\\" + DATABASE_FOLDER + "\\" + DATABASE_NAME + ".db;Version=3;");
                _dbConnection.SetPassword(dbPw);
                _dbConnection.Open();
                _sqlCommandExecuter = new SQLiteCommand(
                    @"CREATE TABLE IF NOT EXISTS Exceptions (
                        Id              INTEGER       PRIMARY KEY AUTOINCREMENT,
                        Date            DATETIME      DEFAULT [],
                        AssemblyName    VARCHAR(255)  DEFAULT [],
                        AssemblyVersion VARCHAR(50)   DEFAULT[0.0.0.0],
                        ClassName       VARCHAR(255)  DEFAULT [],
                        MethodName      VARCHAR(255)  DEFAULT [],
                        ErrorMessage    VARCHAR(255)  DEFAULT[No Info],
                        ErrorStackTrace TEXT          DEFAULT[No StackTrace]
                    );"
                    , _dbConnection);
                _sqlCommandExecuter.ExecuteNonQuery();
            }
            catch
            {
                ConnexionStatus = false;
            }
            ConnexionStatus = true;
        }

        public void Disconnect()
        {
            try
            {
                _dbConnection.Close();
                ConnexionStatus = true;
            }
            catch
            {
            }
        }

        public bool ExecuteSqlQueryWithoutReturnedData(string queryString)
        {
            try
            {
                _sqlCommandExecuter = new SQLiteCommand(queryString, _dbConnection);
                _sqlCommandExecuter.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public List<object> ExecuteSqlQueryWithReturnedData(string queryString)
        {
            try
            {
                List<Object> datas = new List<object>();
                _sqlCommandExecuter = new SQLiteCommand(queryString, _dbConnection);
                SQLiteDataReader reader = _sqlCommandExecuter.ExecuteReader();
                dynamic temporaryObject;
                while (reader.Read())
                {
                    temporaryObject = new ExpandoObject();
                    for (int count = 0; count < reader.FieldCount; count++)
                    {
                        AddPropertyToDynamicObject(temporaryObject, reader.GetName(count), reader.GetValue(count));
                    }
                    datas.Add(temporaryObject);
                }
                return datas;
            }
            catch
            {
                return new List<object>() { -1 };
            }
        }

        /// <summary>
        /// we use this method to add a new property dynamiquely to the dynamic object, because it is normally used as myObjectDynamic.MyNewProperty, but we doesn't know the name of the column before
        /// we start this method. But the properties of the Dynamic object is a Dictionnary, so we can, using a method add manually the new parameter in the way that we want. 
        /// Code found there : http://ravindranaik.com/build-c-objects-dynamically/
        /// </summary>
        private void AddPropertyToDynamicObject(ExpandoObject dynamicObject, string propertyName, object value)
        {
            IDictionary<string, Object> dynamicObjectProperties = dynamicObject as IDictionary<string, Object>;

            if (dynamicObjectProperties.ContainsKey(propertyName))
            {
                dynamicObjectProperties[propertyName] = value;
            }
            else
            {
                dynamicObjectProperties.Add(propertyName, value);
            }
        }

        public string ParseQuery(QueryObject query)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                switch (query.RequestType)
                {
                    case Model.Enum.RequestTypeEnum.create:
                        string columns = "", values = "";
                        stringBuilder.Append("INSERT INTO " + query.TableName + " ");
                        if (query.Parameters.ContainsKey(Model.Enum.QueryParametersEnum.insertValues))
                        {
                            foreach (InsertValuesQueryParameters parameter in query.Parameters[Model.Enum.QueryParametersEnum.insertValues])
                            {
                                if (columns != "")
                                {
                                    columns += ",";
                                }
                                columns += parameter.ColumnName;
                                if (values != "")
                                {
                                    values += ",";
                                }
                                if (parameter.ColumnValue.GetType() == typeof(bool))
                                {
                                    if ((bool)parameter.ColumnValue == true)
                                    {
                                        values += "'1'";
                                    }
                                    else
                                    {
                                        values += "'0'";
                                    }
                                }
                                else if (parameter.ColumnValue.GetType() == typeof(DateTime))
                                {
                                    values += "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                                }
                                else
                                {
                                    values += "'" + parameter.ColumnValue.ToString().Replace("'","''") + "'";
                                }
                            }
                            stringBuilder.Append("(" + columns + ") VALUES (" + values + ");");
                        }
                        break;
                }
                return stringBuilder.ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
