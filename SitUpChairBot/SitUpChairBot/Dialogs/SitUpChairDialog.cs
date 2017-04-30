using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SitUpChairBot.Dialogs
{
    [Serializable]
    public class SitUpChairDialog : LuisDialog<object>
    {
        public SitUpChairDialog(params ILuisService[] services) : base(services)
        {
           
        }

        protected async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = (Activity)await item;
            await base.MessageReceived(context, item);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Désolé je n'ai pas compris");
            context.Wait(MessageReceived);
        }

        [LuisIntent("LazyAss")]
        public async Task LazyAss(IDialogContext context, LuisResult result)
        {
            var test = CreateCard(context);

            await SendProactiveMessage("U55T1EW57:T56C6R3UK", test);

            context.Wait(MessageReceived);
        }

        [LuisIntent("information")]
        public async Task Information(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Mon but premier est de faire bouger les gens lorsqu'ils restent trop longtemps assis");
            context.Wait(MessageReceived);
        }

        [LuisIntent("birth")]
        public async Task Birth(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("J'ai vu le jour lors d'un magnifique hackathon au Microsoft Innovation Center.");
            context.Wait(MessageReceived);
        }

        private static async Task SendProactiveMessage(string slackId, Activity msg) // slackId Id de la personne sur slack ??
        {
            const string slackConnector = "https://slack.botframework.com";
            MicrosoftAppCredentials.TrustServiceUrl(slackConnector);
            var recipient = new ChannelAccount(slackId.ToString());
            var account = new ChannelAccount("B578NFHP0:T56C6R3UK");
            var connector = new ConnectorClient(new Uri(slackConnector), ConfigurationManager.AppSettings["MicrosoftAppId"],
                                                ConfigurationManager.AppSettings["MicrosoftAppPassword"]);

            //msg = Activity.CreateMessageActivity();
            msg.Type = ActivityTypes.Message;
            msg.From = account;
            msg.Recipient = recipient;
            msg.ChannelId = "slack";
            var conversation = "B578NFHP0:T56C6R3UK:D56ECHQKC";
            msg.Conversation = new ConversationAccount(id: conversation);
            msg.Text = "Bonjour Louis, ça fait trop longtemps que tu es assis il est temps de bouger, je te propose : ";
            await connector.Conversations.SendToConversationAsync((Activity)msg);
        }

        public Activity CreateCard(IDialogContext context)
        {
            string Url = "http://situpchair.azurewebsites.net";

            Activity replyToConversation;

            replyToConversation = ((Activity)context.Activity).CreateReply("Voici ce que je te propose : ");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            replyToConversation.AttachmentLayout = "carousel";
            replyToConversation.Attachments = new List<Attachment>();

            List<string> pictures = new List<string>();
            pictures.Add(Url + $"/Images/allongement-jambes.jpg");
            pictures.Add(Url + $"/Images/pieds-sureleves.jpg");
            pictures.Add(Url + $"/Images/halteres.jpg");
            pictures.Add(Url + $"/Images/genoux-leves.jpg");
            pictures.Add(Url + $"/Images/grandir.jpg");
            pictures.Add(Url + $"/Images/replis-vers-lavant.jpg");

            List<string> category = new List<string>();
            category.Add("Allongement jambes");
            category.Add("Pieds surélevés ");
            category.Add("Haltères ");
            category.Add("Genoux levés  ");
            category.Add("Grandir  ");
            category.Add("Replis vers l’avant  ");

            List<string> decription = new List<string>();
            decription.Add("Levez les jambes jointes verticalement, maintenez 10 secondes. Reposez - vous 3 secondes.Faites une série de 10.");
            decription.Add("Relevez les deux jambes de 5 centimètres. Tenez pendant 10 secondes en contractant vos abdominaux puis relâchez.Faites une série de 10.");
            decription.Add("Porter bouteille d’eau comme des haltères ");
            decription.Add("Tenez-vous assis, le dos toujours droit, les coudes sur la table. En expirant, contractez vos abdominaux tout en levant un genou sous la table, puis relâchez en inspirant.Alternez avec l'autre jambe et réalisez cet exercice une dizaine de fois.  ");
            decription.Add("Sur votre siège, appuyez les mains sur vos cuisses, tout en vous étirant la colonne vertébrale, comme si vous cherchiez à vous grandir. Effectuez cet exercice 5 minutes.");
            decription.Add("Faites tomber votre crayon à vos pieds et allez le récupérer en vous servant de vos deux mains. Réitérez l’action 5 fois.");

            List<string> benefits = new List<string>();
            benefits.Add("Il vous permet de muscler en douceur vos jambes et vos abdominaux.");
            benefits.Add("Votre ceinture abdominale en sortira renforcée, tout comme vos cuisses.");
            benefits.Add("Renforcer biceps et triceps");
            benefits.Add("Ce sont vos cuisses qui vont travailler.");
            benefits.Add("");
            benefits.Add("");


            Random rnd = new Random();
            int heroCard = rnd.Next(0, category.Count());

            List<CardImage> cardsImage = new List<CardImage>();
            CardImage img = new CardImage(url: pictures[heroCard]);
            cardsImage.Add(img);

            HeroCard tmp = new HeroCard()
            {
                Images = cardsImage,
                Title = decription[heroCard],
                Subtitle = category[heroCard],
                Text = benefits[heroCard]
            };
            Attachment plAttachment = tmp.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);

            return replyToConversation;
        }
    }
}