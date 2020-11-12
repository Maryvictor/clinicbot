using ClinicBot.Common.Models.MedicalAppointment;
using ClinicBot.Common.Models.User;
using ClinicBot.Data;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicBot.Dialogs.CreateAppointment
{

    
    public class CreateAppointmentDialog: ComponentDialog
    {
        private readonly IDataBaseService _databaseService;
        public static UserModel newUserModel = new UserModel();
        public static MedicalAppointmentModel medicalAppointment = new MedicalAppointmentModel();
        public CreateAppointmentDialog(IDataBaseService databaseService)
        {
            _databaseService = databaseService;
            var waterterfallStep = new WaterfallStep[]
            {
                SetPhone,
                SetFullName,
                SetEmail,
                SetDate,
                SetTime,
                Confirmation,
                FinalProcess
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterterfallStep));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
        }

       
        private async Task<DialogTurnResult> SetPhone(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions { Prompt = MessageFactory.Text("Por favor, informe seu número de telefone: ")}, 
                cancellationToken
                );
        }

        private async Task<DialogTurnResult> SetFullName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userPhone = stepContext.Context.Activity.Text;
            newUserModel.phone = userPhone;

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions { Prompt = MessageFactory.Text("Agora informe seu nome completo")},
                cancellationToken
          );
        }

        private async Task<DialogTurnResult> SetEmail(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var fullNameUser = stepContext.Context.Activity.Text;
            newUserModel.fullName = fullNameUser;

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions { Prompt = MessageFactory.Text("Agora informe seu email:")},
                cancellationToken
                );
        }

        private async Task<DialogTurnResult> SetDate(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userEmail = stepContext.Context.Activity.Text;
            newUserModel.email = userEmail;

            string text = $"Agora necessito uma consulta médica com o seguinte formato:" + $"{ Environment.NewLine}dd/mm/yyyy";
               
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions { Prompt = MessageFactory.Text(text)},
                cancellationToken
                );
        }

        private async Task<DialogTurnResult> SetTime(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            
            var medicalDate = stepContext.Context.Activity.Text;
            medicalAppointment.date = Convert.ToDateTime(medicalDate);

            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions { Prompt = CreateButtonsTime()},
                cancellationToken
                );
        }

        

        private async Task<DialogTurnResult> Confirmation(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var medicalTime = stepContext.Context.Activity.Text;
            medicalAppointment.time = int.Parse(medicalTime);

            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions { Prompt = CreateButtonConfirmation()},
                cancellationToken
                );
        }

       
        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userConfirmation = stepContext.Context.Activity.Text;

            if (userConfirmation.ToLower().Equals("sim"))
            {
                //Save DataBase
                string userId = stepContext.Context.Activity.From.Id;
                var userModel = await _databaseService.User.FirstOrDefaultAsync(x => x.id == userId);

                // Update user
                userModel.phone = newUserModel.phone;
                userModel.fullName = newUserModel.fullName;
                userModel.email = newUserModel.email;

                _databaseService.User.Update(userModel);
                await _databaseService.SaveAsync();

                //Save medical appointment
                medicalAppointment.id = Guid.NewGuid().ToString();
                medicalAppointment.idUser = userId;
                await _databaseService.MedicalAppointments.AddAsync(medicalAppointment);
                await _databaseService.SaveAsync();

                await stepContext.Context.SendActivityAsync("Sua consulta foi salva com êxito!", cancellationToken: cancellationToken);

                //Show sumary
                string sumaryMedical = $"Para: {userModel.fullName}" +
                    $"{Environment.NewLine} 📞 Telefone: {userModel.phone}" +
                    $"{Environment.NewLine} 📧 Email: {userModel.email}" +
                    $"{Environment.NewLine} 📅 Data: {medicalAppointment.date}" +
                    $"{Environment.NewLine} ⏰ Hora: {medicalAppointment.time}";



                await stepContext.Context.SendActivityAsync(sumaryMedical, cancellationToken: cancellationToken);
                await Task.Delay(1000);
                await stepContext.Context.SendActivityAsync("Em que mais posso ajudar?", cancellationToken: cancellationToken);
                medicalAppointment = new MedicalAppointmentModel();

            }
            else
            {
                await stepContext.Context.SendActivityAsync("Não tem problema, será na próxima", cancellationToken: cancellationToken);
            }
            return await stepContext.ContinueDialogAsync(cancellationToken);
        }


        private Activity CreateButtonConfirmation()
        {
            var reply = MessageFactory.Text("Confirma a criação dessa consulta médica?");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                new CardAction(){Title = "Sim", Value= "Sim", Type = ActionTypes.ImBack },
                new CardAction(){Title = "Não", Value = "Não", Type = ActionTypes.ImBack },
            }

            };
            return reply as Activity;
        }
        private Activity CreateButtonsTime()
        {
            var reply = MessageFactory.Text("Agora selecione a hora desejada");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){Title = "9", Value="9", Type= ActionTypes.ImBack},
                    new CardAction(){Title = "10", Value="10", Type= ActionTypes.ImBack},
                    new CardAction(){Title = "11", Value="11", Type= ActionTypes.ImBack},
                    new CardAction(){Title = "12", Value="12", Type= ActionTypes.ImBack},
                    new CardAction(){Title = "13", Value="13", Type= ActionTypes.ImBack},
                    new CardAction(){Title = "14", Value="14", Type= ActionTypes.ImBack},
                }
            };
            return reply as Activity;
        }
    }
}
