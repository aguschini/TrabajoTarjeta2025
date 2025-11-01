using System;
using TrabajoTarjeta;

namespace Tarjeta
{
    /// <summary>
    /// Tarjeta de boleto gratuito estudiantil - viaja gratis (paga $0)
    /// </summary>
    public class BoletoGratuitoEstudiantil : Tarjeta
    {
        // Constructor
        public BoletoGratuitoEstudiantil(float saldo, int id) : base(saldo, id)
        {
        }

        /// <summary>
        /// No descuenta nada - viaja gratis
        /// </summary>
        public override bool DescontarSaldo(float monto)
        {
            // No descuenta saldo, siempre retorna true (puede viajar gratis)
            return true;
        }
    }

    /// <summary>
    /// Tarjeta de medio boleto estudiantil - paga la mitad del valor del pasaje
    /// Con limitaciones: 5 minutos entre viajes y máximo 2 viajes con descuento por día
    /// </summary>
    public class MedioBoletoEstudiantil : Tarjeta
    {
        // Atributos para controlar las limitaciones
        private DateTime? ultimoViaje;
        private int viajesHoy;
        private DateTime? fechaUltimoDia;

        // Constantes
        private const int MINUTOS_ENTRE_VIAJES = 5;
        private const int MAX_VIAJES_CON_DESCUENTO = 2;

        // Constructor
        public MedioBoletoEstudiantil(float saldo, int id) : base(saldo, id)
        {
            ultimoViaje = null;
            viajesHoy = 0;
            fechaUltimoDia = null;
        }

        /// <summary>
        /// Verifica si puede usar medio boleto en este momento
        /// </summary>
        private bool PuedeUsarMedioBoleto(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            // Verificar si pasaron 5 minutos desde el último viaje
            if (ultimoViaje.HasValue)
            {
                TimeSpan diferencia = ahora - ultimoViaje.Value;
                if (diferencia.TotalMinutes < MINUTOS_ENTRE_VIAJES)
                {
                    return false; // No pasaron 5 minutos
                }
            }

            // Verificar si es un nuevo día
            if (fechaUltimoDia.HasValue && ahora.Date != fechaUltimoDia.Value.Date)
            {
                // Es un nuevo día, reiniciar contador
                viajesHoy = 0;
            }

            // Verificar si ya usó los 2 viajes con descuento del día
            if (viajesHoy >= MAX_VIAJES_CON_DESCUENTO)
            {
                return false; // Ya usó los 2 viajes con descuento
            }

            return true;
        }

        /// <summary>
        /// Registra el viaje realizado
        /// </summary>
        private void RegistrarViaje(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            // Si es un nuevo día, reiniciar el contador
            if (fechaUltimoDia.HasValue && ahora.Date != fechaUltimoDia.Value.Date)
            {
                viajesHoy = 0;
            }

            ultimoViaje = ahora;
            fechaUltimoDia = ahora;
            viajesHoy++;
        }

        /// <summary>
        /// Descuenta el saldo considerando las limitaciones de medio boleto
        /// Requiere pasar el objeto Tiempo para aplicar restricciones
        /// </summary>
        public bool DescontarSaldo(float monto, Tiempo tiempo)
        {
            // Verificar si puede usar medio boleto
            if (PuedeUsarMedioBoleto(tiempo))
            {
                // Aplica medio boleto (mitad del precio)
                float montoMedioBoleto = monto / 2f;
                bool resultado = base.DescontarSaldo(montoMedioBoleto);

                if (resultado)
                {
                    RegistrarViaje(tiempo);
                }

                return resultado;
            }
            else
            {
                // No puede usar medio boleto, cobra precio completo
                bool resultado = base.DescontarSaldo(monto);

                // Aunque pague completo, se registra el intento
                // (para el contador de 5 minutos)
                if (resultado)
                {
                    ultimoViaje = tiempo.Now();
                }

                return resultado;
            }
        }

        /// <summary>
        /// Sobrecarga del método original para mantener compatibilidad (sin tiempo)
        /// En este caso siempre cobra medio boleto sin restricciones
        /// </summary>
        public override bool DescontarSaldo(float monto)
        {
            // Medio boleto paga la mitad (sin restricciones de tiempo)
            float montoMedioBoleto = monto / 2f;
            return base.DescontarSaldo(montoMedioBoleto);
        }

        /// <summary>
        /// Método público para consultar si puede usar medio boleto (útil para testing)
        /// </summary>
        public bool PuedeUsarDescuento(Tiempo tiempo)
        {
            return PuedeUsarMedioBoleto(tiempo);
        }

        /// <summary>
        /// Obtener la cantidad de viajes realizados hoy (útil para testing)
        /// </summary>
        public int ViajesHoy => viajesHoy;

        /// <summary>
        /// Obtener la fecha del último viaje (útil para testing)
        /// </summary>
        public DateTime? UltimoViaje => ultimoViaje;
    }

    /// <summary>
    /// Tarjeta de franquicia completa (jubilados) - viaja gratis (paga $0)
    /// </summary>
    public class FranquiciaCompleta : Tarjeta
    {
        // Constructor
        public FranquiciaCompleta(float saldo, int id) : base(saldo, id)
        {
        }

        /// <summary>
        /// No descuenta nada - viaja gratis (jubilados)
        /// </summary>
        public override bool DescontarSaldo(float monto)
        {
            // No descuenta saldo, siempre retorna true (puede viajar gratis)
            return true;
        }
    }
}