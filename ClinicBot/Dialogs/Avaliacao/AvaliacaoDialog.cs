using ClinicBot.Common.Models;
using ClinicBot.Data;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicBot.Dialogs.Avaliacao
{
    public class AvaliacaoDialog : ComponentDialog
    {


        private IDataBaseService _dataBaseService;
        public AvaliacaoDialog(IDataBaseService dataBaseService)
        {
            _dataBaseService = dataBaseService;

            var waterfallSteps = new WaterfallStep[]
            {
                ToShowButton,
                ValidateOptionsAsync
            };
            
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
        }

        public AvaliacaoDialog()
        {
        }

        private async Task<DialogTurnResult> ToShowButton(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = CreateButtonsQualification()
                },
                cancellationToken
                );
        }
       
        private Activity CreateButtonsQualification()
        {
            var reply = MessageFactory.Text("Avalie me por favor");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "1⭐", Value = "1⭐", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "2⭐", Value = "2⭐", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "3⭐", Value = "3⭐", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "4⭐", Value = "4⭐", Type = ActionTypes.ImBack },
                    new CardAction() { Title = "5⭐", Value = "5⭐", Type = ActionTypes.ImBack }


                }
            };
            return reply as Activity;

        }
        private async Task<DialogTurnResult> ValidateOptionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var options = stepContext.Context.Activity.Text;
            await stepContext.Context.SendActivityAsync($"Obrigado por você {options}", cancellationToken: cancellationToken);
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync("Em que mais posso ajudar?", cancellationToken: cancellationToken);
            //Salvando Avaliação
            await SaveAvaliacao(stepContext, options);

            return await stepContext.ContinueDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task SaveAvaliacao(WaterfallStepContext stepContext, string options)
        {
            var avaliacaoModel = new AvaliacaoModel();
            avaliacaoModel.id = Guid.NewGuid().ToString();
            avaliacaoModel.idUser = stepContext.Context.Activity.From.Id;
            avaliacaoModel.avaliacao = options;
            avaliacaoModel.registerDate = DateTime.Now.Date;
            await _dataBaseService.Avaliacao.AddAsync(avaliacaoModel);
            await _dataBaseService.SaveAsync();
        }
    }
}
