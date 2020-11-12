using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicBot.Common.Cards
{
    public class MainOptionsCard
    {
        public static async Task ToShow(DialogContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(activity: CreateCarousel(), cancellationToken);
        }
        private static Activity CreateCarousel()
        {
            var cardConsultasMedicas = new HeroCard
            {
                Title = "Consultas Médicas",
                Subtitle = "Opções",
                Images = new List<CardImage> { new CardImage("https://clinicbotstoragev2.blob.core.windows.net/images/menu_01.jpg") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Criar consulta médica", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "Ver minha consulta", Type = ActionTypes.ImBack }
                }
            };

            var cardInformacaoContato = new HeroCard
            {
                Title = "Informação de Contato",
                Subtitle = "Opções",
                Images = new List<CardImage> { new CardImage("https://clinicbotstoragev2.blob.core.windows.net/images/menu_02.jpg") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(){Title = "Centro de Contato", Value = "Centro de Contato", Type = ActionTypes.ImBack},
                    new CardAction(){Title = "Web Site", Value = "https://docs.microsoft.com/pt-br/", Type = ActionTypes.OpenUrl }
                }
            };

            var cardRedesSociais = new HeroCard
            {
                Title = "Sigo nos na redes",
                Subtitle = "Opções",
                Images = new List<CardImage> { new CardImage("https://clinicbotstoragev2.blob.core.windows.net/images/menu_03.png") },
                Buttons = new List<CardAction>()
                {

                    new CardAction(){Title = "Facebook", Value = "https://m.facebook.com/FaTechGirls/", Type = ActionTypes.OpenUrl },
                    new CardAction(){Title = "Instagram", Value = "https://www.instagram.com/fatechgirls/", Type = ActionTypes.OpenUrl },
                    new CardAction(){Title = "Twitter", Value = "https://twitter.com/fatechgirls", Type = ActionTypes.OpenUrl }
                }
            };
            
           

    var cardAvaliacao = new HeroCard
    {
        Title = "Avaliacao",
        Subtitle = "Opções",
        Images = new List<CardImage> { new CardImage("https://clinicbotstoragev2.blob.core.windows.net/images/menu_04.jpg") },
        Buttons = new List<CardAction>()
                {

                    new CardAction(){Title = "Avaliar Bot", Value = "Avaliar Bot", Type = ActionTypes.ImBack },
                    
    }
            
            };

            var optionsAttachments = new List<Attachment>()
            {
                cardConsultasMedicas.ToAttachment(),
                cardInformacaoContato.ToAttachment(),
                cardRedesSociais.ToAttachment(),
                cardAvaliacao.ToAttachment(),
            };

            var reply = MessageFactory.Attachment(optionsAttachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            return reply as Activity;

        }
    }
}
