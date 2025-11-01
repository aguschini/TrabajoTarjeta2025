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
    }
}