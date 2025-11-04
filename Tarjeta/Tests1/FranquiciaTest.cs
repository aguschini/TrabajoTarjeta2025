using NUnit.Framework;
using Tarjeta;
using TrabajoTarjeta;

namespace Tests1
{
    /// <summary>
    /// Tests para las franquicias de boleto (Iteración 2)
    /// </summary>
    public class FranquiciasTest
    {
        private Colectivo _colectivo;
        private TiempoFalso _tiempo;

        [SetUp]
        public void Setup()
        {
            _colectivo = new Colectivo("125");
            // Establecer hora válida para franquicias (10:00 AM, horario permitido)
            _tiempo = new TiempoFalso(2024, 11, 1, 10, 0);
        }

        // ============================================
        //      TESTS DE MEDIO BOLETO ESTUDIANTIL
        // ============================================

        [Test]
        public void MedioBoleto_PagaLaMitadDelValor()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(2000f, 1);
            float saldoInicial = medioBoleto.Saldo;
            float precioCompleto = 1580f;
            float precioEsperado = precioCompleto / 2f; // 790

            // Act
            bool resultado = medioBoleto.DescontarSaldo(precioCompleto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True, "El pago debería ser exitoso");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(saldoInicial - precioEsperado),
                    $"Debería descontar la mitad: {saldoInicial} - {precioEsperado} = {saldoInicial - precioEsperado}");
            });
        }

        [Test]
        public void MedioBoleto_DescuentaSiempreLaMitad()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);

            // Act - Primer viaje
            medioBoleto.DescontarSaldo(1580f); // Descuenta 790
            float saldoDespuesPrimero = medioBoleto.Saldo;

            // Act - Segundo viaje
            medioBoleto.DescontarSaldo(1580f); // Descuenta 790
            float saldoDespuesSegundo = medioBoleto.Saldo;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(saldoDespuesPrimero, Is.EqualTo(4210f), "Primer viaje: 5000 - 790 = 4210");
                Assert.That(saldoDespuesSegundo, Is.EqualTo(3420f), "Segundo viaje: 4210 - 790 = 3420");
            });
        }


        [Test]
        public void MedioBoleto_PermiteSaldoNegativoHastaLimite()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(400f, 1); // 400 - 790 = -390 (permitido)

            // Act
            bool resultado = medioBoleto.DescontarSaldo(1580f); // Descuenta 790

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True, "Debería permitir saldo negativo dentro del límite");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(-390f), "Saldo: 400 - 790 = -390");
            });
        }

        [Test]
        public void MedioBoleto_NoPermiteSaldoMenorMenos1200()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(-500f, 1); // -500 - 790 = -1290 (excede)
            float saldoInicial = medioBoleto.Saldo;

            // Act
            bool resultado = medioBoleto.DescontarSaldo(1580f); // Intentaría descontar 790

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.False, "No debería permitir exceder -1200");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(saldoInicial), "El saldo no debería cambiar");
            });
        }

        [Test]
        public void MedioBoleto_ConColectivo_DescuentaCorrectamente()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(2000f, 1);

            // Act
            Boleto boleto = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Not.Null, "El pago debería ser exitoso");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(1210f), "Debería descontar 790: 2000 - 790 = 1210");
            });
        }

        // ============================================
        //   TESTS DE LIMITACIONES DE MEDIO BOLETO
        // ============================================

        [Test]
        public void MedioBoleto_SegundoViajeAntesDe5Minutos_NoDebePermitirViaje()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);
            float saldoInicial = medioBoleto.Saldo;

            // Act - Primer viaje
            Boleto boleto1 = _colectivo.PagarCon(medioBoleto, _tiempo);
            float saldoDespuesPrimero = medioBoleto.Saldo;

            // Avanzar solo 3 minutos (menos de 5)
            _tiempo.AgregarMinutos(3);

            // Act - Segundo viaje (antes de 5 minutos) - DEBE RECHAZARSE
            Boleto boleto2 = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1, Is.Not.Null, "Primer viaje debe ser exitoso");
                Assert.That(boleto1.TotalAbonado, Is.EqualTo(790f), "Primer viaje: medio boleto");
                Assert.That(saldoDespuesPrimero, Is.EqualTo(4210f), "Primer viaje: 5000 - 790 = 4210");

                Assert.That(boleto2, Is.Null,
                    "Segundo viaje debe ser RECHAZADO por no respetar 5 minutos");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(4210f),
                    "Saldo no cambia cuando el viaje es rechazado");
            });
        }

        [Test]
        public void MedioBoleto_SegundoViajeDespuesDe5Minutos_CobraMedioBoleto()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);

            // Act - Primer viaje
            Boleto boleto1 = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Avanzar 5 minutos exactos
            _tiempo.AgregarMinutos(5);

            // Act - Segundo viaje (después de 5 minutos)
            Boleto boleto2 = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.TotalAbonado, Is.EqualTo(790f), "Primer viaje: medio boleto");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(790f),
                    "Segundo viaje: medio boleto después de 5 minutos");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(3420f), "5000 - 790 - 790 = 3420");
            });
        }

        [Test]
        public void MedioBoleto_TercerViajeMismoDia_CobraPrecioCompleto()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 1);

            // Act - Primer viaje (medio boleto)
            Boleto boleto1 = _colectivo.PagarCon(medioBoleto, _tiempo);
            _tiempo.AgregarMinutos(5);

            // Act - Segundo viaje (medio boleto)
            Boleto boleto2 = _colectivo.PagarCon(medioBoleto, _tiempo);
            _tiempo.AgregarMinutos(5);

            // Act - Tercer viaje (debería cobrar completo)
            Boleto boleto3 = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.TotalAbonado, Is.EqualTo(790f), "Primer viaje: medio boleto");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(790f), "Segundo viaje: medio boleto");
                Assert.That(boleto3.TotalAbonado, Is.EqualTo(1580f),
                    "Tercer viaje del día: precio completo (máximo 2 por día)");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(1840f), "5000 - 790 - 790 - 1580 = 1840");
            });
        }

        [Test]
        public void MedioBoleto_NuevoDia_ReiniciaContadorViajes()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(10000f, 1);

            // Act - Dos viajes el primer día
            _colectivo.PagarCon(medioBoleto, _tiempo);
            _tiempo.AgregarMinutos(5);
            _colectivo.PagarCon(medioBoleto, _tiempo);
            _tiempo.AgregarMinutos(5);

            // Tercer viaje del primer día (debería cobrar completo)
            Boleto boleto3Dia1 = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Avanzar 3 días (viernes -> lunes, evitando fin de semana)
            _tiempo.AgregarDias(3);

            // Primer viaje del nuevo día (debería volver a tener medio boleto)
            Boleto boleto1Dia2 = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto3Dia1, Is.Not.Null, "Tercer viaje día 1 debería ser exitoso");
                Assert.That(boleto3Dia1.TotalAbonado, Is.EqualTo(1580f),
                    "Tercer viaje día 1: precio completo");
                Assert.That(boleto1Dia2, Is.Not.Null, "Primer viaje día 2 debería ser exitoso");
                Assert.That(boleto1Dia2.TotalAbonado, Is.EqualTo(790f),
                    "Primer viaje día 2: medio boleto (contador reiniciado)");
            });
        }

        // ============================================
        //    TESTS DE BOLETO GRATUITO ESTUDIANTIL
        // ============================================

        [Test]
        public void BoletoGratuito_SiemprePuedePagar()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(0f, 2); // Sin saldo

            // Act & Assert - Múltiples viajes
            for (int i = 0; i < 5; i++)
            {
                bool resultado = boletoGratuito.DescontarSaldo(1580f);
                Assert.That(resultado, Is.True, $"Viaje {i + 1} debería ser gratuito");
            }

            // Assert - El saldo no cambió
            Assert.That(boletoGratuito.Saldo, Is.EqualTo(0f), "El saldo no debería cambiar");
        }

        [Test]
        public void BoletoGratuito_NoModificaSaldo()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(1000f, 2);
            float saldoInicial = boletoGratuito.Saldo;

            // Act
            boletoGratuito.DescontarSaldo(1580f);
            boletoGratuito.DescontarSaldo(1580f);
            boletoGratuito.DescontarSaldo(1580f);

            // Assert
            Assert.That(boletoGratuito.Saldo, Is.EqualTo(saldoInicial),
                "El saldo no debería cambiar nunca");
        }

        [Test]
        public void BoletoGratuito_ConColectivo_NoDescuentaSaldo()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(500f, 2);
            float saldoInicial = boletoGratuito.Saldo;

            // Act
            Boleto boleto = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Not.Null, "El pago debería ser exitoso");
                Assert.That(boletoGratuito.Saldo, Is.EqualTo(saldoInicial), "El saldo no debería cambiar");
            });
        }

        // ============================================
        //   TESTS DE LIMITACIONES BOLETO GRATUITO
        // ============================================

        [Test]
        public void BoletoGratuito_TercerViajeMismoDia_CobraPrecioCompleto()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 2);

            // Act - Primer viaje gratuito
            Boleto boleto1 = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Act - Segundo viaje gratuito
            Boleto boleto2 = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Act - Tercer viaje (debería cobrar completo)
            Boleto boleto3 = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.TotalAbonado, Is.EqualTo(0f), "Primer viaje: gratuito");
                Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f), "Segundo viaje: gratuito");
                Assert.That(boleto3.TotalAbonado, Is.EqualTo(1580f),
                    "Tercer viaje del día: precio completo (máximo 2 gratuitos por día)");
                Assert.That(boletoGratuito.Saldo, Is.EqualTo(3420f), "5000 - 0 - 0 - 1580 = 3420");
            });
        }

        [Test]
        public void BoletoGratuito_CuartoYQuintoViaje_CobranPrecioCompleto()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(10000f, 2);

            // Act - Dos viajes gratuitos
            _colectivo.PagarCon(boletoGratuito, _tiempo); // Gratis
            _colectivo.PagarCon(boletoGratuito, _tiempo); // Gratis

            // Act - Viajes 3, 4 y 5 (deberían cobrar completo)
            Boleto boleto3 = _colectivo.PagarCon(boletoGratuito, _tiempo);
            Boleto boleto4 = _colectivo.PagarCon(boletoGratuito, _tiempo);
            Boleto boleto5 = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto3.TotalAbonado, Is.EqualTo(1580f), "Tercer viaje: completo");
                Assert.That(boleto4.TotalAbonado, Is.EqualTo(1580f), "Cuarto viaje: completo");
                Assert.That(boleto5.TotalAbonado, Is.EqualTo(1580f), "Quinto viaje: completo");
                Assert.That(boletoGratuito.Saldo, Is.EqualTo(5260f), "10000 - 0 - 0 - 1580 - 1580 - 1580 = 5260");
            });
        }

        [Test]
        public void BoletoGratuito_NuevoDia_ReiniciaContador()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(10000f, 2);

            // Act - Dos viajes gratuitos el primer día
            _colectivo.PagarCon(boletoGratuito, _tiempo); // Gratis
            _colectivo.PagarCon(boletoGratuito, _tiempo); // Gratis

            // Tercer viaje del primer día (debería cobrar completo)
            Boleto boleto3Dia1 = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Avanzar 3 días (viernes -> lunes, evitando fin de semana)
            _tiempo.AgregarDias(3);

            // Primer viaje del nuevo día (debería ser gratuito otra vez)
            Boleto boleto1Dia2 = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto3Dia1, Is.Not.Null, "Tercer viaje día 1 debería ser exitoso");
                Assert.That(boleto3Dia1.TotalAbonado, Is.EqualTo(1580f),
                    "Tercer viaje día 1: precio completo");
                Assert.That(boleto1Dia2, Is.Not.Null, "Primer viaje día 2 debería ser exitoso");
                Assert.That(boleto1Dia2.TotalAbonado, Is.EqualTo(0f),
                    "Primer viaje día 2: gratuito (contador reiniciado)");
                Assert.That(boletoGratuito.Saldo, Is.EqualTo(8420f), "10000 - 0 - 0 - 1580 - 0 = 8420");
            });
        }

        [Test]
        public void BoletoGratuito_TercerViajeConPocoSaldo_PermiteSaldoNegativo()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(1000f, 2);

            // Act - Dos viajes gratuitos
            Boleto boleto1 = _colectivo.PagarCon(boletoGratuito, _tiempo);
            Boleto boleto2 = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Act - Tercer viaje (cobra completo, permite saldo negativo)
            Boleto boleto3 = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1, Is.Not.Null, "Primer viaje: exitoso y gratuito");
                Assert.That(boleto2, Is.Not.Null, "Segundo viaje: exitoso y gratuito");
                Assert.That(boleto3, Is.Not.Null, "Tercer viaje: exitoso pero cobra completo");
                Assert.That(boleto3.TotalAbonado, Is.EqualTo(1580f), "Tercer viaje cobra precio completo");
                Assert.That(boletoGratuito.Saldo, Is.EqualTo(-580f), "Permite saldo negativo: 1000 - 1580 = -580");
            });
        }

        [Test]
        public void BoletoGratuito_TercerViajeExcediendoLimite_NoPuedePagar()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(200f, 2); // Saldo que excedería -1200

            // Act - Dos viajes gratuitos
            _colectivo.PagarCon(boletoGratuito, _tiempo);
            _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Act - Tercer viaje (200 - 1580 = -1380, excede límite de -1200)
            Boleto boleto3 = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto3, Is.Null, "Tercer viaje falla: excedería saldo mínimo de -1200");
                Assert.That(boletoGratuito.Saldo, Is.EqualTo(200f), "Saldo no cambia al fallar el pago");
            });
        }

        // ============================================
        //       TESTS DE FRANQUICIA COMPLETA
        // ============================================

        [Test]
        public void FranquiciaCompleta_SiemprePuedePagar()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(0f, 3); // Sin saldo

            // Act & Assert - Múltiples viajes
            for (int i = 0; i < 10; i++)
            {
                bool resultado = franquicia.DescontarSaldo(1580f);
                Assert.That(resultado, Is.True, $"Viaje {i + 1} debería ser gratuito");
            }

            // Assert - El saldo no cambió
            Assert.That(franquicia.Saldo, Is.EqualTo(0f), "El saldo no debería cambiar");
        }

        [Test]
        public void FranquiciaCompleta_RetornaSiempreTrue()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(1000f, 3);

            // Act
            bool resultado1 = franquicia.DescontarSaldo(1580f);
            bool resultado2 = franquicia.DescontarSaldo(5000f);
            bool resultado3 = franquicia.DescontarSaldo(10000f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado1, Is.True, "Siempre debe retornar true");
                Assert.That(resultado2, Is.True, "Siempre debe retornar true");
                Assert.That(resultado3, Is.True, "Siempre debe retornar true");
            });
        }

        [Test]
        public void FranquiciaCompleta_NoModificaSaldo()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(2500f, 3);
            float saldoInicial = franquicia.Saldo;

            // Act - Múltiples viajes
            franquicia.DescontarSaldo(1580f);
            franquicia.DescontarSaldo(1580f);
            franquicia.DescontarSaldo(1580f);
            franquicia.DescontarSaldo(1580f);

            // Assert
            Assert.That(franquicia.Saldo, Is.EqualTo(saldoInicial),
                "El saldo no debería cambiar nunca");
        }

        [Test]
        public void FranquiciaCompleta_ConColectivo_NoDescuentaSaldo()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(3000f, 3);
            float saldoInicial = franquicia.Saldo;

            // Act
            Boleto boleto = _colectivo.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto, Is.Not.Null, "El pago debería ser exitoso");
                Assert.That(franquicia.Saldo, Is.EqualTo(saldoInicial), "El saldo no debería cambiar");
            });
        }

        [Test]
        public void FranquiciaCompleta_ConSaldoNegativo_SiguePudiendoPagar()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(-500f, 3);

            // Act
            bool resultado = franquicia.DescontarSaldo(1580f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True, "Debería poder pagar incluso con saldo negativo");
                Assert.That(franquicia.Saldo, Is.EqualTo(-500f), "El saldo no debería cambiar");
            });
        }

        // ============================================
        //         TESTS DE COMPARACIÓN
        // ============================================

        [Test]
        public void ComparacionTiposTarjeta_DescuentosDiferentes()
        {
            // Arrange
            var tarjetaNormal = new Tarjeta.Tarjeta(3000f, 1);
            var medioBoleto = new MedioBoletoEstudiantil(3000f, 2);
            var boletoGratuito = new BoletoGratuitoEstudiantil(3000f, 3);
            var franquicia = new FranquiciaCompleta(3000f, 4);

            // Act
            tarjetaNormal.DescontarSaldo(1580f);
            medioBoleto.DescontarSaldo(1580f);
            boletoGratuito.DescontarSaldo(1580f);
            franquicia.DescontarSaldo(1580f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(tarjetaNormal.Saldo, Is.EqualTo(1420f), "Normal: 3000 - 1580 = 1420");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(2210f), "Medio: 3000 - 790 = 2210");
                Assert.That(boletoGratuito.Saldo, Is.EqualTo(3000f), "Gratuito: sin descuento");
                Assert.That(franquicia.Saldo, Is.EqualTo(3000f), "Franquicia: sin descuento");
            });
        }
    }
}