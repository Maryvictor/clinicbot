using ClinicBot.Common.Cards;
using ClinicBot.Data;
using ClinicBot.Dialogs.Avaliacao;
using ClinicBot.Dialogs.CreateAppointment;
using ClinicBot.Infraestrutura.LUIS;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicBot.Dialogs
{
    public class RootDialog: ComponentDialog
    {
        private readonly ILuisService _luisService;
        private readonly IDataBaseService _databaseService;

        public RootDialog(ILuisService luisService, IDataBaseService databaseService)
        {
            _databaseService = databaseService;
            _luisService = luisService;
            var waterfallSteps = new WaterfallStep[]
            {
               InitialProcess,
               FinalProcess
            };
            AddDialog(new AvaliacaoDialog(_databaseService));
            AddDialog(new CreateAppointmentDialog(_databaseService));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> InitialProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var luisResult = await _luisService._luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            return await ManageIntentions(stepContext, luisResult, cancellationToken);
  
        }

        private async Task<DialogTurnResult> ManageIntentions(WaterfallStepContext stepContext, Microsoft.Bot.Builder.RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var topIntent = luisResult.GetTopScoringIntent();
            switch (topIntent.intent)
            {
                case "Saudar":
                    await IntentSaudar(stepContext, luisResult, cancellationToken);
                    break;
                case "Agradecer":
                    await IntentAgradecer(stepContext, luisResult, cancellationToken);
                    break;
                case "Despedir":
                    await IntentDespedir(stepContext, luisResult, cancellationToken);
                    break;
                case "VerContato":
                    await IntentVerContato(stepContext, luisResult, cancellationToken);
                    break;
                case "VerOpcoes":
                    await IntentVerOpcoes(stepContext, luisResult, cancellationToken);
                    break;
                case "Avaliar":
                    return await IntentAvaliar(stepContext, luisResult, cancellationToken);
                case "CriarConsulta":
                    return await IntentCriarConsulta(stepContext, luisResult, cancellationToken);
                case "None":
                    await IntentNone(stepContext, luisResult, cancellationToken);
                    break;
                default:
                    break;
            }
            return await stepContext.NextAsync(cancellationToken);
        }

        private async Task<DialogTurnResult> IntentCriarConsulta(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(CreateAppointmentDialog), cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> IntentAvaliar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(AvaliacaoDialog), cancellationToken: cancellationToken);
        }

        private async Task IntentSaudar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Olá, que bom te ver!", cancellationToken: cancellationToken);
        }
        private async Task IntentAgradecer(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Não se preocupe, eu gosto de ajudar", cancellationToken: cancellationToken);
        }

        private async Task IntentDespedir(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Espero te ver de novo", cancellationToken: cancellationToken);
        }
        private async Task IntentVerContato(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            string phoneDetail = $"Nossos números são os seguintes:{Environment.NewLine}" + $" 📞 + 11 99999999{Environment.NewLine} 📞 + 11 88888888";
            string addressDetail = $"🏨 Nosso endereço{Environment.NewLine} Rua dos Bobos, 0, Limoeiro";
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync(phoneDetail, cancellationToken: cancellationToken);
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync(addressDetail, cancellationToken: cancellationToken);
            await stepContext.Context.SendActivityAsync("Em que mais posso ajudar?", cancellationToken: cancellationToken);
        }

        private async Task IntentVerOpcoes(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Aqui tenho minhas opções", cancellationToken: cancellationToken);
            await MainOptionsCard.ToShow(stepContext, cancellationToken);
        }
        private async Task IntentNone(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Não entendi o que você disse", cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

    }
}

