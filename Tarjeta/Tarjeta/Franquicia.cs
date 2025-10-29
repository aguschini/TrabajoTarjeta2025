using System;

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
    /// </summary>
    public class MedioBoletoEstudiantil : Tarjeta
    {
        // Constructor
        public MedioBoletoEstudiantil(float saldo, int id) : base(saldo, id)
        {
        }

        /// <summary>
        /// Descuenta la mitad del valor del boleto
        /// </summary>
        public override bool DescontarSaldo(float monto)
        {
            // Medio boleto paga la mitad
            float montoMedioBoleto = monto / 2f;

            // Llama al método base con la mitad del monto
            return base.DescontarSaldo(montoMedioBoleto);
        }
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
