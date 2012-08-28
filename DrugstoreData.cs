using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Data.SqlClient;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;


namespace Drugstore
{
    class DrugstoreData
    {
        private DataTable Table;
        private FbConnection Connection;

        public DrugstoreData()
        { 
            FbConnectionStringBuilder csb = new FbConnectionStringBuilder();

            // Путь до файла с базой данных
            csb.Database = "D:\\data\\DS.FDB";

            // Настройка параметров "общения" клиента с сервером
            csb.Charset = "WIN1251";
            csb.Dialect = 3;


            // Настройки аутентификации
            csb.UserID = "SYSDBA";
            csb.Password = "masterkey";
            Connection = new FbConnection(csb.ToString());   
        }

        public DataTable GetDataSource()
        {
            Connection.Open();

            FbDataAdapter custDA = new FbDataAdapter(@"SELECT GROUP_NAME, PRODUCT_NAME, SALES_VOLUME, NATURAL_VOLUME FROM Groups, Products WHERE Groups.GROUP_ID = Products.GROUP_ID", Connection);
            Table = new DataTable();
            custDA.Fill(Table);

            Connection.Close();

            return Table;
        }

        protected int GetMainSum()
        {
            Connection.Open();
            FbCommand sqlReqest = new FbCommand("SELECT SUM(products.sales_volume) FROM PRODUCTS", Connection);

            // Выполняем запрос
            FbDataReader r = sqlReqest.ExecuteReader();
            r.Read();
            string s = r.GetString(0);
            Connection.Close();

            return Int16.Parse(s);
        }

        public DataTable GetAbcTable(int GroupId)
        {
            string s = @"select p.group_id, g.group_name, SUM(s.sales_volume) SALES_VOLUME, sum(s.natural_volume) NATURAL_VOLUME from products p, sales s, groups g " +
                       "where g.group_id = p.group_id and s.product_id = p.product_id and s.dt = '01.01.2011' " +
                       "GROUP BY p.group_id, g.group_name ORDER BY 3 desc";

            Connection.Open();

            FbDataAdapter custDA = new FbDataAdapter(s, Connection);
            Table = new DataTable();
            custDA.Fill(Table);

            Connection.Close();

            return Table;      
        }

        public List<AbcItem> GetAbcList()
        {
            string s = @"select p.group_id, g.group_name, SUM(s.sales_volume) SALES_VOLUME, sum(s.natural_volume) NATURAL_VOLUME from products p, sales s, groups g " +
                                   "where g.group_id = p.group_id and s.product_id = p.product_id and s.dt = '01.01.2011' " +
                                   "GROUP BY p.group_id, g.group_name ORDER BY 3 desc";

            List<AbcItem> list = new List<AbcItem>();


            Connection.Open();
            FbCommand sqlReqest = new FbCommand(s, Connection);

            double GlobalSumm = 0;
            // Выполняем запрос
            using (FbDataReader r = sqlReqest.ExecuteReader())
            {
                // Читаем результат запроса построчно - строка за строкой
                while (r.Read())
                {
                    // Обращение к данным полей запроса осуществляется по их номеру в
                    // запросе, в данном случае 0 - name, 1 - fio, 2 - tel
                    //string s = r.GetString(0) + " " + r.GetString(1);// +r.GetString(2);
                    AbcItem item = new AbcItem();
                    item.GroupName = r.GetString(1);
                    item.SalesVolume = r.GetDouble(2);//Double.Parse(r.GetString(2));
                    item.NaturalVolume = r.GetDouble(3);//Double.Parse(r.GetString(3));
                    GlobalSumm += item.SalesVolume;
                    list.Add(item);        
                }
            }

            Connection.Close();

            foreach (AbcItem i in list)
            {
                i.Percentage = i.SalesVolume / GlobalSumm * 100;
            }

            double PercentSum = 0;
            int Group = 0;

            foreach (AbcItem i in list)
            {
                PercentSum = PercentSum + i.Percentage;
                switch (Group)
                { 
                    case 0 :                 
                        if (PercentSum < 80)
                        {
                            i.AbcGroup = "A";
                        }
                        else
                        {
                            i.AbcGroup = "B";
                            PercentSum = i.Percentage;
                            Group = 1;
                        }
                        break;
                    case 1 :
                        if (PercentSum < 20)
                        {
                            i.AbcGroup = "B";
                        }
                        else
                        {
                            i.AbcGroup = "C";
                            PercentSum = i.Percentage;
                            Group = 2;
                        }
                        break;

                    case 2 :
                        i.AbcGroup = "C";
                        break;

                }

            }

            return list;
        }

        public void CalcGroupReport()
        {
            int sum = GetMainSum();
            MessageBox.Show(sum.ToString(), "summ",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);
        }
       
        public List<ProductWithGroup> GetUnsetProductList()
        {
            string s = @"SELECT FIRST 50 PRODUCTS.* " +
                        " FROM PRODUCTS LEFT JOIN PRODUCT_GROUP ON PRODUCTS.PRODUCT_NAME = PRODUCT_GROUP.PRODUCT_NAME " +
                        " WHERE PRODUCT_GROUP.GROUP_ID IS NULL";
            
            List<ProductWithGroup> list = new List<ProductWithGroup>();


            Connection.Open();
            FbCommand sqlReqest = new FbCommand(s, Connection);

            // Выполняем запрос
            using (FbDataReader r = sqlReqest.ExecuteReader())
            {
                // Читаем результат запроса построчно - строка за строкой
                while (r.Read())
                {
                    ProductWithGroup item = new ProductWithGroup();
                    item.ProductId = r.GetInt32(0);
                    item.ProductName = r.GetString(1);
                    item.GroupId = -1;
                    
                    list.Add(item);
                }
            }

            Connection.Close();

            return list;
        }
        public void UpdateProductList(List<ProductWithGroup> list)
        {
            Connection.Open();

            FbTransaction Transaction = Connection.BeginTransaction();

            foreach (ProductWithGroup p in list)
            {
                if (p.GroupId < 0)
                {
                    continue;
                }
                
                string s = "INSERT INTO PRODUCT_GROUP (PRODUCT_NAME, GROUP_ID) VALUES ('" + p.ProductName + "', " + p.GroupId.ToString() + ");";
                FbCommand command = new FbCommand(s, Connection, Transaction);


                //здесь нужно одну строчку
                command.ExecuteNonQuery();
            }          
            
            Transaction.Commit();

            Connection.Close();


        
        }
 
        public List<Group> GetGroupList()
        {
            string s = @"SELECT * FROM GROUPS";

            List<Group> list = new List<Group>();
            
            Connection.Open();
            
            FbCommand sqlReqest = new FbCommand(s, Connection);

            // Выполняем запрос
            using (FbDataReader r = sqlReqest.ExecuteReader())
            {
                // Читаем результат запроса построчно - строка за строкой
                while (r.Read())
                {
                    Group item = new Group();
                    item.GroupId = r.GetInt16(0);
                    item.GroupName = r.GetString(1);

                    list.Add( item );
                }
            }

            Connection.Close();

            return list;  
        }
    }
}
