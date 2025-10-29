using NUnit.Framework;
using Tarjeta;

namespace Tests1
{
    /// <summary>
    /// Tests para las franquicias de boleto (Iteración 2)
    /// </summary>
    public class FranquiciasTest
    {
        private Colectivo _colectivo;

        [SetUp]
        public void Setup()
        {
            _colectivo = new Colectivo("125");
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
            bool resultado = _colectivo.PagarCon(medioBoleto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True, "El pago debería ser exitoso");
                Assert.That(medioBoleto.Saldo, Is.EqualTo(1210f), "Debería descontar 790: 2000 - 790 = 1210");
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
            bool resultado = _colectivo.PagarCon(boletoGratuito);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True, "El pago debería ser exitoso");
                Assert.That(boletoGratuito.Saldo, Is.EqualTo(saldoInicial), "El saldo no debería cambiar");
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
            bool resultado = _colectivo.PagarCon(franquicia);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True, "El pago debería ser exitoso");
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