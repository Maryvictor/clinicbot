using ClinicBot.Common.Models;
using ClinicBot.Common.Models.MedicalAppointment;
using ClinicBot.Common.Models.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicBot.Data
{
    public interface IDataBaseService
    {
        public DbSet<UserModel> User { get; set; }
        DbSet<AvaliacaoModel> Avaliacao { get; set; }
        DbSet<MedicalAppointmentModel> MedicalAppointments { get; set; }
        Task<bool> SaveAsync();

    }
}
