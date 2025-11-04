using NUnit.Framework;
using Tarjeta;
using TrabajoTarjeta;
using System;

namespace Tests1
{
    public class RestriccionesHorariasTest
    {
        private Colectivo _colectivo;
        private TiempoFalso _tiempo;

        [SetUp]
        public void Setup()
        {
            _colectivo = new Colectivo("125");
        }

        // ============================================
        //    TESTS DE MEDIO BOLETO ESTUDIANTIL
        // ============================================

        [Test]
        public void MedioBoleto_LunesA_las8AM_DebePermitirViaje()
        {
            // Arrange - Lunes 4 de noviembre a las 8:00
            _tiempo = new TiempoFalso(2024, 11, 4, 8, 0);
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);

            // Act
            Boleto boleto = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Not.Null, "Debería permitir viaje en horario válido");
                Assert.That(boleto.TotalAbonado, Is.EqualTo(790f), "Medio boleto: $790");
            });
        }

        [Test]
        public void MedioBoleto_ViernesA_las18HS_DebePermitirViaje()
        {
            // Arrange - Viernes 8 de noviembre a las 18:00
            _tiempo = new TiempoFalso(2024, 11, 8, 18, 0);
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);

            // Act
            Boleto boleto = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.That(boleto, Is.Not.Null, "Debería permitir viaje en horario válido");
        }

        [Test]
        public void MedioBoleto_LunesA_las6AM_DebePermitirViaje()
        {
            // Arrange - Lunes a las 6:00 (límite inferior)
            _tiempo = new TiempoFalso(2024, 11, 4, 6, 0);
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);

            // Act
            Boleto boleto = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.That(boleto, Is.Not.Null, "Debería permitir viaje a las 6:00");
        }

        [Test]
        public void MedioBoleto_LunesA_las21_59_DebePermitirViaje()
        {
            // Arrange - Lunes a las 21:59 (último minuto permitido)
            _tiempo = new TiempoFalso(2024, 11, 4, 21, 59);
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);

            // Act
            Boleto boleto = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.That(boleto, Is.Not.Null, "Debería permitir viaje a las 21:59");
        }

        [Test]
        public void MedioBoleto_LunesAntesDeLas6AM_NoDebePermitirViaje()
        {
            // Arrange - Lunes a las 5:59
            _tiempo = new TiempoFalso(2024, 11, 4, 5, 59);
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);
            float saldoInicial = medioBoleto.Saldo;

            // Act
            Boleto boleto = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Null, "No debería permitir viaje antes de las 6:00");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(saldoInicial), "Saldo no debe cambiar");
            });
        }

        [Test]
        public void MedioBoleto_LunesDespuesDeLas22HS_NoDebePermitirViaje()
        {
            // Arrange - Lunes a las 22:00
            _tiempo = new TiempoFalso(2024, 11, 4, 22, 0);
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);
            float saldoInicial = medioBoleto.Saldo;

            // Act
            Boleto boleto = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Null, "No debería permitir viaje a las 22:00 o después");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(saldoInicial), "Saldo no debe cambiar");
            });
        }

        [Test]
        public void MedioBoleto_Sabado_NoDebePermitirViaje()
        {
            // Arrange - Sábado 9 de noviembre a las 10:00
            _tiempo = new TiempoFalso(2024, 11, 9, 10, 0);
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);
            float saldoInicial = medioBoleto.Saldo;

            // Act
            Boleto boleto = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Null, "No debería permitir viaje en sábado");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(saldoInicial));
            });
        }

        [Test]
        public void MedioBoleto_Domingo_NoDebePermitirViaje()
        {
            // Arrange - Domingo 10 de noviembre a las 15:00
            _tiempo = new TiempoFalso(2024, 11, 10, 15, 0);
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);
            float saldoInicial = medioBoleto.Saldo;

            // Act
            Boleto boleto = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Null, "No debería permitir viaje en domingo");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(saldoInicial));
            });
        }

        // ============================================
        //    TESTS DE BOLETO GRATUITO ESTUDIANTIL
        // ============================================

        [Test]
        public void BoletoGratuito_MartesA_las12PM_DebePermitirViaje()
        {
            // Arrange - Martes 5 de noviembre a las 12:00
            _tiempo = new TiempoFalso(2024, 11, 5, 12, 0);
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 2);

            // Act
            Boleto boleto = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Not.Null, "Debería permitir viaje en horario válido");
                Assert.That(boleto.TotalAbonado, Is.EqualTo(0f), "Debería viajar gratis");
            });
        }

        [Test]
        public void BoletoGratuito_Sabado_NoDebePermitirViaje()
        {
            // Arrange - Sábado a las 10:00
            _tiempo = new TiempoFalso(2024, 11, 9, 10, 0);
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 2);

            // Act
            Boleto boleto = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.That(boleto, Is.Null, "No debería permitir viaje en sábado");
        }

        [Test]
        public void BoletoGratuito_DomingoA_las8AM_NoDebePermitirViaje()
        {
            // Arrange - Domingo a las 8:00
            _tiempo = new TiempoFalso(2024, 11, 10, 8, 0);
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 2);

            // Act
            Boleto boleto = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.That(boleto, Is.Null, "No debería permitir viaje en domingo");
        }

        [Test]
        public void BoletoGratuito_LunesA_las5AM_NoDebePermitirViaje()
        {
            // Arrange - Lunes a las 5:00
            _tiempo = new TiempoFalso(2024, 11, 4, 5, 0);
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 2);

            // Act
            Boleto boleto = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.That(boleto, Is.Null, "No debería permitir viaje antes de las 6:00");
        }

        [Test]
        public void BoletoGratuito_ViernesA_las23HS_NoDebePermitirViaje()
        {
            // Arrange - Viernes a las 23:00
            _tiempo = new TiempoFalso(2024, 11, 8, 23, 0);
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 2);

            // Act
            Boleto boleto = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.That(boleto, Is.Null, "No debería permitir viaje después de las 22:00");
        }

        // ============================================
        //       TESTS DE FRANQUICIA COMPLETA
        // ============================================

        [Test]
        public void FranquiciaCompleta_JuevesA_las14HS_DebePermitirViaje()
        {
            // Arrange - Jueves 7 de noviembre a las 14:00
            _tiempo = new TiempoFalso(2024, 11, 7, 14, 0);
            var franquicia = new FranquiciaCompleta(5000f, 3);

            // Act
            Boleto boleto = _colectivo.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Not.Null, "Debería permitir viaje en horario válido");
                Assert.That(boleto.TotalAbonado, Is.EqualTo(0f), "Debería viajar gratis");
            });
        }

        [Test]
        public void FranquiciaCompleta_Sabado_NoDebePermitirViaje()
        {
            // Arrange - Sábado a las 12:00
            _tiempo = new TiempoFalso(2024, 11, 9, 12, 0);
            var franquicia = new FranquiciaCompleta(5000f, 3);

            // Act
            Boleto boleto = _colectivo.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.That(boleto, Is.Null, "No debería permitir viaje en sábado");
        }

        [Test]
        public void FranquiciaCompleta_Domingo_NoDebePermitirViaje()
        {
            // Arrange - Domingo a las 16:00
            _tiempo = new TiempoFalso(2024, 11, 10, 16, 0);
            var franquicia = new FranquiciaCompleta(5000f, 3);

            // Act
            Boleto boleto = _colectivo.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.That(boleto, Is.Null, "No debería permitir viaje en domingo");
        }

        [Test]
        public void FranquiciaCompleta_LunesA_las4AM_NoDebePermitirViaje()
        {
            // Arrange - Lunes a las 4:00
            _tiempo = new TiempoFalso(2024, 11, 4, 4, 0);
            var franquicia = new FranquiciaCompleta(5000f, 3);

            // Act
            Boleto boleto = _colectivo.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.That(boleto, Is.Null, "No debería permitir viaje antes de las 6:00");
        }

        [Test]
        public void FranquiciaCompleta_MiercolesA_las22_30_NoDebePermitirViaje()
        {
            // Arrange - Miércoles a las 22:30
            _tiempo = new TiempoFalso(2024, 11, 6, 22, 30);
            var franquicia = new FranquiciaCompleta(5000f, 3);

            // Act
            Boleto boleto = _colectivo.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.That(boleto, Is.Null, "No debería permitir viaje después de las 22:00");
        }

        // ============================================
        //       TESTS DE LÍMITES HORARIOS
        // ============================================

        [Test]
        public void TodasFranquicias_LimitesExactos_DebeFuncionarCorrectamente()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(10000f, 1);
            var boletoGratuito = new BoletoGratuitoEstudiantil(10000f, 2);
            var franquicia = new FranquiciaCompleta(10000f, 3);

            // Test límite inferior - 6:00 (debe permitir)
            _tiempo = new TiempoFalso(2024, 11, 4, 6, 0);

            // Test límite superior - 21:59 (debe permitir)
            var tiempo2 = new TiempoFalso(2024, 11, 4, 21, 59);

            // Test justo después - 22:00 (NO debe permitir)
            var tiempo3 = new TiempoFalso(2024, 11, 4, 22, 0);

            // Act & Assert
            Assert.Multiple(() =>
            {
                // A las 6:00 - debe permitir
                Assert.That(_colectivo.PagarCon(medioBoleto, _tiempo), Is.Not.Null);
                Assert.That(_colectivo.PagarCon(boletoGratuito, _tiempo), Is.Not.Null);
                Assert.That(_colectivo.PagarCon(franquicia, _tiempo), Is.Not.Null);

                // A las 21:59 - debe permitir
                Assert.That(_colectivo.PagarCon(medioBoleto, tiempo2), Is.Not.Null);
                Assert.That(_colectivo.PagarCon(boletoGratuito, tiempo2), Is.Not.Null);
                Assert.That(_colectivo.PagarCon(franquicia, tiempo2), Is.Not.Null);

                // A las 22:00 - NO debe permitir
                Assert.That(_colectivo.PagarCon(medioBoleto, tiempo3), Is.Null);
                Assert.That(_colectivo.PagarCon(boletoGratuito, tiempo3), Is.Null);
                Assert.That(_colectivo.PagarCon(franquicia, tiempo3), Is.Null);
            });
        }

        [Test]
        public void MedioBoleto_FueraDeHorario_NoIncrementaContadorViajes()
        {
            // Arrange - Sábado (no permitido)
            _tiempo = new TiempoFalso(2024, 11, 9, 10, 0);
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);

            // Act
            Boleto boleto = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Null, "No debería permitir viaje");
                Assert.That(medioBoleto.ViajesHoy, Is.EqualTo(0),
                    "No debería incrementar contador de viajes");
            });
        }

        [Test]
        public void BoletoGratuito_FueraDeHorario_NoIncrementaContador()
        {
            // Arrange - Domingo (no permitido)
            _tiempo = new TiempoFalso(2024, 11, 10, 10, 0);
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 2);

            // Act
            Boleto boleto = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Null, "No debería permitir viaje");
                Assert.That(boletoGratuito.ViajesGratuitosHoy, Is.EqualTo(0),
                    "No debería incrementar contador de viajes gratuitos");
            });
        }
    }
}