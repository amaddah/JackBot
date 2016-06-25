using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JackBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        private Dictionary<int, string> dict;

        private  String[] mots = {

            // Allumer
            "allumer",
            "allume",

            // Eteindre
            "eteindre",
            "eteins",

            // Mettre
            "mettre",
            "mets",


            // Être
            "etre",
            "est",

            // Résumé
            "resumer",
            "resume",


        };

        private void implementDict()
        {
            int i = 1;
            foreach (String m in this.mots) {
                Console.WriteLine(m);
                //this.dict.Add(i, m);
                i++;
             }
        }

        public async Task<Message> Post([FromBody]Message message)
        {
            implementDict();
            //implementDict(this.mots);
            if (message.Type == "Message")
            {
                //int length = (message.Text ?? string.Empty).Length;
                //return message.CreateReplyMessage($"[Yoann] Tu as envoyé {length} caractères");
                String[] test = message.Text.Split(' ');
                Message reply = message.CreateReplyMessage();
                reply.Type = "Message";
                
                //foreach(String t in test)
                //{
                //    if (dict.ContainsValue(t))
                //        reply.Text = "[Trouvé = {" + t + "}]";
                //}
                //reply.Text += "[" + t + "]";
                reply.Text = "https://google.com";
                return reply;
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                reply.Text = "Cc";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}