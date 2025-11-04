using System;
using TrabajoTarjeta;

namespace Tarjeta
{
    /// <summary>
    /// Colectivo interurbano que viaja fuera de la ciudad
    /// Admite todas las franquicias y sigue los mismos criterios que las líneas urbanas
    /// Tarifa: $3000
    /// </summary>
    public class ColectivoInterurbano : Colectivo
    {
        // Constante de precio del boleto interurbano
        private const float PRECIO_BOLETO_INTERURBANO = 3000f;

        // Constructor
        public ColectivoInterurbano(string linea) : base(linea)
        {
        }

        /// <summary>
        /// Método principal - retorna Boleto según éxito del pago
        /// Usa tarifa interurbana de $3000
        /// Funciona con todas las tarjetas (normal, medio boleto, gratuitas)
        /// </summary>
        public new Boleto PagarCon(Tarjeta tarjeta, Tiempo tiempo)
        {
            float saldoAnterior = tarjeta.Saldo;
            bool pagoExitoso;

            // Si es medio boleto, usar el método con tiempo para aplicar restricciones
            if (tarjeta is MedioBoletoEstudiantil medioBoleto)
            {
                pagoExitoso = medioBoleto.DescontarSaldo(PRECIO_BOLETO_INTERURBANO, tiempo);
            }
            // Si es boleto gratuito, usar el método con tiempo para aplicar restricciones
            else if (tarjeta is BoletoGratuitoEstudiantil boletoGratuito)
            {
                pagoExitoso = boletoGratuito.DescontarSaldo(PRECIO_BOLETO_INTERURBANO, tiempo);
            }
            // Si es franquicia completa, usar el método con tiempo específico
            else if (tarjeta is FranquiciaCompleta franquicia)
            {
                pagoExitoso = franquicia.DescontarSaldo(PRECIO_BOLETO_INTERURBANO, tiempo);
            }
            else
            {
                // Para tarjetas normales, usar el método con tiempo para aplicar uso frecuente
                pagoExitoso = tarjeta.DescontarSaldo(PRECIO_BOLETO_INTERURBANO, tiempo);
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
    }
}
