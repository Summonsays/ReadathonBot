using System;
using System.Configuration;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace readAThonBot
{
    public class Program
    {
        static void Main(string[] args) => new Program().RunBotAuto().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        public async Task RunBotAuto()
        {
            _client = new DiscordSocketClient(); 
            _commands = new CommandService();
            _services = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider();

            string token = ConfigurationManager.ConnectionStrings["discordToken"].ConnectionString;

            readAThonBot.modules.Commands.Start();

            _client.Log += _client_Log;
            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandsAsync;
            _client.UserJoined += AnnounceJoinedUser;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            //var channel = _client.GetChannel(/*/TextChannelID/*/) as SocketTextChannel; // Gets the channel to send the message in
            //await channel.SendMessageAsync($"Welcome {user.mention} to {channel.Guild.Name}"); //Welcomes the new user
           /* var channels = this.Context.Guild.Channels;//.find(channel => channel.name === 'Name of the channel');
            foreach (var channel in channels)
            {
                if (channel.Name.IndexOf("rules") > -1)
                {
                    await ((IUser)this.Context.User).SendMessageAsync("welcome: " + this.Context.User.Username + " Please read  <#" + channel.Id + ">");// "<#CHANNELID>);
                }
            }*/
            user.SendMessageAsync("welcome: " + user.DisplayName);            
        }
        private async Task HandleCommandsAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if(message!= null)
            {
                var context = new SocketCommandContext(_client, message);
                if (message.Author.IsBot) return;

                int argPos = 0;
                if (message.HasStringPrefix("!", ref argPos))
                {
                    Console.WriteLine(arg.Content);
                    var result = await _commands.ExecuteAsync(context, argPos, _services);
                    if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                
                }
            }
        }
    }
}