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
        /// Método principal - retorna Boleto según éxito del pago
        /// Funciona con todas las tarjetas (normal, medio boleto, gratuitas)
        /// </summary>
        public Boleto PagarCon(Tarjeta tarjeta, Tiempo tiempo)
        {
            const float PRECIO_BOLETO = 1580f;

            float saldoAnterior = tarjeta.Saldo;
            bool pagoExitoso;

            // Si es medio boleto, usar el método con tiempo para aplicar restricciones
            if (tarjeta is MedioBoletoEstudiantil medioBoleto)
            {
                pagoExitoso = medioBoleto.DescontarSaldo(PRECIO_BOLETO, tiempo);
            }
            else
            {
                // Para otros tipos de tarjeta, usar el método normal
                pagoExitoso = tarjeta.DescontarSaldo(PRECIO_BOLETO);
            }

            if (!pagoExitoso) return null;

            // El total abonado es la diferencia entre el saldo anterior y el actual
            float totalAbonado = saldoAnterior - tarjeta.Saldo;

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