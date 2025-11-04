using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabajoTarjeta
{
    public class TiempoFalso : Tiempo
    {
        private DateTime tiempo;

        public TiempoFalso(int year, int month, int day)
        {
            tiempo = new DateTime(year, month, day);
        }

        public TiempoFalso(int year, int month, int day, int hour, int minute)
        {
            tiempo = new DateTime(year, month, day, hour, minute, 0);
        }

        public override DateTime Now()
        {
            return tiempo;
        }

        public void AgregarDias(int cantidad)
        {
            tiempo = tiempo.AddDays(cantidad);
        }

        public void AgregarMinutos(int cantidad)
        {
            tiempo = tiempo.AddMinutes(cantidad);
        }

        public void AgregarHoras(int cantidad)
        {
            tiempo = tiempo.AddHours(cantidad);
        }

        public void EstablecerHora(int hora, int minuto)
        {
            tiempo = new DateTime(tiempo.Year, tiempo.Month, tiempo.Day, hora, minuto, 0);
        }
    }
}