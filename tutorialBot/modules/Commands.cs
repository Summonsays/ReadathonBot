using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Diagnostics;

namespace tutorialBot.modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {

        private static Stopwatch stopWatch = new Stopwatch();
        private tutorialBot.modules.DataBase DB = new DataBase();

        public static void Start()
        {
            stopWatch.Start();
        }

        private bool hasPermission(Discord.WebSocket.SocketGuildUser user)
        {
            //might have to use something else here
            if (user.GuildPermissions.ManageEvents)
                return true;
            else return false;
        }

        [Command("upTime")]
        public async Task UpTime()
        {
            TimeSpan ts = stopWatch.Elapsed;
            int hours = (int)(ts.TotalSeconds/3600);
            int minutes = (int)((ts.TotalSeconds / 60)%60);
            int seconds = (int)(ts.TotalSeconds % 60);
            await ReplyAsync("Time since start is: "+ hours+ "Hr " + minutes+"min "+ seconds+ "secs");
        }

        [Command("addPages")]
        public async Task addPages([Remainder]string text)
        {
            int pages = 0;
            if(text.IndexOf(' ')>-1)
                text=text.Split(' ')[0];
            if (Int32.TryParse(text, out pages))
            {
                DB.addPages(this.Context, pages);
                await ReplyAsync("Added " + text+ " pages " + this.Context.User.Username);
            }
            else
            {
                await ReplyAsync("Invalid pages entered: " + text);
            }
        }

        [Command("addBook")]
        public async Task addPages(string title, string author, [Remainder] string text)
        {
            await ReplyAsync("Title: " + title + " By " + author +" whatever: " + text);
        }
        [Command("t")]
        public async Task t()
        {
            await ReplyAsync(this.Context.User.Username + " has read " + DB.getPages(this.Context) + " pages");
        }

        [Command("stats")]
        public async Task stats()
        {
            await ReplyAsync(this.Context.User.Username + " has read " + DB.getPages(this.Context) +" pages");
        }

            [Command("test")]
        public async Task test()
        {
            if(hasPermission((Discord.WebSocket.SocketGuildUser)this.Context.User))
                await ReplyAsync("yes");
            else
                await ReplyAsync("no");
        }


        [Command("hi")]
        public async Task Hi()
        {
            await ReplyAsync("hello!");
        }
    }
}
