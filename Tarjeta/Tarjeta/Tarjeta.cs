using System;
using System.Collections.Generic;
using TrabajoTarjeta;

namespace Tarjeta
{
    public class Tarjeta
    {
        // Atributos
        public float Saldo { get; set; }
        public int Id { get; set; }
        public float SaldoPendiente { get; protected set; }

        // Atributos para boleto de uso frecuente
        private int viajesMes;
        private DateTime? ultimoMesRegistrado;

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
            viajesMes = 0;
            ultimoMesRegistrado = null;
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

        /// <summary>
        /// Actualiza el contador de viajes mensuales
        /// Si es un nuevo mes, reinicia el contador
        /// </summary>
        private void ActualizarContadorMensual(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            // Si es un nuevo mes, reiniciar contador
            if (!ultimoMesRegistrado.HasValue ||
                ahora.Year != ultimoMesRegistrado.Value.Year ||
                ahora.Month != ultimoMesRegistrado.Value.Month)
            {
                viajesMes = 0;
                ultimoMesRegistrado = ahora;
            }
        }

        /// <summary>
        /// Calcula el monto a pagar aplicando descuento por uso frecuente
        /// Solo aplica a tarjetas normales (no franquicias)
        /// - Del viaje 1 al 29: Tarifa normal
        /// - Del viaje 30 al 59: 20% de descuento
        /// - Del viaje 60 al 80: 25% de descuento
        /// - Del viaje 81 en adelante: Tarifa normal
        /// </summary>
        protected virtual float CalcularMontoConDescuentoFrecuente(float montoBase, Tiempo tiempo)
        {
            ActualizarContadorMensual(tiempo);

            // Aplicar descuentos según cantidad de viajes
            if (viajesMes >= 30 && viajesMes < 60)
            {
                return montoBase * 0.80f; // 20% descuento
            }
            else if (viajesMes >= 60 && viajesMes < 81)
            {
                return montoBase * 0.75f; // 25% descuento
            }

            return montoBase; // Sin descuento (viajes 1-29 y 81+)
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

        /// <summary>
        /// Sobrecarga que acepta Tiempo para aplicar uso frecuente
        /// </summary>
        public virtual bool DescontarSaldo(float monto, Tiempo tiempo)
        {
            // Calcular monto con descuento por uso frecuente
            float montoFinal = CalcularMontoConDescuentoFrecuente(monto, tiempo);

            if (!PuedeDescontar(montoFinal))
            {
                return false;
            }

            Saldo -= montoFinal;
            viajesMes++; // Incrementar contador de viajes

            // Después de descontar, intentar acreditar saldo pendiente
            AcreditarCarga();

            return true;
        }

        // Propiedades públicas para testing
        public int ViajesMes => viajesMes;
    }
}