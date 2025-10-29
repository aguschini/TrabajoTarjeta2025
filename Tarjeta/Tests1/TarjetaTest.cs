using NUnit.Framework;
using System.Collections.Generic;
using Tarjeta;

namespace Tests1
{
    public class TarjetaTest
    {
        private Tarjeta.Tarjeta _tarjeta;

        [SetUp]
        public void Setup()
        {
            _tarjeta = new Tarjeta.Tarjeta(1000f, 1);
        }

        // ============================================
        //            TESTS DE ITERACIÓN 1 
        // ============================================

        [Test]
        public void Constructor_DeberiaCrearTarjetaConSaldoYIdCorrectos()
        {
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(1000f));
                Assert.That(_tarjeta.Id, Is.EqualTo(1));
            });
        }
        [Test]
        public void Cargar_CantidadPermitida_DeberiaAumentarSaldo()
        {
            // Act
            _tarjeta.Cargar(2000f);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(3000f));
        }

        [Test]
        public void Cargar_CantidadNoPermitida_NoDeberiaCambiarSaldo()
        {
            // Arrange
            float saldoInicial = _tarjeta.Saldo;

            // Act
            _tarjeta.Cargar(2500f); // Cantidad no permitida

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(saldoInicial));
        }

        [Test]
        public void Cargar_SuperaLimiteMaximo_NoDeberiaCambiarSaldo()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(35000f, 1); // Casi en el límite
            float saldoInicial = _tarjeta.Saldo;

            // Act
            _tarjeta.Cargar(10000f); // Esto superaría 40,000

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(saldoInicial));
        }

        [Test]
        public void Cargar_CantidadPermitidaEnLimite_DeberiaAceptarCarga()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(35000f, 1);

            // Act
            _tarjeta.Cargar(5000f); // Llegaría exactamente a 40,000

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(40000f));
        }

        [Test]
        public void Cargar_VerificarTodasCargasPermitidas()
        {
            // Arrange
            var cargasPermitidas = new List<float>
            {
                2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000
            };

            foreach (var carga in cargasPermitidas)
            {
                // Arrange para cada test
                _tarjeta = new Tarjeta.Tarjeta(0f, 1);

                // Act
                _tarjeta.Cargar(carga);

                // Assert
                Assert.That(_tarjeta.Saldo, Is.EqualTo(carga));
            }
        }

        [Test]
        public void Saldo_DeberiaPoderModificarValor()
        {
            // Act
            _tarjeta.Saldo = 5000f;

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(5000f));
        }

        [Test]
        public void Id_DeberiaPoderModificarValor()
        {
            // Act
            _tarjeta.Id = 2;

            // Assert
            Assert.That(_tarjeta.Id, Is.EqualTo(2));
        }

        // ============================================
        // TESTS DE ITERACIÓN 2 - DESCUENTO DE SALDOS
        // ============================================

        [Test]
        public void DescontarSaldo_SaldoSuficiente_DeberiaDescontarYRetornarTrue()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(2000f, 1);
            float montoADescontar = 1580f;
            float saldoEsperado = 420f; // 2000 - 1580

            // Act
            bool resultado = _tarjeta.DescontarSaldo(montoADescontar);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True, "Debería retornar true cuando hay saldo suficiente");
                Assert.That(_tarjeta.Saldo, Is.EqualTo(saldoEsperado), "El saldo debería descontarse correctamente");
            });
        }

        [Test]
        public void DescontarSaldo_SaldoExacto_DeberiaDejarEnCeroYRetornarTrue()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(1580f, 1);

            // Act
            bool resultado = _tarjeta.DescontarSaldo(1580f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True, "Debería retornar true con saldo exacto");
                Assert.That(_tarjeta.Saldo, Is.EqualTo(0f), "El saldo debería quedar en cero");
            });
        }

        [Test]
        public void DescontarSaldo_MultipleDescuentos_DeberiaDescontarCorrectamente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(5000f, 1);

            // Act & Assert - Primer descuento
            bool descuento1 = _tarjeta.DescontarSaldo(1580f);
            Assert.That(descuento1, Is.True);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(3420f)); // 5000 - 1580

            // Act & Assert - Segundo descuento
            bool descuento2 = _tarjeta.DescontarSaldo(1580f);
            Assert.That(descuento2, Is.True);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(1840f)); // 3420 - 1580

            // Act & Assert - Tercer descuento
            bool descuento3 = _tarjeta.DescontarSaldo(1580f);
            Assert.That(descuento3, Is.True);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(260f)); // 1840 - 1580

            // Act & Assert - Cuarto descuento (debe fallar)
            bool descuento4 = _tarjeta.DescontarSaldo(1580f);
            Assert.That(descuento4, Is.False);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(260f)); // No debe cambiar
        }

        [Test]
        public void DescontarSaldo_DescuentoPequeno_DebeFuncionar()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(100f, 1);

            // Act
            bool resultado = _tarjeta.DescontarSaldo(50f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True);
                Assert.That(_tarjeta.Saldo, Is.EqualTo(50f));
            });
        }



        // ============================================
        //    TESTS DE ITERACIÓN 2 - SALDO NEGATIVO
        // ============================================

        [Test]
        public void SaldoNegativo_NoPuedeQuedarMenorMenos1200()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(300f, 1); // Si paga 1580, quedaría en -1280 (excede límite)
            float saldoInicial = _tarjeta.Saldo;

            // Act
            bool resultado = _tarjeta.DescontarSaldo(1580f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.False, "No debería permitir saldo menor a -1200");
                Assert.That(_tarjeta.Saldo, Is.EqualTo(saldoInicial), "El saldo no debería cambiar");
            });
        }

        [Test]
        public void SaldoNegativo_PermiteHastaMenos1200Exacto()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(380f, 1); // 380 - 1580 = -1200 (exactamente el límite)

            // Act
            bool resultado = _tarjeta.DescontarSaldo(1580f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True, "Debería permitir llegar exactamente a -1200");
                Assert.That(_tarjeta.Saldo, Is.EqualTo(-1200f), "El saldo debería ser exactamente -1200");
            });
        }

        [Test]
        public void SaldoNegativo_PermiteSaldoNegativoDentroDelLimite()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(800f, 1); // 800 - 1580 = -780

            // Act
            bool resultado = _tarjeta.DescontarSaldo(1580f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True, "Debería permitir saldo negativo dentro del límite");
                Assert.That(_tarjeta.Saldo, Is.EqualTo(-780f), "El saldo debería quedar en -780");
            });
        }

        [Test]
        public void Cargar_ConSaldoNegativo_DescuentaDeudaPrimero()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(-500f, 1); // Saldo negativo de -500

            // Act
            _tarjeta.Cargar(2000f);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(1500f), "Debería descontar la deuda: 2000 - 500 = 1500");
        }

        [Test]
        public void Cargar_ConSaldoNegativo_YSuperaMaximo_NoPermiteCarga()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(-1000f, 1); // Saldo negativo

            // Act
            _tarjeta.Cargar(30000f); // 30000 - 1000 = 29000 (permitido)
            float saldoDespuesPrimeraCarga = _tarjeta.Saldo;

            _tarjeta.Cargar(15000f); // 29000 + 15000 = 44000 (excede 40000)

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(saldoDespuesPrimeraCarga),
                "No debería permitir carga que supere el máximo de 40000");
        }

        [Test]
        public void ViajePlus_DescuentaCorrectamenteAlRecargar()
        {
            // Arrange - Simular un viaje plus
            _tarjeta = new Tarjeta.Tarjeta(1000f, 1);

            // Act - Realizar primer viaje (queda con 1000 - 1580 = -580)
            _tarjeta.DescontarSaldo(1580f);
            float saldoDespuesViajePlus = _tarjeta.Saldo;

            // Assert - Verificar que quedó negativo
            Assert.That(saldoDespuesViajePlus, Is.EqualTo(-580f), "Debería quedar con saldo negativo");

            // Act - Recargar
            _tarjeta.Cargar(3000f);

            // Assert - Verificar descuento correcto
            Assert.That(_tarjeta.Saldo, Is.EqualTo(2420f),
                "Al recargar 3000 con deuda de -580, debería quedar: 3000 - 580 = 2420");
        }

        [Test]
        public void ViajePlus_MultiplesViajes_DescuentaCorrectamente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(2000f, 1);

            // Act - Primer viaje (2000 - 1580 = 420)
            _tarjeta.DescontarSaldo(1580f);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(420f));

            // Act - Segundo viaje con plus (420 - 1580 = -1160)
            _tarjeta.DescontarSaldo(1580f);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(-1160f), "Debería permitir segundo viaje plus");

            // Act - Recargar
            _tarjeta.Cargar(5000f);

            // Assert - Verificar que descuenta ambos viajes plus
            Assert.That(_tarjeta.Saldo, Is.EqualTo(3840f),
                "Al recargar 5000 con deuda de -1160: 5000 - 1160 = 3840");
        }

        [Test]
        public void ViajePlus_NoPermiteTercerViajeQueSupereLimite()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(1000f, 1);

            // Act - Primer viaje plus (1000 - 1580 = -580)
            bool viaje1 = _tarjeta.DescontarSaldo(1580f);
            Assert.That(viaje1, Is.True);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(-580f));

            // Act - Intento de segundo viaje plus (-580 - 1580 = -2160, excede -1200)
            bool viaje2 = _tarjeta.DescontarSaldo(1580f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(viaje2, Is.False, "No debería permitir viaje que supere -1200");
                Assert.That(_tarjeta.Saldo, Is.EqualTo(-580f), "El saldo no debería cambiar");
            });
        }

        [Test]
        public void Cargar_ConSaldoNegativoGrande_RestableceCorrectamente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(-1200f, 1); // Máximo negativo permitido

            // Act
            _tarjeta.Cargar(5000f);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(3800f),
                "Con saldo -1200 y recarga de 5000: 5000 - 1200 = 3800");
        }

        [Test]
        public void ViajePlus_ConRecargaInsuficiente_DescuentaLoQueSeRecarga()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(500f, 1);

            // Act - Viaje que deja negativo (500 - 1580 = -1080)
            _tarjeta.DescontarSaldo(1580f);
            Assert.That(_tarjeta.Saldo, Is.EqualTo(-1080f));

            // Act - Recarga pequeña que no cubre toda la deuda
            _tarjeta.Cargar(2000f);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(920f),
                "Con deuda de -1080 y recarga de 2000: 2000 - 1080 = 920");
        }
    }
}
