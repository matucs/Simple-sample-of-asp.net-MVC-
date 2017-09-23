using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MabnaInterviewTest.Controllers
{
    public class EMailController : Controller
    {
        const string logFile = "log.txt";

        // این یک متد است که شناسه یا نام کاربری یکی کاربر را گرفته و
        // بعد از جستجو در دیتابیس یک ایمیل برای او ارسال می‌کند.
        // سپس این عملیات در یک فایل لاگ ثبت می‌شود.

        public ActionResult Index( )
        {
            
            return View();
        }
        public ActionResult SendMail(int? id, string username = null)
        {
            //url : http://localhost:57129/Email/Index؟username=Ali&id=1
            // username or id could be null
            //this is sample username=Ali id =1

            if (Request.QueryString.Keys.Count == 0)
                return HttpNotFound();

            DB db = new DB();
            db.Connect();

            SqlDataReader reader = db.SelectUser(username /*Request.QueryString["username"]*/, id);

            reader.Read();

            string email = reader.GetString(2);

            emailer.Instance.send(email);
            FileStream fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + logFile, FileMode.Append, FileAccess.Write);
            CustomStreamWriter customstreamwriter = new CustomStreamWriter(fs);
            customstreamwriter.Write(email);
            customstreamwriter.Dispose();
            fs.Close();
            fs.Dispose();

            string wellcom = string.Empty;
            if (id !=null || username !=null)
                wellcom =  String.IsNullOrEmpty(username)  ? "wellcome  " + id.ToString() : "wellcome " + username;
            ViewData["wellcome"] = wellcom;
            return View("Index");
        }

        public class CustomStreamWriter : StreamWriter
        {
            public CustomStreamWriter(FileStream stream)
                : base(stream)
            { }

            public override void Write(string value)
            {
                try
                {
                    base.WriteLine(DateTime.Now.ToString() + " sent email to " +  value);
                }
                catch (IOException ex)
                {
                    throw ex;
                }
            }
        }
        public class DB
        {
            /*connectionstr must be save in app.config or web.config and if possible is beter to encrypt it.
            but for now it is hard code for simplify*/

            private string connectionString = @"Server=.\sql2014;Database=tst;User Id=sa;Password = 123;";
            private SqlConnection con;
            public void Connect()
            {
                con = new SqlConnection(connectionString);
                try
                {
                    con.Open();
                }
                catch (SqlException ex)
                {

                    throw ex;
                }
            }
            public SqlDataReader SelectUser(string username, int? id)
            {
                try
                {

                    SqlCommand cmd =
  new SqlCommand("select * from tblUsers where id = " + id + " or username = '" + username + "'", con);

                    if ((int)cmd.ExecuteScalar() <= 0)
                        throw new HttpException(404, "User or UserID not Found");
                    return cmd.ExecuteReader();
                }
                catch (SqlException ex)
                {

                    throw ex;
                }

            }
        }
        // این یک کلاس است که وظیفه ارسال ایمیل را بر عهده دارد
        public class emailer
        {
            private static emailer instance = null;

            public static emailer Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new emailer();
                    }
                    return instance;
                }
            }

            internal void send(string email)
            {
                try
                {
                    // فرض کنید این تکه از کد پیاده سازی شده است
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
        }
    }
}
