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

        //stockage nom entity
        private static HashSet<String> name_entity = new HashSet<String>();
        private static HashSet<String> name_tag = new HashSet<String>();

        //Stockage des question précédente
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
            new test {mot = "est", index = 8 },
            new test {mot = "qu'est", index = 8 },

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
            new test {mot = "ok", index = 512 },

            //salutation
            new test {mot = "bonjour", index = 1024 },
            new test {mot = "salut", index = 1024 },

            //aujourd'hui
            new test {mot = "aujourd'hui", index = 2048 },

            //Nom chaine
            new test {mot = "channel", index = 4096 },
            new test {mot = "documentaria", index = 4096 },

            //Avoir
            new test {mot = "avoir", index = 8192 },
            
            //Replay
            new test {mot = "replay", index = 16384 },

            //Envoyer
            new test {mot = "envoyer", index = 32768 },
            new test {mot = "l'envoyer", index = 32768 },

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

        private int detectTag(string t, Message reply)
        {
            int rep;
            int taille_entity = name_entity.Count;
            int taille_tag = name_entity.Count;
            String name = t.Substring(1);
            switch (t[0])
            {
                case '#':
                    {
                        if (dict.ContainsKey(name))
                        {
                            name_tag.Add(name);
                            rep = dict[name];
                        }
                        else
                        {
                            reply.Text = "Vous vous êtes trompée de chaîne ou chaîne introuvable.";
                            rep = -1;
                        }
                        break;
                    }
                case '@':
                    {
                        rep = 0;
                        name_entity.Add(name);
                        break;
                    }
                default:
                    {
                        rep = 0;
                        break;
                    }
            }
            return rep;
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
                            return " Voilà le lien pour échanger en direct sur le documentaire avec des gens du monde entier : https://www.twitch.tv/superchaine/v/Latêtesousl’eau ";
                        }
                        else
                            return "Désolé, je ne comprends pas votre commande.";
                    }
                case 7176:
                    {
                        String rep = "Bonjour, Jackbot à votre service." + Environment.NewLine +"Voici le programme de la soirée : " + Environment.NewLine;
                        rep += getProgramm(DateTime.Now.ToString("M/d/yyyy"));
                        return rep;
                    }
                case 24576:
                    {
                        String rep = "Le voici : ";
                        rep += getReplay("0");
                        return rep;
                    }
                case 32768:
                    {
                        String rep = "Voilà c'est envoyé à Marion DuPont";
                        rep += "Je te propose également de voir ces trois documentaires traitant de la même thématique :";
                        rep += Environment.NewLine + "- @Les Dents de la mer : " + getReplay("1");
                        rep += Environment.NewLine + "- @La mer avant tout : " + getReplay("2");
                        rep += Environment.NewLine + "- @La mère mer : " + getReplay("3");
                        rep += Environment.NewLine + "Des fan de l'émission ont créé une conversation publique sur la tête sous l'eau souhaitez vous la rejoindre?";
                        PropositionDiscusion = true;
                        return rep;
                    }
                default:
                    return "Désolé, je ne comprends pas votre commande.";
            }
        }

        private string getReplay(String t)
        {
            String p = "http://channel.hackateam.com/replay/";
            switch (t)
            {
                case "0":
                    p += "971ec5dbf48d9e70f2475a5c586a45ab10e38e65e2a2118c7cf8468894ed1a11";
                    break;
                case "1":
                    p += "28fbe199093d46f0bd5d0216cd20768451114bfc3e2b47be14ca9a52fde6f537";
                    break;
                case "2":
                    p += "da570095254b3b3d1a3ef02451af9016b97afcb6018c788929eebb4f766c57e3";
                    break;
                case "3":
                    p += "d861f7949af91e3b8746a8d279f11655f83375aa0d35452707e49fe3729ee82f";
                    break;
                default:
                    return "Aucun lien disponible.";
            }
            return p;
        }

        private string getProgramm(string v)
        {
            String[] datas = v.Split('/');
            int jour = Int32.Parse(datas[0]);
            int mois = Int32.Parse(datas[1]);
            long annee = Int64.Parse(datas[2]);

            String query = ""; // Plus tard

            return "- 20:00 : @Latêtesousl’eau" + Environment.NewLine +
                "- 21:50 : @LaMerEtLaNature.";

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
                    int tag_num = detectTag(commande, reply);
                    if (tag_num == -1) return reply;
                    reponse += tag_num;                    
                    if (dict.ContainsKey(commande))
                        reponse += dict[commande] ;
                }
                reply.Text = getReponse(reponse);
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