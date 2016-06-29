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

        private static bool PropositionRegarderEmision = false;
        private static bool PropositionDiscusion = false;

        public struct test
        {
            public String mot;
            public int index;
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

            //montrer
            new test {mot = "montrer", index = 64 },

            //documentaire
            new test {mot = "documentaire", index = 128 },

            //agriculture
            new test {mot = "l'agriculture", index = 256 },
            new test {mot = "agriculture", index = 256 },

            //aquiessement
            new test {mot = "oui", index = 512 },
            new test {mot = "ok", index = 512 }

            


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


        private string getReponse(int index)
        {
            
            switch (index)
            {
                case 33:
                    return "J'ai allumé la télévision.";
                case 34:
                    return "J'ai éteint la télévision";
                case 448:
                    {
                        PropositionRegarderEmision = true;
                        return "Salut, je te propose \"TheLastFarmer\" un documentaire qui explore les conséquences dramatiques du néoliberalisme et sur la vie des petits paysans sur Terre. Veux-tu le regarder?";
                    }
                case 512:
                    {
                        if(PropositionRegarderEmision)
                        {
                            PropositionRegarderEmision = false;
                            PropositionDiscusion = true;
                            return "Il est diffusé en ce moment en français. Souhaites-tu discuter avec des personnes qui regardent le documentaire ?";
                        }
                        else if(PropositionDiscusion)
                        {
                            PropositionDiscusion = false;
                            return " Voilà le lien pour échanger en direct sur le documentaire avec des gens du monde entier : https://www.twitch.tv/superchaine/v/thelastfarmer";
                        }
                        else
                            return "Désolé je ne peux pas répondre à cette demande ou je ne la comprends pas";
                    }
                default:
                    return "Désolé je ne peux pas répondre à cette demande ou je ne la comprends pas";
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
            if (message.Type == "Message")
            {
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
                reply.Text += getReponse(reponse);
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