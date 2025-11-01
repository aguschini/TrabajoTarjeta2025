using System;
using TrabajoTarjeta;

namespace Tarjeta
{
    public class Colectivo
    {
        // Atributos
        public string Linea { get; set; }

        // Constante de precio del boleto
        private const float PRECIO_BOLETO = 1580f;

        // Constructor
        public Colectivo(string linea)
        {
            Linea = linea;
        }

        /// <summary>
        /// Método principal - retorna bool según éxito del pago
        /// Funciona con todas las tarjetas (normal, medio boleto, gratuitas)
        /// </summary>
        public Boleto PagarCon(Tarjeta tarjeta, Tiempo tiempo)
        {
            const float PRECIO_BOLETO = 1580f;
            float totalAbonado = tarjeta is MedioBoletoEstudiantil ? PRECIO_BOLETO / 2f :
                                 tarjeta is BoletoGratuitoEstudiantil or FranquiciaCompleta ? 0f :
                                 PRECIO_BOLETO;

            bool pagoExitoso = tarjeta.DescontarSaldo(totalAbonado);
            if (!pagoExitoso) return null;

            Boleto boleto = new Boleto(
                tiempo.Now(),
                tarjeta.GetType().Name,
                Linea,
                totalAbonado,
                tarjeta.Saldo,
                tarjeta.Id
            );

            return boleto;
        }


        /// <summary>
        /// Método legacy - mantener para compatibilidad con tests anteriores
        /// </summary>
        public void Pagar(Tarjeta tarjeta, Tiempo tiempo)
        {
            PagarCon(tarjeta, tiempo);
        }

    }
}

