using NUnit.Framework;
using Tarjeta;
using TrabajoTarjeta;
using System;

namespace Tests1
{
    /// <summary>
    /// Tests para la funcionalidad de trasbordos (Iteración 5)
    /// Un usuario puede trasbordar sin costo si:
    /// - Es dentro de 1 hora desde el primer boleto
    /// - Es a una línea diferente
    /// - Es de lunes a sábado de 7:00 a 22:00
    /// </summary>
    public class TrasbordoTest
    {
        private Colectivo _colectivo125;
        private Colectivo _colectivo140;
        private Colectivo _colectivoK;
        private TiempoFalso _tiempo;

        [SetUp]
        public void Setup()
        {
            _colectivo125 = new Colectivo("125");
            _colectivo140 = new Colectivo("140");
            _colectivoK = new Colectivo("K");
            // Lunes a las 10:00 AM
            _tiempo = new TiempoFalso(2024, 11, 4, 10, 0);
        }

        // ============================================
        //    TESTS BÁSICOS DE TRASBORDO
        // ============================================

        [Test]
        public void Trasbordo_DentroDeUnaHora_LineaDiferente_DebeSer_Gratuito()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);

            // Act - Primer viaje (línea 125)
            Boleto boleto1 = _colectivo125.PagarCon(tarjeta, _tiempo);
            float saldoDespuesPrimero = tarjeta.Saldo;

            // Avanzar 30 minutos
            _tiempo.AgregarMinutos(30);

            // Act - Segundo viaje (línea 140 - trasbordo)
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1, Is.Not.Null, "Primer boleto debe emitirse");
                Assert.That(boleto1.EsTrasbordo, Is.False, "Primer viaje no es trasbordo");
                Assert.That(boleto1.TotalAbonado, Is.EqualTo(1580f), "Primer viaje cobra normal");

                Assert.That(boleto2, Is.Not.Null, "Segundo boleto debe emitirse");
                Assert.That(boleto2.EsTrasbordo, Is.True, "Segundo viaje es trasbordo");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f), "Trasbordo es gratis");
                Assert.That(tarjeta.Saldo, Is.EqualTo(saldoDespuesPrimero), "Saldo no cambia en trasbordo");
            });
        }

        [Test]
        public void Trasbordo_MismaLinea_DebeCobrarse()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);

            // Act - Primer viaje (línea 125)
            _colectivo125.PagarCon(tarjeta, _tiempo);

            // Avanzar 30 minutos
            _tiempo.AgregarMinutos(30);

            // Act - Segundo viaje (misma línea 125)
            Boleto boleto2 = _colectivo125.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto2.EsTrasbordo, Is.False, "Mismo colectivo no es trasbordo");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(1580f), "Debe cobrar precio completo");
                Assert.That(tarjeta.Saldo, Is.EqualTo(5000f - 1580f - 1580f), "Debe descontar dos pasajes");
            });
        }

        [Test]
        public void Trasbordo_DespuesDe1Hora_DebeCobrarse()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);

            // Act - Primer viaje
            _colectivo125.PagarCon(tarjeta, _tiempo);

            // Avanzar 61 minutos (más de 1 hora)
            _tiempo.AgregarMinutos(61);

            // Act - Segundo viaje
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto2.EsTrasbordo, Is.False, "Pasó más de 1 hora, no es trasbordo");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(1580f), "Debe cobrar precio completo");
            });
        }

        [Test]
        public void Trasbordo_ExactamenteEnUnaHora_DebeSer_Gratuito()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);

            // Act - Primer viaje
            _colectivo125.PagarCon(tarjeta, _tiempo);

            // Avanzar exactamente 1 hora (60 minutos)
            _tiempo.AgregarMinutos(60);

            // Act - Segundo viaje
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto2.EsTrasbordo, Is.True, "Exactamente 1 hora es válido");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f), "Trasbordo es gratis");
            });
        }

        // ============================================
        //    TESTS DE RESTRICCIONES HORARIAS
        // ============================================

        [Test]
        public void Trasbordo_AntesDeLas7AM_NoPermiteTrasbordo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);
            _tiempo.EstablecerHora(7, 0); // 7:00 AM

            // Act - Primer viaje a las 7:00
            _colectivo125.PagarCon(tarjeta, _tiempo);

            // Intentar trasbordo a las 6:30 (retrocedemos el tiempo conceptualmente)
            // Mejor: simular que el primer viaje fue a las 6:00
            _tiempo.EstablecerHora(6, 0);
            _colectivo125.PagarCon(tarjeta, _tiempo);

            _tiempo.AgregarMinutos(30); // 6:30

            // Act - Intento de trasbordo
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto2.EsTrasbordo, Is.False, "Antes de las 7:00 no permite trasbordo");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(1580f), "Cobra precio completo");
            });
        }

        [Test]
        public void Trasbordo_A_las7AM_PermiteTrasbordo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);
            _tiempo.EstablecerHora(7, 0);

            // Act - Primer viaje
            _colectivo125.PagarCon(tarjeta, _tiempo);

            // Avanzar 30 minutos
            _tiempo.AgregarMinutos(30);

            // Act - Trasbordo
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto2.EsTrasbordo, Is.True, "A las 7:00 permite trasbordo");
        }

        [Test]
        public void Trasbordo_A_las21_59_PermiteTrasbordo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);
            _tiempo.EstablecerHora(21, 30);

            // Act - Primer viaje
            _colectivo125.PagarCon(tarjeta, _tiempo);

            // Avanzar 29 minutos (21:59)
            _tiempo.AgregarMinutos(29);

            // Act - Trasbordo
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto2.EsTrasbordo, Is.True, "Antes de las 22:00 permite trasbordo");
        }

        [Test]
        public void Trasbordo_A_las22_00_NoPermiteTrasbordo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);
            _tiempo.EstablecerHora(21, 30);

            // Act - Primer viaje
            _colectivo125.PagarCon(tarjeta, _tiempo);

            // Avanzar 30 minutos (22:00)
            _tiempo.AgregarMinutos(30);

            // Act - Intento de trasbordo
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto2.EsTrasbordo, Is.False, "A las 22:00 o después no permite trasbordo");
        }

        [Test]
        public void Trasbordo_Domingo_NoPermiteTrasbordo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);
            // Domingo 10 de noviembre de 2024 a las 10:00
            _tiempo = new TiempoFalso(2024, 11, 10, 10, 0);

            // Act - Primer viaje
            _colectivo125.PagarCon(tarjeta, _tiempo);

            // Avanzar 30 minutos
            _tiempo.AgregarMinutos(30);

            // Act - Intento de trasbordo
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto2.EsTrasbordo, Is.False, "Domingos no permiten trasbordo");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(1580f), "Cobra precio completo");
            });
        }

        [Test]
        public void Trasbordo_Sabado_PermiteTrasbordo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);
            // Sábado 9 de noviembre de 2024 a las 10:00
            _tiempo = new TiempoFalso(2024, 11, 9, 10, 0);

            // Act - Primer viaje
            _colectivo125.PagarCon(tarjeta, _tiempo);

            // Avanzar 30 minutos
            _tiempo.AgregarMinutos(30);

            // Act - Trasbordo
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto2.EsTrasbordo, Is.True, "Sábados permiten trasbordo");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f), "Trasbordo es gratis");
            });
        }

        // ============================================
        //    TESTS DE TRASBORDOS MÚLTIPLES
        // ============================================

        [Test]
        public void TrasbordosMultiples_DentroDeUnaHora_TodosGratuitos()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);

            // Act - Viaje inicial (125)
            Boleto boleto1 = _colectivo125.PagarCon(tarjeta, _tiempo);
            _tiempo.AgregarMinutos(20);

            // Trasbordo a 140
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);
            _tiempo.AgregarMinutos(20);

            // Trasbordo a K
            Boleto boleto3 = _colectivoK.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.EsTrasbordo, Is.False);
                Assert.That(boleto1.TotalAbonado, Is.EqualTo(1580f));

                Assert.That(boleto2.EsTrasbordo, Is.True);
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f));

                Assert.That(boleto3.EsTrasbordo, Is.True);
                Assert.That(boleto3.TotalAbonado, Is.EqualTo(0f));

                Assert.That(tarjeta.Saldo, Is.EqualTo(5000f - 1580f), "Solo cobra el primer viaje");
            });
        }

        [Test]
        public void TrasbordoMultiple_VolverALineaAnterior_PermiteTrasbordo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);

            // Act - 125 → 140 → 125
            _colectivo125.PagarCon(tarjeta, _tiempo);
            _tiempo.AgregarMinutos(20);

            _colectivo140.PagarCon(tarjeta, _tiempo);
            _tiempo.AgregarMinutos(20);

            // Volver a 125 (diferente a la última que fue 140)
            Boleto boleto3 = _colectivo125.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto3.EsTrasbordo, Is.True, "Puede volver a línea anterior si es diferente a la última");
                Assert.That(boleto3.TotalAbonado, Is.EqualTo(0f));
            });
        }

        // ============================================
        //    TESTS CON DIFERENTES TIPOS DE TARJETA
        // ============================================

        [Test]
        public void Trasbordo_ConMedioBoleto_FuncionaCorrectamente()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);

            // Act - Primer viaje (paga medio boleto)
            Boleto boleto1 = _colectivo125.PagarCon(medioBoleto, _tiempo);
            _tiempo.AgregarMinutos(30);

            // Trasbordo
            Boleto boleto2 = _colectivo140.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.TotalAbonado, Is.EqualTo(790f), "Primer viaje: medio boleto");
                Assert.That(boleto2.EsTrasbordo, Is.True);
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f), "Trasbordo: gratis");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(5000f - 790f), "Solo descuenta primer viaje");
            });
        }

        [Test]
        public void Trasbordo_ConBoletoGratuito_FuncionaCorrectamente()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 2);

            // Act - Primer viaje (gratis)
            Boleto boleto1 = _colectivo125.PagarCon(boletoGratuito, _tiempo);
            _tiempo.AgregarMinutos(30);

            // Trasbordo
            Boleto boleto2 = _colectivo140.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.TotalAbonado, Is.EqualTo(0f), "Primer viaje: gratis");
                Assert.That(boleto2.EsTrasbordo, Is.True);
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f), "Trasbordo: gratis");
                Assert.That(boletoGratuito.Saldo, Is.EqualTo(5000f), "No descuenta nada");
                Assert.That(boletoGratuito.ViajesGratuitosHoy, Is.EqualTo(1), "Cuenta solo 1 viaje gratuito (el trasbordo no cuenta)");
            });
        }

        [Test]
        public void Trasbordo_ConFranquiciaCompleta_FuncionaCorrectamente()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(5000f, 3);

            // Act - Primer viaje (gratis)
            Boleto boleto1 = _colectivo125.PagarCon(franquicia, _tiempo);
            _tiempo.AgregarMinutos(30);

            // Trasbordo
            Boleto boleto2 = _colectivo140.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.TotalAbonado, Is.EqualTo(0f));
                Assert.That(boleto2.EsTrasbordo, Is.True);
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f));
                Assert.That(franquicia.Saldo, Is.EqualTo(5000f), "No descuenta nada");
            });
        }

        // ============================================
        //    TESTS DE CASOS ESPECIALES
        // ============================================

        [Test]
        public void Trasbordo_SinViajeAnterior_NoPermiteTrasbordo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);

            // Act - Primer viaje sin antecedentes
            Boleto boleto = _colectivo125.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto.EsTrasbordo, Is.False, "Primer viaje nunca es trasbordo");
        }

        [Test]
        public void Trasbordo_ConSaldoInsuficiente_PeroValidoParaTrasbordo_PermiteViajar()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(1580f, 1); // Saldo justo para 1 viaje

            // Act - Primer viaje (consume todo el saldo)
            _colectivo125.PagarCon(tarjeta, _tiempo);
            _tiempo.AgregarMinutos(30);

            // Trasbordo (sin saldo, pero válido)
            Boleto boleto2 = _colectivo140.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto2, Is.Not.Null, "Trasbordo permite viajar sin saldo");
                Assert.That(boleto2.EsTrasbordo, Is.True);
                Assert.That(tarjeta.Saldo, Is.EqualTo(0f), "Saldo sigue en 0");
            });
        }

        [Test]
        public void PuedeTrasbordar_VerificaCorrectamente()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);

            // Sin viaje previo
            Assert.That(tarjeta.PuedeTrasbordar("125", _tiempo), Is.False, "Sin viaje previo no puede trasbordar");

            // Después de un viaje
            _colectivo125.PagarCon(tarjeta, _tiempo);
            _tiempo.AgregarMinutos(30);

            Assert.That(tarjeta.PuedeTrasbordar("140", _tiempo), Is.True, "Puede trasbordar a línea diferente");
            Assert.That(tarjeta.PuedeTrasbordar("125", _tiempo), Is.False, "No puede trasbordar a misma línea");
        }
    }
}
