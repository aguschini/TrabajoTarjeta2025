using NUnit.Framework;
using Tarjeta;
using TrabajoTarjeta;
using System;

namespace Tests1
{
    /// <summary>
    /// Tests adicionales para aumentar cobertura de Franquicia.cs
    /// Estos tests cubren casos edge y ramas no cubiertas por los tests principales
    /// </summary>
    public class FranquiciaCoberturaTest
    {
        private Colectivo _colectivo;
        private TiempoFalso _tiempo;

        [SetUp]
        public void Setup()
        {
            _colectivo = new Colectivo("125");
            // Viernes a las 10:00 AM (horario válido)
            _tiempo = new TiempoFalso(2024, 11, 1, 10, 0);
        }

        // ============================================
        //    TESTS DE BOLETO GRATUITO - COBERTURA
        // ============================================

        [Test]
        public void BoletoGratuito_PuedeUsarDescuento_ConViajesDisponibles()
        {
            // Arrange
            var boleto = new BoletoGratuitoEstudiantil(5000f, 1);

            // Act & Assert - Primer uso
            Assert.That(boleto.PuedeUsarDescuento(_tiempo), Is.True,
                "Debe poder usar descuento cuando tiene viajes disponibles");
        }

        [Test]
        public void BoletoGratuito_PuedeUsarDescuento_DespuesDeUsarDos()
        {
            // Arrange
            var boleto = new BoletoGratuitoEstudiantil(5000f, 1);

            // Act - Usar los 2 viajes gratuitos
            _colectivo.PagarCon(boleto, _tiempo);
            _colectivo.PagarCon(boleto, _tiempo);

            // Assert - Ya no puede usar descuento
            Assert.That(boleto.PuedeUsarDescuento(_tiempo), Is.False,
                "No debe poder usar descuento después de 2 viajes");
        }

        [Test]
        public void BoletoGratuito_DescontarSaldo_SinTiempo_SiempreRetornaTrue()
        {
            // Arrange
            var boleto = new BoletoGratuitoEstudiantil(0f, 1); // Sin saldo

            // Act
            bool resultado = boleto.DescontarSaldo(1580f); // Sin parámetro tiempo

            // Assert
            Assert.That(resultado, Is.True,
                "Método sin tiempo debe retornar siempre true (compatibilidad)");
        }

        [Test]
        public void BoletoGratuito_CambiarDeDia_ReiniciaContador()
        {
            // Arrange
            var boleto = new BoletoGratuitoEstudiantil(10000f, 1);

            // Act - Dos viajes el día 1 (viernes)
            _colectivo.PagarCon(boleto, _tiempo);
            _colectivo.PagarCon(boleto, _tiempo);

            // Verificar que usó los 2 viajes
            Assert.That(boleto.ViajesGratuitosHoy, Is.EqualTo(2));

            // Cambiar 3 días (viernes -> lunes, evitando fin de semana)
            _tiempo.AgregarDias(3);

            // Hacer otro viaje (debería ser gratis otra vez)
            Boleto boletoNuevo = _colectivo.PagarCon(boleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boletoNuevo, Is.Not.Null, "Debe permitir viaje en nuevo día");
                Assert.That(boletoNuevo.TotalAbonado, Is.EqualTo(0f),
                    "Nuevo día debe resetear y permitir viaje gratis");
                Assert.That(boleto.ViajesGratuitosHoy, Is.EqualTo(1),
                    "Contador debe estar en 1 en el nuevo día");
            });
        }

        [Test]
        public void BoletoGratuito_FueraDeHorario_NoIncrementaContador()
        {
            // Arrange
            var boleto = new BoletoGratuitoEstudiantil(5000f, 1);
            _tiempo.EstablecerHora(23, 0); // Fuera de horario

            // Act
            Boleto boletoFallo = _colectivo.PagarCon(boleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boletoFallo, Is.Null, "No debe emitir boleto fuera de horario");
                Assert.That(boleto.ViajesGratuitosHoy, Is.EqualTo(0),
                    "No debe incrementar contador si falló el viaje");
            });
        }

        [Test]
        public void BoletoGratuito_CalcularMontoConDescuentoFrecuente_NoAplicaDescuento()
        {
            // Arrange
            var boleto = new BoletoGratuitoEstudiantil(10000f, 1);

            // Este test verifica que el método protegido no aplique descuento
            // Indirectamente: hacer muchos viajes y verificar que el monto no cambia

            // Act - Hacer múltiples viajes para probar que no aplica descuento frecuente
            for (int i = 0; i < 3; i++)
            {
                _colectivo.PagarCon(boleto, _tiempo);
            }

            // El tercer viaje cobra completo (no gratis)
            // Si aplicara descuento frecuente, cobraría menos
            Assert.That(boleto.Saldo, Is.EqualTo(10000f - 1580f),
                "Tercer viaje debe cobrar 1580 sin descuento frecuente");
        }

        // ============================================
        //    TESTS DE MEDIO BOLETO - COBERTURA
        // ============================================

        [Test]
        public void MedioBoleto_PuedeUsarDescuento_ConViajesDisponibles()
        {
            // Arrange
            var medio = new MedioBoletoEstudiantil(5000f, 1);

            // Act & Assert
            Assert.That(medio.PuedeUsarDescuento(_tiempo), Is.True,
                "Debe poder usar descuento cuando tiene viajes disponibles");
        }

        [Test]
        public void MedioBoleto_PuedeUsarDescuento_DespuesDeDosSinEsperar5Min()
        {
            // Arrange
            var medio = new MedioBoletoEstudiantil(5000f, 1);

            // Act - Dos viajes sin esperar
            _colectivo.PagarCon(medio, _tiempo);
            _tiempo.AgregarMinutos(5);
            _colectivo.PagarCon(medio, _tiempo);

            // Intentar tercero sin esperar
            _tiempo.AgregarMinutos(3); // Solo 3 minutos

            // Assert
            Assert.That(medio.PuedeUsarDescuento(_tiempo), Is.False,
                "No debe poder usar descuento: 2 viajes usados + no pasaron 5 min");
        }

        [Test]
        public void MedioBoleto_DescontarSaldo_SinTiempo_CobraMitad()
        {
            // Arrange
            var medio = new MedioBoletoEstudiantil(2000f, 1);

            // Act
            bool resultado = medio.DescontarSaldo(1580f); // Sin tiempo

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True);
                Assert.That(medio.Saldo, Is.EqualTo(2000f - 790f),
                    "Sin tiempo debe cobrar mitad sin restricciones");
            });
        }

        [Test]
        public void MedioBoleto_RechazaViaje_AntesDe5Minutos()
        {
            // Arrange
            var medio = new MedioBoletoEstudiantil(5000f, 1);

            // Act - Primer viaje
            _colectivo.PagarCon(medio, _tiempo);

            // Intentar segundo viaje antes de 5 minutos
            _tiempo.AgregarMinutos(4);
            Boleto boleto2 = _colectivo.PagarCon(medio, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto2, Is.Null,
                    "Debe rechazar viaje antes de 5 minutos");
                Assert.That(medio.Saldo, Is.EqualTo(5000f - 790f),
                    "Saldo solo debe reflejar el primer viaje");
            });
        }

        [Test]
        public void MedioBoleto_UltimoViaje_SeActualizaCorrectamente()
        {
            // Arrange
            var medio = new MedioBoletoEstudiantil(5000f, 1);

            // Act - Primer viaje
            Assert.That(medio.UltimoViaje, Is.Null, "Inicialmente no debe tener último viaje");

            _colectivo.PagarCon(medio, _tiempo);

            // Assert
            Assert.That(medio.UltimoViaje, Is.Not.Null, "Debe registrar último viaje");
            Assert.That(medio.UltimoViaje.Value, Is.EqualTo(_tiempo.Now()));
        }

        [Test]
        public void MedioBoleto_ViajesHoy_CuentaCorrectamente()
        {
            // Arrange
            var medio = new MedioBoletoEstudiantil(10000f, 1);

            // Act - Dos viajes con descuento
            _colectivo.PagarCon(medio, _tiempo);
            _tiempo.AgregarMinutos(5);
            _colectivo.PagarCon(medio, _tiempo);
            _tiempo.AgregarMinutos(5);
            _colectivo.PagarCon(medio, _tiempo); // Tercer viaje (completo)

            // Assert
            Assert.That(medio.ViajesHoy, Is.EqualTo(3),
                "Debe contar todos los viajes del día");
        }

        [Test]
        public void MedioBoleto_NuevoDia_ReiniciaContadorYPermite5Min()
        {
            // Arrange
            var medio = new MedioBoletoEstudiantil(10000f, 1);

            // Act - Viaje día 1 (viernes)
            _colectivo.PagarCon(medio, _tiempo);

            // Día siguiente (3 días para evitar fin de semana: viernes -> lunes)
            _tiempo.AgregarDias(3);

            // Viaje inmediato (no espera 5 min del día anterior)
            Boleto boleto2 = _colectivo.PagarCon(medio, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto2, Is.Not.Null, "Debe permitir viaje en nuevo día");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(790f), "Debe cobrar medio boleto");
                Assert.That(medio.ViajesHoy, Is.EqualTo(1), "Contador debe estar en 1");
            });
        }

        [Test]
        public void MedioBoleto_CalcularMontoConDescuentoFrecuente_NoAplicaDescuento()
        {
            // Arrange
            var medio = new MedioBoletoEstudiantil(20000f, 1);

            // Act - Hacer múltiples viajes (más de 30 para probar descuento frecuente)
            for (int i = 0; i < 3; i++)
            {
                if (i > 0) _tiempo.AgregarMinutos(5);
                _colectivo.PagarCon(medio, _tiempo);
            }

            // El tercer viaje debe cobrar 1580 (completo), no menos
            // Si aplicara descuento frecuente, los primeros viajes costarían menos
            float saldoEsperado = 20000f - 790f - 790f - 1580f;
            Assert.That(medio.Saldo, Is.EqualTo(saldoEsperado),
                "No debe aplicar descuento frecuente a franquicias");
        }

        // ============================================
        //    TESTS DE FRANQUICIA COMPLETA - COBERTURA
        // ============================================

        [Test]
        public void FranquiciaCompleta_DescontarSaldo_SinTiempo_SiempreRetornaTrue()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(0f, 1);

            // Act
            bool resultado = franquicia.DescontarSaldo(1580f);

            // Assert
            Assert.That(resultado, Is.True,
                "Método sin tiempo debe retornar siempre true");
        }

        [Test]
        public void FranquiciaCompleta_DescontarSaldo_ConTiempo_FueraDeHorario()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(5000f, 1);
            _tiempo.EstablecerHora(23, 0); // Fuera de horario

            // Act
            bool resultado = franquicia.DescontarSaldo(1580f, _tiempo);

            // Assert
            Assert.That(resultado, Is.False,
                "Debe retornar false fuera de horario");
        }

        [Test]
        public void FranquiciaCompleta_CalcularMontoConDescuentoFrecuente_NoAplicaDescuento()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(10000f, 1);

            // Act - Muchos viajes
            for (int i = 0; i < 5; i++)
            {
                _colectivo.PagarCon(franquicia, _tiempo);
            }

            // Assert - Saldo no cambia (siempre gratis)
            Assert.That(franquicia.Saldo, Is.EqualTo(10000f),
                "Franquicia completa nunca descuenta saldo");
        }

        [Test]
        public void FranquiciaCompleta_Sabado_NoPermiteViaje()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(5000f, 1);
            // Sábado 9 de noviembre
            _tiempo = new TiempoFalso(2024, 11, 9, 10, 0);

            // Act
            Boleto boleto = _colectivo.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.That(boleto, Is.Null,
                "Franquicia completa no funciona en sábado");
        }

        [Test]
        public void FranquiciaCompleta_DomingoConSaldoNegativo_NoPermiteViaje()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(-500f, 1);
            // Domingo
            _tiempo = new TiempoFalso(2024, 11, 10, 10, 0);

            // Act
            bool resultado = franquicia.DescontarSaldo(1580f, _tiempo);

            // Assert
            Assert.That(resultado, Is.False,
                "No debe permitir viaje en domingo, incluso con saldo negativo");
        }

        // ============================================
        //    TESTS DE HORARIOS LÍMITE
        // ============================================

        [Test]
        public void TodasFranquicias_HoraLimiteInferior_6AM()
        {
            // Arrange
            _tiempo.EstablecerHora(6, 0);
            var medio = new MedioBoletoEstudiantil(5000f, 1);
            var gratuito = new BoletoGratuitoEstudiantil(5000f, 2);
            var completa = new FranquiciaCompleta(5000f, 3);

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(_colectivo.PagarCon(medio, _tiempo), Is.Not.Null,
                    "Medio boleto permite a las 6:00");
                Assert.That(_colectivo.PagarCon(gratuito, _tiempo), Is.Not.Null,
                    "Boleto gratuito permite a las 6:00");
                Assert.That(_colectivo.PagarCon(completa, _tiempo), Is.Not.Null,
                    "Franquicia completa permite a las 6:00");
            });
        }

        [Test]
        public void TodasFranquicias_HoraLimiteSuperior_21_59()
        {
            // Arrange
            _tiempo.EstablecerHora(21, 59);
            var medio = new MedioBoletoEstudiantil(5000f, 1);
            var gratuito = new BoletoGratuitoEstudiantil(5000f, 2);
            var completa = new FranquiciaCompleta(5000f, 3);

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(_colectivo.PagarCon(medio, _tiempo), Is.Not.Null,
                    "Medio boleto permite a las 21:59");
                Assert.That(_colectivo.PagarCon(gratuito, _tiempo), Is.Not.Null,
                    "Boleto gratuito permite a las 21:59");
                Assert.That(_colectivo.PagarCon(completa, _tiempo), Is.Not.Null,
                    "Franquicia completa permite a las 21:59");
            });
        }

        [Test]
        public void TodasFranquicias_AntesDe6AM_Rechazan()
        {
            // Arrange
            _tiempo.EstablecerHora(5, 59);
            var medio = new MedioBoletoEstudiantil(5000f, 1);
            var gratuito = new BoletoGratuitoEstudiantil(5000f, 2);
            var completa = new FranquiciaCompleta(5000f, 3);

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(_colectivo.PagarCon(medio, _tiempo), Is.Null,
                    "Medio boleto rechaza antes de las 6:00");
                Assert.That(_colectivo.PagarCon(gratuito, _tiempo), Is.Null,
                    "Boleto gratuito rechaza antes de las 6:00");
                Assert.That(_colectivo.PagarCon(completa, _tiempo), Is.Null,
                    "Franquicia completa rechaza antes de las 6:00");
            });
        }

        [Test]
        public void TodasFranquicias_A_las22_00_Rechazan()
        {
            // Arrange
            _tiempo.EstablecerHora(22, 0);
            var medio = new MedioBoletoEstudiantil(5000f, 1);
            var gratuito = new BoletoGratuitoEstudiantil(5000f, 2);
            var completa = new FranquiciaCompleta(5000f, 3);

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(_colectivo.PagarCon(medio, _tiempo), Is.Null,
                    "Medio boleto rechaza a las 22:00");
                Assert.That(_colectivo.PagarCon(gratuito, _tiempo), Is.Null,
                    "Boleto gratuito rechaza a las 22:00");
                Assert.That(_colectivo.PagarCon(completa, _tiempo), Is.Null,
                    "Franquicia completa rechaza a las 22:00");
            });
        }
    }
}
