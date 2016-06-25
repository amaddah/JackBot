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
using System.Web.UI.WebControls;
using System.Text;
using System.Globalization;

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

        private Dictionary<int, string> dictCom = new Dictionary<int, string>();

        public struct test
        {
            public String mot;
            public int index;
        }

        public struct testFunction
        {
            public int index;
            public string message;
        }

        private test[] assoc = new test[]{

            // Allumer
            new test{mot = "allumer", index = 1},
            new test{mot = "allume", index = 1},

            // Eteindre
            new test{mot = "eteindre", index = 2},
            new test{mot = "eteins", index = 2},

            // Mettre
            
            new test{mot = "mettre", index = 4},
            new test{mot = "mets", index = 4},

            // Être
            new test{mot = "etre", index = 8},

            // Résumé
            new test{mot = "resumer", index = 16},

            // Tele
            new test{mot = "television", index = 32},
            new test{mot = "tv", index = 32},


        };

        private testFunction[] assocFunc = new testFunction[]{

            // Télévision
            new testFunction{index = 33, message = "J'ai allumé la télévision."},
            new testFunction{index = 34, message = "J'ai éteins la télévision"}
        };

        MessagesController()
        {
            this.implementDic();
            this.implementDicCom();
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

        private void implementDicCom()
        {
            foreach (testFunction m in this.assocFunc)
            {
                try
                {
                    this.dictCom.Add(m.index, m.message);
                }
                catch (Exception e) { }
            }
        }

        static string getCommande(string t)
        {
            string text = t.ToLower();
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
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
                int reponse = 0;
                foreach (string t in test)
                {
                    string commande = getCommande(t);
                    if (dict.ContainsKey(commande))
                        reponse += dict[commande];
                }
                if (dictCom.ContainsKey(reponse))
                    reply.Text += dictCom[reponse];
                else
                    reply.Text += "[index=" + reponse + "] Rien trouver";

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