using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.SQLite
{
    public class Table
    {        
        public Column Columns { set; get; }
        public string TableName { set; get; }

        public Table()
        {
            Columns = new Column();
        }

        public Table(string name)
        {
            TableName = name;
            Columns = new Column();
        }
        
    }
}