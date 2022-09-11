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
        private static Stopwatch sprintTime = new Stopwatch();
        private tutorialBot.modules.DataBase DB = new DataBase();
        private static commandRegistry allCommands = new commandRegistry();
        private static int howMad = 0;
        //in minutes
        private static int sprintDuration = 0;

        //private string[] commands = new string[] { "hi", "help", "bookCount", "testy", "whatstheanswertolifetheuniverseandeverything", "haveAdminPerm", "stats", "addPages", "addBook", "restUser" };

        public static void Start()
        {
            stopWatch.Start();
            CustomCommand temp = new CustomCommand();
            temp = new CustomCommand("addBook", "Enter a book you've read");
            allCommands.register(temp);
            temp = new CustomCommand("addPages", "Enter a number to add pages to your total");
            allCommands.register(temp);
            temp = new CustomCommand("bookCount", "By defualt returns number of books you have read. Add a username to it to find out how many books someone else has read");
            allCommands.register(temp);
            temp = new CustomCommand("haveAdminPerm", "This returns if you have elivated permissions");
            allCommands.register(temp);
            temp = new CustomCommand("help", "This is the help command, supply with second command for details");
            allCommands.register(temp);
            temp = new CustomCommand("hi", "Bots love attention too!");
            allCommands.register(temp);
            temp = new CustomCommand("resetUser", "WARNING! This will erase your stats!");
            allCommands.register(temp);
            temp = new CustomCommand("stats", "This returns your personal stats. Add a username to see other people's stats");
            allCommands.register(temp);
            temp = new CustomCommand("testy", "You won't like me when I'm angry!!!!!");
            allCommands.register(temp);
            temp = new CustomCommand("uptime", "Returns the time since bot was started");
            allCommands.register(temp);
            temp = new CustomCommand("whatstheanswertolifetheuniverseandeverything", "Did you want to ask another question?");
            allCommands.register(temp);

        }

        public async void startSprint(int time)
        {
            sprintTime.Start();
            await Task.Delay(time);
            stopSprint();
        }

        public async void stopSprint()
        {
            if(sprintTime.IsRunning)
            {
                await ReplyAsync("sprint ended");
                sprintTime.Reset();
            }
        }

        [Command("help")]
        public async Task help()
        {
            await ReplyAsync(allCommands.help());
        }

        [Command("help")]
        public async Task help(string command)
        {
            if (command == "Alwaysafk")
                await ReplyAsync("I can't help you, no one can.");
            else
                await ReplyAsync(allCommands.help(command));
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
            int hours = (int)(ts.TotalSeconds / 3600);
            int minutes = (int)((ts.TotalSeconds / 60) % 60);
            int seconds = (int)(ts.TotalSeconds % 60);
            await ReplyAsync("Time since start is: " + hours + "Hr " + minutes + "min " + seconds + "secs");
        }
        [Command("resetUser")]
        public async Task resetUserData([Remainder] string text)
        {
            if (!hasPermission((Discord.WebSocket.SocketGuildUser)this.Context.User))
                return;
            if (text == "")
            {
                DB.resetUser(this.Context);
                await ReplyAsync("User " + this.Context.User.Username + " was reset");
            }
            else
            {
                DB.resetUser(this.Context, text);
                await ReplyAsync("User " + text + " was reset");
            }

        }
        [Command("resetUser")]
        public async Task resetUserData()
        {
            if (!hasPermission((Discord.WebSocket.SocketGuildUser)this.Context.User))
                return;
            DB.resetUser(this.Context);
            await ReplyAsync("User " + this.Context.User.Username + " was reset");

        }

        [Command("addPages")]
        public async Task addPages([Remainder] string text)
        {
            int pages = 0;
            if (text.IndexOf(' ') > -1)
                text = text.Split(' ')[0];
            if (Int32.TryParse(text, out pages))
            {
                if (pages < 0)
                    await ReplyAsync("You can't unread pages... " + this.Context.User.Username);
                else
                {
                    DB.addPages(this.Context, pages);
                    await ReplyAsync("Added " + text + " pages " + this.Context.User.Username);
                }
            }
            else
            {
                await ReplyAsync("Invalid pages entered: " + text);
            }
        }

        [Command("Finished")]
        public async Task addBook(string input, [Remainder] string text)
        {
            input = input + " " + text;
            DB.addBook(this.Context, input);   
            await ReplyAsync(input);
        }

        [Command("t")]
        public async Task t()
        {
            await ReplyAsync(this.Context.User.Username + " has read " + DB.getPages(this.Context.User.Username) + " pages");
        }

        [Command("stats")]
        public async Task stats()
        {
            int books = DB.getBooks(this.Context.User.Username);
            int pages = DB.getPages(this.Context.User.Username);

            string response = this.Context.User.Username + " has read " + books +" book";
            if (books>1)
            {
                response += "s";
            }
            response += " and has read " + pages + " pages";
            await ReplyAsync(response);
        }
        [Command("stats")]
        public async Task stats(string userName)
        {
            await ReplyAsync(userName + " has read " + DB.getPages(userName) + " pages");
        }

        [Command("haveAdminPerm")]
        public async Task test()
        {
            if (hasPermission((Discord.WebSocket.SocketGuildUser)this.Context.User))
                await ReplyAsync("yes");
            else
                await ReplyAsync("no");
        }

        [Command("whatstheanswertolifetheuniverseandeverything")]
        public async Task theQuestion()
        {
            await ReplyAsync("42");
        }

        [Command("testy")]
        public async Task unhappy()
        {
            string angry = "O w O";
            if (howMad == 1)
                angry = ">.<!";
            if (howMad == 2)
                angry = "(┛ಠ_ಠ)┛彡┻━┻";
            if (howMad == 3)
                angry = "(ノಠ益ಠ)ノ彡┻━┻";
            if (howMad == 4)
                angry = "┬─┬ノ(º _ ºノ)";
            howMad++;
            if (howMad > 4)
                howMad = 0;
            await ReplyAsync(angry);
        }

        [Command("bookCount")]
        public async Task bookCount()
        {
            await ReplyAsync(this.Context.User.Username + " has read " + DB.getBooks(this.Context.User.Username) + " books");
        }

        [Command("startSprint")]
        public async Task startSprint(string timeStr)
        {
            int time = 0;
            if (timeStr.IndexOf(' ') > -1)
                timeStr = timeStr.Split(' ')[0];
            if (Int32.TryParse(timeStr, out time))
            {
                if (time < 0)
                    await ReplyAsync("No negative sprint times" + this.Context.User.Username);
                else
                {
                    if (!sprintTime.IsRunning)
                    {
                        if(time> 35000)
                            await ReplyAsync("This Sprint is too long");
                        else
                        {
                            time = time*60000 ;
                            sprintDuration = time;
                            startSprint(time);
                            await ReplyAsync("Sprint started for  " + timeStr + " minutes");
                        }
                    }
                    else
                        await ReplyAsync("Sprint already started");
                }
            }
            else
            {
                await ReplyAsync("Invalid time entered: " + timeStr);
            }
        }
        [Command("stopSprint")]
        public async Task stopSprintCommand()
        {
            if (!hasPermission((Discord.WebSocket.SocketGuildUser)this.Context.User))
                return;
            stopSprint();
        }
        [Command("time")]
        public async Task timeSinceStart()
        {
            await ReplyAsync(sprintTime.Elapsed.ToString());
        }
        [Command("timeLeft")]
        public async Task timeLeft()
        {
            if(!sprintTime.IsRunning)
                await ReplyAsync("No sprint currently running");
            else
            {
                int temp = sprintDuration;// * 6000;
                //seconds left
                temp = (int)((temp - (int)sprintTime.Elapsed.TotalMilliseconds)/1000);
                int hours = (int)(temp / 3600);
                int minutes = (int)((temp / 60) % 60);
                int seconds = (int)(temp % 60);
                await ReplyAsync("Time left: "+hours+":"+minutes+":"+seconds);
            }
        }

        [Command("hi")]
        public async Task Hi()
        {
            await ReplyAsync("hello!");
        }

        [Command("Paige")]
        public async Task paige()
        {
            if(this.Context.User.Id== 184478513951145985)
                await ReplyAsync("Paige is best wife!");
            else if (this.Context.User.Id == 449345265598464001)
                await ReplyAsync("You are awesome!");
            else
                await ReplyAsync("Paige is in charge! Don't mess with her!");
        }


    }

    internal class commandRegistry
    {
        static IList<CustomCommand> registeredCommands = new List<CustomCommand>();

        public void register(CustomCommand temp)
        {
            registeredCommands.Add(temp);
        }

        public bool isRegistered(CustomCommand temp)
        {
            return registeredCommands.Contains(temp);
        }

        public string help()
        {
            string temp = "The list of commands are as follows: ";
            foreach (CustomCommand item in registeredCommands)
            {
                temp += "\n" + item.getCommand();
            }
            return temp;
        }

        public string help(string commandName)
        {
            foreach(CustomCommand item in registeredCommands)
            {
                if(item.getCommand()== commandName)
                    return item.getHelp();
            }
            return "command not found";
        }
    }
    internal class CustomCommand
    {
        string commandWord = "";
        string helpMessage = "";
        public CustomCommand() { }
        public CustomCommand(string s1, string s2)
        {
            commandWord = s1;
            helpMessage = s2;
        }
        public string getCommand() { return commandWord; }
        public string getHelp() { return helpMessage; }
    }
}
