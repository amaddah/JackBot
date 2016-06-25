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
        private Dictionary<string, int> dict = new Dictionary<string, int>();

        public struct test
        {
            public String mot;
            public int index;
        }

        private  String[] mots = new string[]{

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

            // Tele
            "television",
            "TV"


        };

        private test[] assoc = new test[]{

            // Allumer
            new test{mot = "allumer", index = 1},
            new test{mot = "allume", index = 1},

            // Eteindre
            new test{mot = "eteindre", index = 2},
            new test{mot = "eteins", index = 2},

            // Mettre
            
            new test{mot = "mettre", index = 3},
            new test{mot = "mets", index = 3},

            // Être
            new test{mot = "etre", index = 4},

            // Résumé
            new test{mot = "resumer", index = 5},

            // Tele
            new test{mot = "television", index = 6},
            new test{mot = "TV", index = 6},


        };

        MessagesController()
        {
            this.implementDic();
        }

        private void implementDic()
        {
            foreach (test m in this.assoc)
            {
                try
                {
                    this.dict.Add(m.mot, m.index);
                }
                catch(Exception e) { }
            }
        }

        public async Task<Message> Post([FromBody]Message message)
        {
            //implementDict(this.mots);
            if (message.Type == "Message")
            {
                //int length = (message.Text ?? string.Empty).Length;
                //return message.CreateReplyMessage($"[Yoann] Tu as envoyé {length} caractères");
                String[] test = message.Text.Split(' ');
                Message reply = message.CreateReplyMessage();
                reply.Type = "Message";
                
                foreach(String t in test)
                {
                    if (dict.ContainsKey(t))
                        reply.Text += "[Trouvé = {" + dict[t] + "}]";
                }
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