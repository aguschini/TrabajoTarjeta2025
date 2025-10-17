using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarjeta
{
    public class Colectivo
    {
        public string Linea { get; private set; }
        private const int PRECIO_BOLETO = 1580;

        public Colectivo(string linea)
        {
            Linea = linea;
        }

        public Boleto pagarCon(Tarjeta tarjeta)
        {
            if (tarjeta == null) return null;

            bool pagado = tarjeta.Descontar(PRECIO_BOLETO);
            if (pagado)
            {
                return new Boleto(PRECIO_BOLETO);
            }

            return null;
        }
    }
}


