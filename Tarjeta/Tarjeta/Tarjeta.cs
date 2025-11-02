using System;
using System.Collections.Generic;

namespace Tarjeta
{
    public class Tarjeta
    {
        // Atributos
        public float Saldo { get; set; }
        public int Id { get; set; }
        public float SaldoPendiente { get; protected set; }

        // Constantes
        protected const float SALDO_MINIMO = -1200f;
        protected const float SALDO_MAXIMO = 56000f;

        // Cargas permitidas
        protected List<float> cargasPermitidas = new List<float>
        {
            2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000
        };

        // Constructor
        public Tarjeta(float saldo, int id)
        {
            Saldo = saldo;
            Id = id;
            SaldoPendiente = 0f;
        }

        /// <summary>
        /// Acredita el saldo pendiente a la tarjeta hasta el máximo permitido
        /// </summary>
        public void AcreditarCarga()
        {
            if (SaldoPendiente <= 0)
            {
                return; // No hay saldo pendiente
            }

            // Calcular cuánto se puede acreditar
            float espacioDisponible = SALDO_MAXIMO - Saldo;

            if (espacioDisponible <= 0)
            {
                return; // La tarjeta ya está al máximo
            }

            // Acreditar lo que se pueda
            float montoAAcreditar = Math.Min(SaldoPendiente, espacioDisponible);

            Saldo += montoAAcreditar;
            SaldoPendiente -= montoAAcreditar;
        }

        // Método cargar
        public virtual void Cargar(float cantidad)
        {
            // Validar que la cantidad esté en la lista de cargas permitidas
            if (!cargasPermitidas.Contains(cantidad))
            {
                Console.WriteLine("Carga no permitida. Las cargas permitidas son: 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000.");
                return;
            }

            // Si hay saldo negativo, primero se descuenta de la carga
            if (Saldo < 0)
            {
                float saldoNegativo = Math.Abs(Saldo);
                float cargarReal = cantidad - saldoNegativo;

                if (cargarReal > SALDO_MAXIMO)
                {
                    // La carga excede el máximo después de cubrir el negativo
                    Saldo = SALDO_MAXIMO;
                    SaldoPendiente += cargarReal - SALDO_MAXIMO;
                    Console.WriteLine($"Se descontó el saldo negativo. Saldo: {Saldo}. Saldo pendiente: {SaldoPendiente}");
                    return;
                }

                Saldo = cargarReal;
                Console.WriteLine($"Se descontó el saldo negativo. Nuevo saldo: {Saldo}");
                return;
            }

            // Verificar si la carga excede el límite
            if (Saldo + cantidad > SALDO_MAXIMO)
            {
                // Calcular cuánto se puede acreditar y cuánto queda pendiente
                float espacioDisponible = SALDO_MAXIMO - Saldo;
                float excedente = cantidad - espacioDisponible;

                Saldo = SALDO_MAXIMO;
                SaldoPendiente += excedente;

                Console.WriteLine($"Carga parcial. Saldo: {Saldo}. Saldo pendiente: {SaldoPendiente}");
                return;
            }

            // Sumar la cantidad al saldo
            Saldo += cantidad;
            Console.WriteLine($"Carga exitosa. El nuevo saldo es: {Saldo}");
        }

        // Método para pagar (virtual para permitir override en clases hijas)
        public virtual bool PuedeDescontar(float monto)
        {
            // Puede descontar si después del descuento el saldo no queda menor al permitido
            return (Saldo - monto) >= SALDO_MINIMO;
        }

        public virtual bool DescontarSaldo(float monto)
        {
            if (!PuedeDescontar(monto))
            {
                return false;
            }

            Saldo -= monto;

            // Después de descontar, intentar acreditar saldo pendiente
            AcreditarCarga();

            return true;
        }
    }
}