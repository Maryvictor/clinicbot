
using Microsoft.Bot.Builder.AI.Luis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicBot.Infraestrutura.LUIS
{
    public interface ILuisService
    {
        public LuisRecognizer _luisRecognizer { get; set; }

    }
}
