using System;
using TrabajoTarjeta;

namespace Tarjeta
{
    /// <summary>
    /// Tarjeta de boleto gratuito estudiantil - viaja gratis (paga $0)
    /// Limitación: máximo 2 viajes gratuitos por día
    /// Restricción horaria: Lunes a viernes de 6:00 a 22:00
    /// </summary>
    public class BoletoGratuitoEstudiantil : Tarjeta
    {
        // Atributos para controlar las limitaciones
        private int viajesGratuitosHoy;
        private DateTime? fechaUltimoDia;

        // Constantes
        private const int MAX_VIAJES_GRATUITOS = 2;

        // Constructor
        public BoletoGratuitoEstudiantil(float saldo, int id) : base(saldo, id)
        {
            viajesGratuitosHoy = 0;
            fechaUltimoDia = null;
        }

        /// <summary>
        /// NO aplica descuento por uso frecuente a franquicias
        /// </summary>
        protected override float CalcularMontoConDescuentoFrecuente(float montoBase, Tiempo tiempo)
        {
            return montoBase; // No aplica descuento por uso frecuente
        }

        /// <summary>
        /// Verifica si el horario y día son válidos para usar franquicias
        /// Lunes a viernes de 6:00 a 22:00
        /// </summary>
        private bool HorarioValido(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            // Verificar día de la semana (lunes=1 a viernes=5)
            if (ahora.DayOfWeek == DayOfWeek.Saturday ||
                ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            // Verificar horario (6:00 a 22:00)
            int hora = ahora.Hour;
            if (hora < 6 || hora >= 22)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica si puede usar boleto gratuito en este momento
        /// </summary>
        private bool PuedeUsarBoletoGratuito(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            // Verificar si es un nuevo día
            if (fechaUltimoDia.HasValue && ahora.Date != fechaUltimoDia.Value.Date)
            {
                // Es un nuevo día, reiniciar contador
                viajesGratuitosHoy = 0;
            }

            // Verificar si ya usó los 2 viajes gratuitos del día
            if (viajesGratuitosHoy >= MAX_VIAJES_GRATUITOS)
            {
                return false; // Ya usó los 2 viajes gratuitos
            }

            return true;
        }

        /// <summary>
        /// Registra el viaje gratuito realizado
        /// </summary>
        private void RegistrarViajeGratuito(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            // Si es un nuevo día, reiniciar el contador
            if (fechaUltimoDia.HasValue && ahora.Date != fechaUltimoDia.Value.Date)
            {
                viajesGratuitosHoy = 0;
            }

            fechaUltimoDia = ahora;
            viajesGratuitosHoy++;
        }

        /// <summary>
        /// Descuenta el saldo considerando las limitaciones de boleto gratuito
        /// Requiere pasar el objeto Tiempo para aplicar restricciones
        /// </summary>
        public new bool DescontarSaldo(float monto, Tiempo tiempo)
        {
            // VALIDAR HORARIO PRIMERO
            if (!HorarioValido(tiempo))
            {
                return false; // No puede usar franquicia fuera de horario
            }

            DateTime ahora = tiempo.Now();

            // Actualizar contador de día si es necesario
            if (fechaUltimoDia.HasValue && ahora.Date != fechaUltimoDia.Value.Date)
            {
                viajesGratuitosHoy = 0;
            }

            // Determinar si puede viajar gratis o debe pagar completo
            if (viajesGratuitosHoy < MAX_VIAJES_GRATUITOS)
            {
                // Viaja gratis - no descuenta saldo
                RegistrarViajeGratuito(tiempo);
                return true;
            }
            else
            {
                // Ya usó los 2 viajes gratuitos del día, cobra precio completo
                // Usar el método base sin tiempo para evitar descuento frecuente
                return base.DescontarSaldo(monto);
            }
        }

        /// <summary>
        /// Sobrecarga del método original para mantener compatibilidad (sin tiempo)
        /// En este caso siempre viaja gratis sin restricciones
        /// </summary>
        public override bool DescontarSaldo(float monto)
        {
            // No descuenta saldo, siempre retorna true (puede viajar gratis)
            return true;
        }

        /// <summary>
        /// Método público para consultar si puede usar boleto gratuito (útil para testing)
        /// </summary>
        public bool PuedeUsarDescuento(Tiempo tiempo)
        {
            return PuedeUsarBoletoGratuito(tiempo);
        }

        /// <summary>
        /// Obtener la cantidad de viajes gratuitos realizados hoy (útil para testing)
        /// </summary>
        public int ViajesGratuitosHoy => viajesGratuitosHoy;
    }

    /// <summary>
    /// Tarjeta de medio boleto estudiantil - paga la mitad del valor del pasaje
    /// Con limitaciones: 5 minutos entre viajes y máximo 2 viajes con descuento por día
    /// Restricción horaria: Lunes a viernes de 6:00 a 22:00
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
        /// NO aplica descuento por uso frecuente a franquicias
        /// </summary>
        protected override float CalcularMontoConDescuentoFrecuente(float montoBase, Tiempo tiempo)
        {
            return montoBase; // No aplica descuento por uso frecuente
        }

        /// <summary>
        /// Verifica si el horario y día son válidos para usar franquicias
        /// Lunes a viernes de 6:00 a 22:00
        /// </summary>
        private bool HorarioValido(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            // Verificar día de la semana (lunes=1 a viernes=5)
            if (ahora.DayOfWeek == DayOfWeek.Saturday ||
                ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            // Verificar horario (6:00 a 22:00)
            int hora = ahora.Hour;
            if (hora < 6 || hora >= 22)
            {
                return false;
            }

            return true;
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
        /// CAMBIO IMPORTANTE: Ahora RECHAZA el viaje si no pasaron 5 minutos
        /// </summary>
        public new bool DescontarSaldo(float monto, Tiempo tiempo)
        {
            // VALIDAR HORARIO PRIMERO
            if (!HorarioValido(tiempo))
            {
                return false; // No puede usar franquicia fuera de horario
            }

            DateTime ahora = tiempo.Now();

            // Verificar si es un nuevo día
            if (fechaUltimoDia.HasValue && ahora.Date != fechaUltimoDia.Value.Date)
            {
                viajesHoy = 0;
            }

            // CAMBIO: Verificar restricción de 5 minutos ANTES de procesar el pago
            if (ultimoViaje.HasValue)
            {
                TimeSpan diferencia = ahora - ultimoViaje.Value;
                if (diferencia.TotalMinutes < MINUTOS_ENTRE_VIAJES)
                {
                    // No pasaron 5 minutos - RECHAZAR el viaje
                    return false;
                }
            }

            // Determinar el monto a pagar según cantidad de viajes del día
            float montoAPagar;
            if (viajesHoy < MAX_VIAJES_CON_DESCUENTO)
            {
                montoAPagar = monto / 2f; // Medio boleto
            }
            else
            {
                montoAPagar = monto; // Precio completo (tercer viaje en adelante)
            }

            // Intentar el pago
            bool resultado = base.DescontarSaldo(montoAPagar);

            if (resultado)
            {
                RegistrarViaje(tiempo);
            }

            return resultado;
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
    /// Restricción horaria: Lunes a viernes de 6:00 a 22:00
    /// </summary>
    public class FranquiciaCompleta : Tarjeta
    {
        // Constructor
        public FranquiciaCompleta(float saldo, int id) : base(saldo, id)
        {
        }

        /// <summary>
        /// NO aplica descuento por uso frecuente a franquicias
        /// </summary>
        protected override float CalcularMontoConDescuentoFrecuente(float montoBase, Tiempo tiempo)
        {
            return montoBase; // No aplica descuento por uso frecuente
        }

        /// <summary>
        /// Verifica si el horario y día son válidos para usar franquicias
        /// Lunes a viernes de 6:00 a 22:00
        /// </summary>
        private bool HorarioValido(Tiempo tiempo)
        {
            DateTime ahora = tiempo.Now();

            // Verificar día de la semana (lunes=1 a viernes=5)
            if (ahora.DayOfWeek == DayOfWeek.Saturday ||
                ahora.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            // Verificar horario (6:00 a 22:00)
            int hora = ahora.Hour;
            if (hora < 6 || hora >= 22)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sobrecarga CON tiempo - viaja gratis sin descontar saldo
        /// Con restricción horaria
        /// </summary>
        public new bool DescontarSaldo(float monto, Tiempo tiempo)
        {
            // VALIDAR HORARIO
            if (!HorarioValido(tiempo))
            {
                return false;
            }

            // No descuenta saldo, siempre retorna true (puede viajar gratis)
            return true;
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