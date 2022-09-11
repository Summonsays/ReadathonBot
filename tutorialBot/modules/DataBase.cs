using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Npgsql;
using System.Configuration;


namespace tutorialBot.modules
{
    internal class DataBase
    {
        //temporary will replace with realDB
        string[] names = new string[10];
        string connectionString = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
        //static pagesRead[] sprintUsers = new pagesRead[10];
        public DataBase()
        {

        }



       /*testing DB
        private void createConnection()
        {
            using var con = new NpgsqlConnection(connectionString);
            con.Open();

            var sql = "SELECT * from version()";

            using var cmd = new NpgsqlCommand(sql, con);

                var version = cmd.ExecuteScalar().ToString();
                Console.WriteLine($"PostgreSQL version: {version}");

        }*/

        private void createUser(SocketUser userContext)
        {
            var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "SELECT * from rat_db.users where discord_id=" + userContext.Id;
            var cmd = new NpgsqlCommand(sql, con);

            cmd.CommandText = "INSERT INTO rat_db.users(discord_id, discord_name, creation, lastModified) VALUES(" + userContext.Id+",'"+userContext.Username+ "',:datetimeParamName,:datetimeParamName)";

            NpgsqlParameter param = new NpgsqlParameter(":datetimeParamName", NpgsqlTypes.NpgsqlDbType.Timestamp);
            param.Value = DateTime.Now;
            cmd.Parameters.Add(param); 
            Console.WriteLine(cmd.CommandText);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        private userInfo lookUpUser(SocketUser userContext)
        {
            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "SELECT * from rat_db.users where discord_id="+userContext.Id;
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();
            userInfo temp = new userInfo();



            if (!rdr.HasRows)
            {
                createUser(userContext);
                return temp = new userInfo(userContext.Id, userContext.Username);
                //create user in DB
            }
            while (rdr.Read())
            {
                Console.WriteLine("{0} {1} {2} {3} {4}", rdr.GetInt32(0), rdr.GetInt64(1), rdr.GetString(2), rdr.GetDateTime(3), rdr.GetDateTime(4));
                temp = new userInfo((ulong)rdr.GetInt64(1), rdr.GetString(2));
            }
            con.Close();

            return temp;
        }

        internal void resetUser(SocketCommandContext context, string name="default")
        {
            if (name == "default")
                name = context.User.Username;
            userInfo tempUser = lookUpUserByName(name);
            if (tempUser.getUserID() == 0)//name not found
                return;

            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "delete from rat_db.read_pages where user_id =" + tempUser.getUserID();
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();
        }
        private userInfo lookUpUserByName(string userName)
        {
            userInfo temp = new userInfo();
            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "SELECT * from rat_db.users where discord_name='" + userName+"'";
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();
            if (!rdr.HasRows)
            {
                return temp = new userInfo(0, userName);
                //create user in DB
            }
            while (rdr.Read())
            {
                Console.WriteLine("{0} {1} {2} {3} {4}", rdr.GetInt32(0), rdr.GetInt64(1), rdr.GetString(2), rdr.GetDateTime(3), rdr.GetDateTime(4));
                temp = new userInfo((ulong)rdr.GetInt64(1), rdr.GetString(2));
            }
            con.Close();
            return temp;
        }

        internal void addPages(SocketCommandContext context, int pages)
        {
            userInfo userObj = lookUpUser(context.User);
            var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "Insert into rat_db.read_pages (user_id, createts, modifiedts, pages_read) VALUES(" + userObj.getUserID()+",:datetimeParamName,:datetimeParamName,"+pages+")";
            NpgsqlParameter param = new NpgsqlParameter(":datetimeParamName", NpgsqlTypes.NpgsqlDbType.Timestamp);
            var cmd = new NpgsqlCommand(sql, con);
            param.Value = DateTime.Now;
            cmd.Parameters.Add(param);
            Console.WriteLine(cmd.CommandText);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        internal void addBook(SocketCommandContext context, string title)
        {
            userInfo userObj = lookUpUser(context.User);
            var con = new NpgsqlConnection(connectionString);
            con.Open();
            title = "'" + title + "'";
            var sql = "INSERT INTO rat_db.read_books(user_id, book_title, book_author, createts, modifiedts) VALUES(" + userObj.getUserID()+","+title+ ", null,:datetimeParamName,:datetimeParamName)"; 
            NpgsqlParameter param = new NpgsqlParameter(":datetimeParamName", NpgsqlTypes.NpgsqlDbType.Timestamp);
            var cmd = new NpgsqlCommand(sql, con);
            param.Value = DateTime.Now;
            cmd.Parameters.Add(param);
            Console.WriteLine(cmd.CommandText);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public int getPages(string userName)
        {
            userInfo userObj = lookUpUserByName(userName);
            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "SELECT * from  rat_db.read_pages where user_id=" + userObj.getUserID();
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();
            int pages = 0;
            if (!rdr.HasRows)
            {
                return 0;
            }
            while (rdr.Read())
            {
                pages += rdr.GetInt32(4);
                //Console.WriteLine("{0} {1} {2} {3} {4}", rdr.GetInt32(0), rdr.GetInt64(1), rdr.GetString(2), rdr.GetDateTime(3), rdr.GetDateTime(4));
            }
            con.Close();

            return pages;
        }
        public int getBooks(string userName)
        {
            userInfo userObj = lookUpUserByName(userName);
            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "SELECT * from  rat_db.read_books where user_id=" + userObj.getUserID();
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();
            int books = 0;
            if (!rdr.HasRows)
            {
                return 0;
            }
            while (rdr.Read())
            {
                books++;
            }
            con.Close();

            return books;
        }
    }

    internal class userInfo
    {
        int pages=0;
        ulong userID = 0;
        string username="";

        public userInfo() { }
        public userInfo(ulong userIDTemp, string user)
        {
            userID = userIDTemp;
            username = user;   
        }

        internal ulong getUserID() { return userID; }
        internal string getUserName() { return username; }
        internal int getPages(){ return pages; }
        internal void addPages(int temp)
        {
            pages += temp;
        }
    }
}
