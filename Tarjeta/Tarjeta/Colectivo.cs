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
        /// Soporta trasbordos: si cumple las condiciones, el viaje es gratis
        /// </summary>
        public Boleto PagarCon(Tarjeta tarjeta, Tiempo tiempo)
        {
            const float PRECIO_BOLETO = 1580f;

            float saldoAnterior = tarjeta.Saldo;
            bool pagoExitoso;
            bool esTrasbordo = false;
            float totalAbonado = 0f;

            // Verificar si puede hacer trasbordo
            if (tarjeta.PuedeTrasbordar(Linea, tiempo))
            {
                // Es trasbordo - viaje gratis, no descuenta saldo
                esTrasbordo = true;
                pagoExitoso = true;
            }
            else
            {
                // No es trasbordo - procesar pago normal
                // Si es medio boleto, usar el método con tiempo para aplicar restricciones
                if (tarjeta is MedioBoletoEstudiantil medioBoleto)
                {
                    pagoExitoso = medioBoleto.DescontarSaldo(PRECIO_BOLETO, tiempo);
                }
                // Si es boleto gratuito, usar el método con tiempo para aplicar restricciones
                else if (tarjeta is BoletoGratuitoEstudiantil boletoGratuito)
                {
                    pagoExitoso = boletoGratuito.DescontarSaldo(PRECIO_BOLETO, tiempo);
                }
                // Si es franquicia completa, usar el método con tiempo específico
                else if (tarjeta is FranquiciaCompleta franquicia)
                {
                    pagoExitoso = franquicia.DescontarSaldo(PRECIO_BOLETO, tiempo);
                }
                else
                {
                    // Para tarjetas normales, usar el método con tiempo para aplicar uso frecuente
                    pagoExitoso = tarjeta.DescontarSaldo(PRECIO_BOLETO, tiempo);
                }
            }

            if (!pagoExitoso) return null;

            // Calcular el total abonado
            if (esTrasbordo)
            {
                totalAbonado = 0f; // Trasbordo gratis
            }
            else
            {
                totalAbonado = saldoAnterior - tarjeta.Saldo;
            }

            Boleto boleto = new Boleto(
                tiempo.Now(),
                tarjeta.GetType().Name,
                Linea,
                totalAbonado,
                tarjeta.Saldo,
                tarjeta.Id,
                esTrasbordo
            );

            // Registrar el boleto para futuros trasbordos
            tarjeta.RegistrarBoleto(Linea, tiempo);

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