using System;

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
        public bool PagarCon(Tarjeta tarjeta)
        {
            // Intenta descontar el saldo
            // Cada tipo de tarjeta maneja el descuento según su lógica
            bool pagoExitoso = tarjeta.DescontarSaldo(PRECIO_BOLETO);

            if (pagoExitoso)
            {
                Console.WriteLine($"Pago exitoso en línea {Linea}. Nuevo saldo: ${tarjeta.Saldo}");
            }
            else
            {
                Console.WriteLine($"Pago rechazado en línea {Linea}. Saldo insuficiente: ${tarjeta.Saldo}");
            }

            return pagoExitoso;
        }

        /// <summary>
        /// Método legacy - mantener para compatibilidad con tests anteriores
        /// </summary>
        public void Pagar(Tarjeta tarjeta)
        {
            PagarCon(tarjeta);
        }
    }
}