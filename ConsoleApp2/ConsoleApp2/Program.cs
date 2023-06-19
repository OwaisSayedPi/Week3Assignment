using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/branch.txt";
            int BranchID = 0;
            if (File.Exists(path))
            {
                int.TryParse(File.ReadAllLines(path)[0],out BranchID);
            }

            string Query = "EXEC OS_GET_SERVICES "+BranchID;
            ShowMenu(Query);
            int.TryParse(Console.ReadLine(), out int serviceID);



            Query = "EXEC OS_GET_QUESTIONS " + serviceID;
            string answer = ServiceQuestions(Query);

            Query = "EXEC OS_ANSWERS_SP " + answer + "," + serviceID + "," + BranchID;
            string Token = GetToken(Query);
            Console.WriteLine("Your Token is :"+Token);

            Console.ReadLine();
            Main(args);
        }
        public static string GetToken(string Query)
        {
            Console.Clear();
            string token = "";
            SqlConnection sql;
            string connectionString = @"Data Source=PC-227\SQL2016EXPRESS;Initial Catalog=Northwind;User ID=sagar;Password=aa";

            sql = new SqlConnection(connectionString);
            sql.Open();

            SqlCommand cmd = new SqlCommand(Query, sql);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                token = reader.GetString(0);
            }
            reader.Close();
            sql.Close();
            return token;
        }
        public static string ServiceQuestions(string Query)
        {
            Console.Clear();
            SqlConnection sql;
            string connectionString = @"Data Source=PC-227\SQL2016EXPRESS;Initial Catalog=Northwind;User ID=sagar;Password=aa";

            sql = new SqlConnection(connectionString);
            sql.Open();

            SqlCommand cmd = new SqlCommand(Query, sql);
            SqlDataReader reader = cmd.ExecuteReader();
            QandA QandA = new QandA();
            QandA.listOfQandA = new List<QandA>();
            string answers = "";
            int count = 1;
            while (reader.Read())
            {
                QandA q = new QandA();
                Console.WriteLine(reader["Question"]);
                int qid = reader.GetInt32(0);
                q.Question = reader.GetString(1);
                q.Answer = Console.ReadLine();
                answers += String.Format("'<root><ID>{0}</ID><Answer>{1}</Answer><QuestionID>{2}</QuestionID></root>'", reader.GetInt32(0).ToString().Trim(), q.Answer.Trim(),qid.ToString().Trim());
                //answers += "<root><ID>" + count + "</ID><Answer>" + q.Answer + "</Answer><QuestionID>" + reader.GetInt32(0).ToString() + "</QuestionID></root>";
                QandA.listOfQandA.Add(q);
                count++;
            }            
            reader.Close();
            sql.Close();
            return answers;
        }
        public static void ShowMenu(string Query)
        {
            Console.Clear();
            SqlConnection sql;
            string connectionString = @"Data Source=PC-227\SQL2016EXPRESS;Initial Catalog=Northwind;User ID=sagar;Password=aa";

            sql = new SqlConnection(connectionString);
            sql.Open();

            SqlCommand cmd = new SqlCommand(Query, sql);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
            }
            reader.Close();

            sql.Close();
        }
        public string ToXML(Object oObject)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(oObject.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, oObject);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }
    }
    class QandA
    {
        public List<QandA> listOfQandA { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
