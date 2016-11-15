using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BYOD_manager.Models;
using System.IO;
using System.Net;

namespace BYOD_manager.Controllers
{
    public class SyncUserController : Controller
    {
        // Private variable declarations
        private BYODEntities1 db = new BYODEntities1();
        private static int newUsers = 0;
        private static string fileName = "";

        // Index main view
        public ActionResult Index()
            {
            syncUsers();
            ViewBag.newUsers = newUsers;
            ViewBag.fileName = fileName;
            ViewBag.Message = "Users have been synced with IDAttend export";
            return View();
            }

        /// <summary>
        /// Finds the latest student export and insets each student (one per line) into 
        /// the SQL databse. 
        /// </summary>
        [NonAction]
        public static void syncUsers()
        {
            var directory = new DirectoryInfo(@"\\10.5.129.1\\apps\\IDattend\\Import\\Student");
            var myFile = directory.GetFiles()
             .OrderByDescending(f => f.LastWriteTime)
             .First();

            int i = 1;
            while (!myFile.ToString().StartsWith("ea_7131_student"))
                {
                   myFile = directory.GetFiles()
                   .OrderByDescending(f => f.LastWriteTime).Skip(i)
                   .First();
                    i++;
                }

            fileName = myFile.ToString();
            string[] records = System.IO.File.ReadAllLines(@directory + "\\" + myFile.ToString());

            foreach (string record in records)
            {
                string[] fields = record.Split(',');
                if (fields[0] == "MISID")
                {
                    //Do nothing
                }
                else
                {
                    try
                       {
                        if (!(fields[0] == "") && !(fields[1] == "")) {
                            newUser(fields[0], fields[1], fields[3], fields[2], fields[18], false);
                            newUsers++;
                        }
                        }   
                    catch {   }
                } // else
            } // foreach loop
        } // syncUsers

        /// <summary>
        /// Creates a new user with input variables read from the Student Details CSV file.
        /// </summary>
        public static void newUser(string EQID, string MISID, string firstName, string lastName, string yearLevel, bool approved)
        {
            string connectionString = "Persist Security Info=False;User ID=NSSCF;Password=Springfield.4;Initial Catalog=BYOD;Server=eqmtn7131003";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO BYOD (EQID, MISID, Fname, Lname, YearLevel, Approved) VALUES (@EQID, @MISID, @Fname, @Lname, @YearLevel, @Approved)");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@EQID", EQID);
                cmd.Parameters.AddWithValue("@MISID", MISID);
                cmd.Parameters.AddWithValue("@Fname", firstName);
                cmd.Parameters.AddWithValue("@Lname", lastName);
                cmd.Parameters.AddWithValue("@YearLevel", yearLevel);
                cmd.Parameters.AddWithValue("@Approved", approved);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void deleteUser(string MISID)
        {
            string connectionString = "Persist Security Info=False;User ID=NSSCF;Password=Springfield.4;Initial Catalog=BYOD;Server=eqmtn7131003";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM BYOD WHERE MISID='" + MISID + "'");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
