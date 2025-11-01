using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarjeta
{
    public class Boleto
    {
        //Atributos
        public DateTime Fecha { get; set; }
        public string TipoTarjeta { get; set; }
        public string LineaColectivo { get; set; }
        public float TotalAbonado { get; set; }
        public float SaldoRestante { get; set; }
        public int IdTarjeta { get; set; }

        //Constructor
        public Boleto(DateTime fecha, string tipoTarjeta, string linea, float total, float saldoRestante, int id)
        {
            Fecha = fecha;
            TipoTarjeta = tipoTarjeta;
            LineaColectivo = linea;
            TotalAbonado = total;
            SaldoRestante = saldoRestante;
            IdTarjeta = id;
        }
    }

}


