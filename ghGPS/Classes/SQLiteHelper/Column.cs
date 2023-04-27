using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.SQLite
{
    /// <summary>
    /// Class to access the column properties
    /// </summary>
    public class ColumnProperties
    {
        public string ColumnName { get; set; } = "";
        public bool PrimaryKey { get; set; } = false;
        public ColType ColDataType { get; set; } = ColType.Text;
        public bool AutoIncrement { get; set; } = false;
        public bool NotNull { get; set; } = false;
        public string DefaultValue { get; set; } = "";
    }

    public class Column : IEnumerable<ColumnProperties>
    {
        private List<ColumnProperties> cols = new List<ColumnProperties>();

        public Column()
        {
            cols = new List<ColumnProperties>();
        }

        public IEnumerator<ColumnProperties> GetEnumerator()
        {
            return this.cols.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string colName)
        {
            ColumnProperties ml = new ColumnProperties();
            ml.ColumnName = colName;
            ml.PrimaryKey = false;
            ml.ColDataType = ColType.Text;
            ml.AutoIncrement = false;

            cols.Add(ml);
        }

        public void Add(string colName, ColType colDataType)
        {
            ColumnProperties ml = new ColumnProperties();

            ml.ColumnName = colName;
            ml.PrimaryKey = false;
            ml.ColDataType = colDataType;
            ml.AutoIncrement = false;

            cols.Add(ml);
        }

        public void Add(string colName, bool autoIncrement)
        {
            ColumnProperties ml = new ColumnProperties();
            ml.ColumnName = colName;

            if (autoIncrement)
            {
                ml.PrimaryKey = true;
                ml.ColDataType = ColType.Integer;
                ml.AutoIncrement = true;
            }
            else
            {
                ml.PrimaryKey = false;
                ml.ColDataType = ColType.Text;
                ml.AutoIncrement = false;
            }

            cols.Add(ml);
        }

        public void Add(string colName, ColType colDataType, bool notNull)
        {
            ColumnProperties ml = new ColumnProperties();

            ml.ColumnName = colName;            
            ml.ColDataType = colDataType;            
            ml.NotNull = notNull;        

            cols.Add(ml);
        }

        public void Add(string colName, ColType colDataType, bool primaryKey, bool autoIncrement, bool notNull)
        {
            ColumnProperties ml = new ColumnProperties();

            ml.ColumnName = colName;

            if (autoIncrement)
            {
                ml.PrimaryKey = true;
                ml.ColDataType = ColType.Integer;
                ml.AutoIncrement = true;
            }
            else
            {
                ml.PrimaryKey = primaryKey;
                ml.ColDataType = colDataType;
                ml.AutoIncrement = false;
                ml.NotNull = notNull;                
            }

            cols.Add(ml);
        }

        public void Add(string colName, ColType colDataType, bool primaryKey, bool autoIncrement, bool notNull, string defaultValue)
        {
            ColumnProperties ml = new ColumnProperties();

            ml.ColumnName = colName;

            if (autoIncrement)
            {
                ml.PrimaryKey = true;
                ml.ColDataType = ColType.Integer;
                ml.AutoIncrement = true;
            }
            else
            {
                ml.PrimaryKey = primaryKey;
                ml.ColDataType = colDataType;
                ml.AutoIncrement = false;
                ml.NotNull = notNull;
                ml.DefaultValue = defaultValue;
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
                if (cols[i].ColumnName == colName)
                    throw new Exception("Column name of \"" + colName + "\" is already existed.");
            }
        }

        public int IndexOf(ColumnProperties item)
        {
            return cols.IndexOf(item);
        }

        public void Insert(int index, ColumnProperties item)
        {
            CheckColumnName(item.ColumnName);

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
                if (cols[index].ColumnName != value.ColumnName)
                {
                    CheckColumnName(value.ColumnName);
                }

                cols[index] = value;
            }
        }

        public void Add(ColumnProperties item)
        {
            CheckColumnName(item.ColumnName);

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
}
