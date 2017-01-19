using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using CSharpSample.DesignPattern.Factory;
using CSharpSample.SampleCode.Entity;
using CVL.Extentions;
using Dapper;

namespace CSharpSample.SampleCode
{
    /// <summary>
    /// DataBaseアクセスクラス
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "For Japanese support")]
    public class DatabaseAccess : ISamplePractitioner
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DatabaseAccess()
        {
            ConnectionString = GetConnectionString();
            SQLServerConnection.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// 接続文字列
        /// </summary>
        private string ConnectionString { get; }

        /// <summary>
        /// 接続文字列設定項目
        /// 接続文字列を完成させるには、SampleDBSettins.txtの内容を読み込み変更する必要がある。
        /// </summary>
        private Dictionary<string, string> SampleDBKeyStrings { get; set; } = new Dictionary<string, string>
        {
            { "ServerName", string.Empty },
            { "DBName", string.Empty },
            { "UserName", string.Empty },
            { "Password", string.Empty },
        };

        /// <summary>
        /// SQL Server Connection
        /// </summary>
        private SqlConnection SQLServerConnection { get; } = new SqlConnection();

        /// <summary>
        /// Sampleの実行
        /// </summary>
        /// <param name="exampleNo">例番号</param>
        /// <returns>実行結果</returns>
        public bool Do(int exampleNo)
        {
            switch (exampleNo)
            {
                case 1:
                    AccessSQLServer();
                    break;
                case 2:
                    FirstLinq2SQL();
                    break;
                case 3:
                    FirstDapper();
                    break;
                default:
                    break;
            }

            return true;
        }

        /// <summary>
        /// SQL Serverへのアクセス
        /// </summary>
        public void AccessSQLServer()
        {
            SQLServerConnection.Open();

            // SELECT文を設定・実行します。
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM HouseholdAccount";
            command.Connection = SQLServerConnection;
            SqlDataReader reader = command.ExecuteReader();

            // 結果を表示します。
            while (reader.Read())
            {
                var id = (int)reader.GetValue(0);
                var date = (DateTime)reader.GetValue(1);
                var item = (string)reader.GetValue(2);

                Console.WriteLine($"Id={id} Item={item} Date={date}");
            }

            SQLServerConnection.Close();
        }

        /// <summary>
        /// Linq to SQL のサンプルコード
        /// </summary>
        public void FirstLinq2SQL()
        {
            DataContext dc = new DataContext(ConnectionString);
            var householdAccount = dc.GetTable<HouseholdAccount>();

            var query = from p in householdAccount
                        where p.Id <= 20 
                        select new { p.Id, p.Item, p.Date };

            Console.WriteLine("-->> Generated Command Start <<--");
            Console.WriteLine(dc.GetCommand(query).CommandText);
            Console.WriteLine("-->> Generated Command End <<--");

            query.ForEach(x =>
            {
                Console.WriteLine($"Id={x.Id} Item={x.Item} Date={x.Date}");
            });
        }

        /// <summary>
        /// Dapperのサンプルコード
        /// </summary>
        public void FirstDapper()
        {
            SQLServerConnection.Open();

            var result = SQLServerConnection.Query<HouseholdAccount>(@"SELECT * FROM HOUSEHOLDACCOUNT");
            result.ForEach(x =>
            {
                Console.WriteLine($"Id={x.Id} Item={x.Item} Date={x.Date}");
            });

            SQLServerConnection.Close();
        }

        /// <summary>
        /// SQL Serverへの接続文字列を取得する
        /// </summary>
        /// <returns>接続文字列</returns>
        private string GetConnectionString()
        {
            // 接続文字列取得（パスワード等を記載することになるので、文字列を置換する）
            var connectionString = ConfigurationManager.ConnectionStrings["SampleDB"].ConnectionString;

            ReadDBKeyStrings();

            // app.configから読み込んだ接続文字列を置換する
            connectionString = connectionString.Replace("$(ServerName)", SampleDBKeyStrings["ServerName"]);
            connectionString = connectionString.Replace("$(DBName)", SampleDBKeyStrings["DBName"]);
            connectionString = connectionString.Replace("$(UserName)", SampleDBKeyStrings["UserName"]);
            connectionString = connectionString.Replace("$(Password)", SampleDBKeyStrings["Password"]);

            return connectionString;
        }

        /// <summary>
        /// DB接続文字列のキーワードを読み込む
        /// </summary>
        private void ReadDBKeyStrings()
        {
            using (StreamReader sr = new StreamReader("SampleDBSettings.txt", Encoding.GetEncoding("Shift_JIS")))
            {
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    var key = string.Empty;
                    var value = string.Empty;

                    SampleDBKeyStrings.ForEach(x =>
                    {
                        if (line.IndexOf(x.Key) >= 0)
                        {
                            key = line.Split('=')[0];
                            value = line.Split('=')[1];
                            return;
                        }
                    });

                    if (key != string.Empty)
                    {
                        SampleDBKeyStrings[key] = value;
                    }
                }
            }
        }
    }
}