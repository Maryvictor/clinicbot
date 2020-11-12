using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicBot.Common.Models
{
    public class AvaliacaoModel
    {
        public string id { get; set; }
        public string idUser { get; set; }
        public string avaliacao { get; set; }
        public DateTime registerDate { get; set; }

    }
}
