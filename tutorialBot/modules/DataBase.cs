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
        static IList<pagesRead> sprintUsers = new List<pagesRead>();
        string connectionString = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
        //static pagesRead[] sprintUsers = new pagesRead[10];
        public DataBase()
        {

        }



       /*testing DB*/
        private void createConnection()
        {
            using var con = new NpgsqlConnection(connectionString);
            con.Open();

            var sql = "SELECT * from version()";

            using var cmd = new NpgsqlCommand(sql, con);

                var version = cmd.ExecuteScalar().ToString();
                Console.WriteLine($"PostgreSQL version: {version}");

        }

        private void createUser(SocketUser userContext)
        {
            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "SELECT * from \"RATDB\".users where discordID=" + userContext.Id;
            using var cmd = new NpgsqlCommand(sql, con);

            cmd.CommandText = "INSERT INTO \"RATDB\".users(discordID, discordName, creation, lastModified) VALUES(" + userContext.Id+",'"+userContext.Username+ "',:datetimeParamName,:datetimeParamName)";

            NpgsqlParameter param = new NpgsqlParameter(":datetimeParamName", NpgsqlTypes.NpgsqlDbType.Timestamp);
            param.Value = DateTime.Now;
            cmd.Parameters.Add(param); 
            Console.WriteLine(cmd.CommandText);
            cmd.ExecuteNonQuery();
        }


        private pagesRead lookUpUser(SocketUser userContext)
        {
            using var con = new NpgsqlConnection(connectionString);
            con.Open();
            var sql = "SELECT * from \"RATDB\".users where discordID="+userContext.Id;
            using var cmd = new NpgsqlCommand(sql, con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                //read the user data
            }
            else
            {
                createUser(userContext);
                //create user in DB
            }
            while (rdr.Read())
            {
                Console.WriteLine("{0} {1} {2} {3} {4}", rdr.GetInt32(0), rdr.GetInt64(1), rdr.GetString(2), rdr.GetDateTime(3), rdr.GetDateTime(4));
            }

            foreach (pagesRead user in sprintUsers)
            {
                ulong id = userContext.Id;
                if (user.getUserID() == userContext.Id)
                    return user;
            }
            //could not find user
            pagesRead temp = new pagesRead(userContext.Id, userContext.Username);
            sprintUsers.Add(temp);
            return temp;
        }

        internal void addPages(SocketCommandContext context, int pages)
        {
            pagesRead userObj = lookUpUser(context.User);
            userObj.addPages(pages);
        }

        public int getPages(SocketCommandContext context)
        {
            pagesRead userObj = lookUpUser(context.User);
            return userObj.getPages();
        }
    }

    internal class pagesRead
    {
        int pages=0;
        ulong userID = 0;
        string username="";

        public pagesRead() { }
        public pagesRead(ulong userIDTemp, string user)
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
