using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Windows.Forms;

namespace ghGPS.Classes
{

    public class EntryList
    {
        public EntryList()
        {
            ColumnName = new List<string>();
            DbType = new List<DbType>();
            Content = new List<object>();
        }

        public List<string> ColumnName { set; get; }

        public List<DbType> DbType { set; get; }

        public List<object> Content { set; get; }

        public void Add(string ColumnName, DbType DbType, object Content)
        {
            this.ColumnName.Add(ColumnName);
            this.Content.Add(Content);
            this.DbType.Add(DbType);
        }
    }

    public class ListWithName
    {
        public ListWithName()
        {
            SubItems = new List<object>();
        }

        public string Text { set; get; }

        public List<object> SubItems { set; get; }
    }

    /// <summary>
    /// Class to access the column properties
    /// </summary>
    public class ColumnProperties
    {
        public string ID { get; set; }
        public string Name { get; set; } = "";
        public ColType DataType { get; set; } = ColType.Text;
        public string DefaultValue { get; set; } = "";
        public bool AllowNull { get; set; } = true;
        public bool AutoIncrement { get; set; } = false;
        public bool PrimaryKey { get; set; } = false;
    }

    public enum ColType
    {
        Text,
        Datetime,
        Integer,
        Decimal,
        Varchar,
        Blob
    }

    /// <summary>
    /// Column Properties and functions
    /// </summary>
    public class Column : IEnumerable<ColumnProperties>
    {
        public Column()
        {
            cols = new List<ColumnProperties>();
        }
        private List<ColumnProperties> cols = new List<ColumnProperties>();

        public IEnumerator<ColumnProperties> GetEnumerator()
        {
            return this.cols.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add Column and parse name only with default params
        /// </summary>
        /// <param name="Name"></param>
        public void Add(string Name)
        {
            ColumnProperties ml = new ColumnProperties();
            ml.Name = Name;
            ml.DataType = ColType.Varchar;
            ml.AllowNull = true;
            ml.DefaultValue = " ";
            cols.Add(ml);
        }
        

        /// <summary>
        /// Add column or field and parse Name and Data type. Nullable is true
        /// </summary>
        /// <param name="Name">Column or field name</param>
        /// <param name="DataType">Data type</param>
        public void Add(string Name, ColType DataType)
        {
            ColumnProperties ml = new ColumnProperties();
            ml.Name = Name;
            ml.DataType = DataType;
            ml.AllowNull = true;
            ml.DefaultValue = " ";
            cols.Add(ml);
        }

        /// <summary>
        /// Add column or field and parse Name, Data type and if nullable
        /// </summary>
        /// <param name="Name">Column or field name</param>
        /// <param name="DataType">Data type</param>
        /// <param name="AllowNulls">Is nullable</param>
        public void Add(string Name, ColType DataType, bool AllowNulls)
        {
            ColumnProperties ml = new ColumnProperties();
            ml.Name = Name;
            ml.DataType = DataType;
            ml.AllowNull = AllowNulls;
            ml.AutoIncrement = false;
            ml.DefaultValue = " ";
            cols.Add(ml);
        }

        /// <summary>
        /// Add a column with all params
        /// </summary>
        /// <param name="Name">Name of the column or field</param>
        /// <param name="DataType">Data type</param>
        /// <param name="AllowNulls">Is nullable</param>
        /// <param name="ID">Column or field ID</param>
        public void Add(string Name, ColType DataType, bool AllowNulls, string ID)
        {
            ColumnProperties ml = new ColumnProperties();
            ml.Name = Name;            
            ml.DataType = DataType;
            ml.ID = ID;
            ml.AllowNull = AllowNulls;
            ml.DefaultValue = " ";
            cols.Add(ml);
        }

        public void Add(string Name, ColType DataType, bool AllowNulls, bool AutoIncrement, bool PrimaryKey)
        {
            ColumnProperties ml = new ColumnProperties();
            ml.Name = Name;
            if (autoIncrement)
            {
                ml.PrimaryKey = true;
                ml.ColDataType = ColType.Integer;
                ml.AutoIncrement = true;
            }
            else
            {
                ml.PrimaryKey = PrimaryKey;
                ml.ColDataType = DataType;
                ml.AutoIncrement = false;
                ml.AllowNull = AllowNulls;
            }
            cols.Add(ml);
        }
        
        public void Add(string Name, ColType DataType, bool PrimaryKey, bool AutoIncrement, bool AllowNulls, string DefaultValue)
        {
            ColumnProperties ml = new ColumnProperties();

            ml.Name = Name;

            if (AutoIncrement)
            {
                ml.PrimaryKey = true;
                ml.ColDataType = ColType.Integer;
                ml.AutoIncrement = true;
            }
            else
            {
                ml.PrimaryKey = PrimaryKey;
                ml.ColDataType = colDataType;
                ml.AutoIncrement = false;
                ml.AllowNull = AllowNulls;
                ml.DefaultValue = DefaultValue;
            }

            cols.Add(ml);
        }

        /// <summary>
        /// Count number of columns 
        /// </summary>
        public int Count
        {
            get { return cols.Count; }
        }

        private void CheckColumnName(string colName)
        {
            for (int i = 0; i < cols.Count; i++)
            {
                if (cols[i].Name == colName)
                    throw new Exception("Column name of \"" + colName + "\" is already existed.");
            }
        }

        public int IndexOf(ColumnProperties item)
        {
            return cols.IndexOf(item);
        }

        public void Insert(int index, ColumnProperties item)
        {
            CheckColumnName(item.Name);

            cols.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            cols.RemoveAt(index);
        }

        public ColumnProperties this[int index]
        {
            get
            {
                return cols[index];
            }
            set
            {
                if (cols[index].Name != value.Name)
                {
                    CheckColumnName(value.Name);
                }

                cols[index] = value;
            }
        }

        public void Add(ColumnProperties item)
        {
            CheckColumnName(item.Name);

            cols.Add(item);
        }

        public void Clear()
        {
            cols.Clear();
        }

        public bool Contains(ColumnProperties item)
        {
            return cols.Contains(item);
        }

        public void CopyTo(ColumnProperties[] array, int arrayIndex)
        {
            cols.CopyTo(array, arrayIndex);
        }


        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ColumnProperties item)
        {
            return cols.Remove(item);
        }

    }

    /// <summary>
    /// Create new table
    /// </summary>
    public class Table
    {
        public Table()
        {
            Columns = new Column();
        }

        public string Name { set; get; }

        public Column Columns { set; get; }

    }

    public class SQLiteHelper
    {
        SQLiteDataReader DataReader;

        public string DatabaseFile { set; get; }

        public string Password { set; get; }


        SQLiteConnection DataBaseConnnection = new SQLiteConnection();

        private void SetConnection()
        {

            if (DataBaseConnnection.State == System.Data.ConnectionState.Open)
            {
                DataBaseConnnection.Close();
            }

            if (Password != null)
            {
                DataBaseConnnection.ConnectionString = @"Data Source=" + DatabaseFile + "; Password=" + Password + ";";
                DataBaseConnnection.Open();
            }
            if (Password == null)
            {
                DataBaseConnnection.ConnectionString = @"Data Source=" + DatabaseFile + ";";
                DataBaseConnnection.Open();
            }

        }

        public void CreateDatabase()
        {
            CreateDatabase(DatabaseFile, Password);
        }

        public void CreateDatabase(string DatabaseFile, string Password = null)
        {
            this.DatabaseFile = DatabaseFile;

            SQLiteConnection.CreateFile(DatabaseFile);

            if (Password != null)
            {
                this.Password = Password;
                DataBaseConnnection.SetPassword(Password);
            }


            //SetConnection();

        }

        public void CreateTable(Table table)
        {
            SetConnection();

            StringBuilder sb = new StringBuilder();
            sb.Append("create table if not exists `");
            sb.Append(table.Name);
            sb.AppendLine("`(");

            bool firstRecord = true;

            foreach (var col in table.Columns)
            {
                if (col.Name.Trim().Length == 0)
                {
                    throw new Exception("Column name cannot be blank.");
                }

                if (firstRecord)
                    firstRecord = false;
                else
                    sb.AppendLine(",");

                sb.Append(col.Name);
                sb.Append(" ");

                if (col.AutoIncrement)
                {

                    sb.Append("integer primary key autoincrement");
                    continue;
                }

                switch (col.DataType)
                {
                    case ColType.Text:
                        sb.Append("text"); break;
                    case ColType.Integer:
                        sb.Append("integer"); break;
                    case ColType.Decimal:
                        sb.Append("decimal"); break;
                    case ColType.Datetime:
                        sb.Append("datetime"); break;
                    case ColType.Blob:
                        sb.Append("blob"); break;
                    case ColType.Varchar:
                        sb.Append("varchar"); break;
                }

                if (col.PrimaryKey)
                    sb.Append(" primary key");
                else if (col.AllowNull)
                    sb.Append(" not null");
                else if (col.DefaultValue.Length > 0)
                {
                    sb.Append(" default ");

                    if (col.DefaultValue.Contains(" ") || col.DataType == ColType.Text || col.DataType == ColType.Datetime)
                    {
                        sb.Append("'");
                        sb.Append(col.DefaultValue);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(col.DefaultValue);
                    }
                }
            }

            sb.AppendLine(");");            
            
            SQLiteCommand sqliteCommand = new SQLiteCommand(sb.ToString(), DataBaseConnnection);
            sqliteCommand.ExecuteNonQuery();
        }

        //public void CreateTable(Table Table)
        //{
        //    try
        //    {
        //        SetConnection();
        //        string firstLine = "CREATE TABLE IF NOT EXISTS [" + Table.Name + "] ([ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ";

        //        StringBuilder queryBuilder = new StringBuilder();
        //        queryBuilder.Append(firstLine);

        //        foreach (var item in Table.Columns)
        //        {
        //            string nl = "";
        //            if (item.AllowNull) nl = "NULL";
        //            else nl = "NOT NULL";

        //            queryBuilder.Append("[" + item.Name + "] " + item.DataType + " " + nl + ", ");
        //        }

        //        queryBuilder.Remove(queryBuilder.Length - 2, 2);
        //        queryBuilder.Append(")");
        //        SQLiteCommand sqliteCommand = new SQLiteCommand(queryBuilder.ToString(), DataBaseConnnection);
        //        sqliteCommand.ExecuteNonQuery();

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

        //    }

        //}

        public void CreateTable(string TableName, string[] ColumnNames, bool[] AllowNulls, DbType[] DbTypes)
        {
            TableName = RemoveSpecialCharacters(TableName);
            try
            {
                SetConnection();
                string firstLine = "CREATE TABLE IF NOT EXISTS [" + TableName + "] ([ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ";
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append(firstLine);
                for (int i = 0; i < ColumnNames.Length; i++)
                {
                    string nl = "";
                    if (AllowNulls[i]) nl = "NULL";
                    else nl = "NOT NULL";

                    queryBuilder.Append("[" + ColumnNames[i] + "] " + DbTypes[i] + " " + nl + ", ");

                }
                queryBuilder.Remove(queryBuilder.Length - 2, 2);
                queryBuilder.Append(")");
                SQLiteCommand sqliteCommand = new SQLiteCommand(queryBuilder.ToString(), DataBaseConnnection);
                sqliteCommand.ExecuteNonQuery();
                DataBaseConnnection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

            }

        }

        public void DeleteTable(string TableName)
        {
            try
            {
                SetConnection();
                SQLiteCommand sqliteCommand = new SQLiteCommand("DROP TABLE IF EXISTS " + TableName, DataBaseConnnection);
                sqliteCommand.ExecuteNonQuery();
                SQLiteCommand VacuumCommand = new SQLiteCommand("vacuum;", DataBaseConnnection);
                VacuumCommand.ExecuteNonQuery();
                DataBaseConnnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                //frmPassword fp = new frmPassword();
                //if (fp.ShowDialog() == DialogResult.OK)
                //{
                //    Password = fp.txtPass.Text;
                //    DeleteTable(TableName);
                //}
                //Password = null;
                DeleteTable(TableName);
            }

        }

        public List<string> GetTableNames()
        {
            List<string> tables = new List<string>();
            try
            {
                SetConnection();
                SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT NAME FROM sqlite_master WHERE TYPE='table' ORDER BY NAME", DataBaseConnnection);
                DataReader = sqliteCommand.ExecuteReader();
                while (DataReader.Read())
                {
                    if (DataReader.HasRows)
                    {
                        tables.Add(DataReader[0].ToString());
                    }
                }
                DataBaseConnnection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            return tables;
        }

        public Column GetColumnsFromTableName(string TableName)
        {
            TableName = RemoveSpecialCharacters(TableName);
            Column cols = new Column();
            try
            {
                SetConnection();
                SQLiteCommand sqliteCommand = new SQLiteCommand("PRAGMA table_info('" + TableName + "');", DataBaseConnnection);
                DataReader = sqliteCommand.ExecuteReader();

                while (DataReader.Read())
                {
                    ColumnProperties ml = new ColumnProperties();
                    ml.ID = DataReader[0].ToString();
                    ml.Name = DataReader[1].ToString();                    
                    ml.DataType = (ColType)Enum.Parse(typeof(ColType),DataReader[2].ToString().CapitalizeFirst());
                    bool nl = false;
                    if (DataReader[3].ToString() == "0") nl = true;
                    if (DataReader[3].ToString() == "1") nl = false;
                    ml.AllowNull = nl;
                    cols.Add(ml.Name, ml.DataType, nl, ml.ID);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            return cols;

        }

        public void CreateEntry(string TableName, EntryList EntryList)
        {
            TableName = RemoveSpecialCharacters(TableName);
            try
            {
                SetConnection();
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append("insert into " + TableName + " (");
                foreach (var item in EntryList.ColumnName)
                {
                    queryBuilder.Append(item + ", ");
                }
                queryBuilder.Remove(queryBuilder.Length - 2, 2);
                queryBuilder.Append(")");
                queryBuilder.Append(" values (");
                foreach (var item in EntryList.ColumnName)
                {
                    queryBuilder.Append("@" + item + ", ");
                }
                queryBuilder.Remove(queryBuilder.Length - 2, 2);
                queryBuilder.Append(")");
                SQLiteCommand sqliteCommand = new SQLiteCommand(queryBuilder.ToString(), DataBaseConnnection);

                for (int i = 0; i < EntryList.ColumnName.Count; i++)
                {
                    sqliteCommand.Parameters.Add("@" + EntryList.ColumnName[i], EntryList.DbType[i]).Value = EntryList.Content[i];
                }
                sqliteCommand.ExecuteNonQuery();
                DataBaseConnnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        public void CreateEntry(string TableName, object[] Content)
        {
            TableName = RemoveSpecialCharacters(TableName);
            try
            {
                SetConnection();
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append("insert into " + TableName + " (");
                foreach (var item in GetColumnsFromTableName(TableName))
                {
                    queryBuilder.Append(item.Name + ", ");
                }
                queryBuilder.Remove(queryBuilder.Length - 2, 2);
                queryBuilder.Append(")");
                queryBuilder.Append(" values (");
                foreach (var item in GetColumnsFromTableName(TableName))
                {
                    queryBuilder.Append("@" + item.Name + ", ");
                }
                queryBuilder.Remove(queryBuilder.Length - 2, 2);
                queryBuilder.Append(")");
                SQLiteCommand sqliteCommand = new SQLiteCommand(queryBuilder.ToString(), DataBaseConnnection);
                List<string> colsNames = new List<string>();
                foreach (var item in GetColumnsFromTableName(TableName))
                {
                    colsNames.Add(item.Name);
                }

                for (int i = 0; i < GetColumnsFromTableName(TableName).Count; i++)
                {
                    sqliteCommand.Parameters.Add("@" + colsNames[i], DbType.Object).Value = Content[i];
                }
                sqliteCommand.ExecuteNonQuery();
                DataBaseConnnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        public void DeleteEntry(string TableName, string ColumnName, string Equals)
        {
            TableName = RemoveSpecialCharacters(TableName);
            ColumnName = RemoveSpecialCharacters(ColumnName);
            //Equals = RemoveSpecialCharacters(Equals);
            try
            {
                SetConnection();
                SQLiteCommand sqliteCommand = new SQLiteCommand("DELETE FROM " + TableName + " WHERE " + ColumnName + "=@" + Equals, DataBaseConnnection);
                sqliteCommand.Parameters.Add("@" + Equals, DbType.String).Value = Equals;
                sqliteCommand.ExecuteNonQuery();
                SQLiteCommand VacuumCommand = new SQLiteCommand("vacuum;", DataBaseConnnection);
                VacuumCommand.ExecuteNonQuery();
                DataBaseConnnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        public void RunQuery(string Sql)
        {           
            try
            {
                SetConnection();
                SQLiteCommand sqliteCommand = new SQLiteCommand(Sql, DataBaseConnnection);                
                sqliteCommand.ExecuteNonQuery();
                SQLiteCommand VacuumCommand = new SQLiteCommand("vacuum;", DataBaseConnnection);
                VacuumCommand.ExecuteNonQuery();
                DataBaseConnnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        public void UpdateEntry(string TableName, EntryList EntryList, string ColumnName, string Equals)
        {
            TableName = RemoveSpecialCharacters(TableName);
            ColumnName = RemoveSpecialCharacters(ColumnName);
            Equals = RemoveSpecialCharacters(Equals);

            try
            {
                SetConnection();
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append("update " + TableName + " set ");
                foreach (var item in EntryList.ColumnName)
                {
                    queryBuilder.Append(item + "=@" + item + ", ");
                }
                queryBuilder.Remove(queryBuilder.Length - 2, 2);
                queryBuilder.Append(" ");
                queryBuilder.Append(" WHERE " + ColumnName + "='" + Equals + "'");
                SQLiteCommand sqliteCommand = new SQLiteCommand(queryBuilder.ToString(), DataBaseConnnection);

                for (int i = 0; i < EntryList.ColumnName.Count; i++)
                {
                    sqliteCommand.Parameters.Add("@" + EntryList.ColumnName[i], EntryList.DbType[i]).Value = EntryList.Content[i];
                }
                sqliteCommand.ExecuteNonQuery();
                DataBaseConnnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

        }

        public List<ListWithName> GetEntries(string TableName)
        {
            TableName = RemoveSpecialCharacters(TableName);

            List<ListWithName> listLvi = new List<ListWithName>();
            try
            {

                SetConnection();

                SQLiteCommand sqliteCommand = new SQLiteCommand("select * from " + TableName, DataBaseConnnection);

                DataReader = sqliteCommand.ExecuteReader();

                while (DataReader.Read())
                {
                    ListWithName lwn = new ListWithName();
                    lwn.Text = DataReader[0].ToString();
                    for (int i = 1; i < DataReader.FieldCount; i++)
                    {
                        lwn.SubItems.Add(DataReader[i]);
                    }
                    listLvi.Add(lwn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            return listLvi;
        }

        public List<ListWithName> SearchDatabase(string ColumnName, string SearchKeyWord)
        {
            ColumnName = RemoveSpecialCharacters(ColumnName);
            SearchKeyWord = RemoveSpecialCharacters(SearchKeyWord);
            List<ListWithName> listLwn = new List<ListWithName>();
            try
            {
                foreach (var table in GetTableNames())
                {
                    if (table != "sqlite_sequence")
                    {
                        SetConnection();
                        SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT * FROM " + table + " WHERE " + ColumnName + " LIKE @searchKey", DataBaseConnnection);
                        sqliteCommand.Parameters.Add("@searchKey", DbType.String).Value = "%" + SearchKeyWord + "%";
                        DataReader = sqliteCommand.ExecuteReader();
                        if (DataReader.HasRows)
                        {
                            while (DataReader.Read())
                            {
                                ListWithName lwn = new ListWithName();
                                lwn.Text = DataReader[0].ToString();
                                for (int i = 1; i < DataReader.FieldCount; i++)
                                {
                                    lwn.SubItems.Add(DataReader[i].ToString());
                                }
                                listLwn.Add(lwn);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            return listLwn;
        }

        public List<ListWithName> SearchDatabase(string SearchKeyWord)
        {
            SearchKeyWord = RemoveSpecialCharacters(SearchKeyWord);
            List<ListWithName> listLwn = new List<ListWithName>();
            try
            {
                foreach (var table in GetTableNames())
                {
                    foreach (var col in GetColumnsFromTableName(table))
                    {
                        if (table != "sqlite_sequence")
                        {
                            SetConnection();
                            SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT * FROM " + table + " WHERE " + col.Name + " LIKE @searchKey", DataBaseConnnection);
                            sqliteCommand.Parameters.Add("@searchKey", DbType.String).Value = "%" + SearchKeyWord + "%"; ;
                            DataReader = sqliteCommand.ExecuteReader();
                            if (DataReader.HasRows)
                            {
                                while (DataReader.Read())
                                {
                                    ListWithName lwn = new ListWithName();
                                    lwn.Text = DataReader[0].ToString();
                                    for (int i = 1; i < DataReader.FieldCount; i++)
                                    {
                                        lwn.SubItems.Add(DataReader[i].ToString());
                                    }
                                    listLwn.Add(lwn);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            return listLwn;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public string Escape(string data)
        {
            data = data.Replace("'", "''");
            data = data.Replace("\\", "\\\\");
            return data;
        }

    }
}
