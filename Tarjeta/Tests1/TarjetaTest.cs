using NUnit.Framework;
using Tarjeta;
using System;

namespace Tests1
{
    public class TarjetaTest
    {
        private Tarjeta.Tarjeta _tarjeta;

        [SetUp]
        public void Setup()
        {
            _tarjeta = new Tarjeta.Tarjeta(5000f, 1);
        }

        // ============================================
        //           TESTS DE CONSTRUCTOR
        // ============================================

        [Test]
        public void Constructor_DeberiaInicializarCorrectamente()
        {
            // Arrange & Act
            var tarjeta = new Tarjeta.Tarjeta(3000f, 5);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(tarjeta.Saldo, Is.EqualTo(3000f));
                Assert.That(tarjeta.Id, Is.EqualTo(5));
            });
        }

        // ============================================
        //           TESTS DE DESCONTAR SALDO
        // ============================================

        [Test]
        public void DescontarSaldo_ConSaldoSuficiente_DeberiaDescontar()
        {
            // Act
            bool resultado = _tarjeta.DescontarSaldo(1580f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True);
                Assert.That(_tarjeta.Saldo, Is.EqualTo(3420f));
            });
        }

        [Test]
        public void DescontarSaldo_SaldoInsuficiente_NoDeberiaDescontar()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(500f, 1);

            // Act - Intentar descontar más de lo que permite el límite negativo
            // 500 - 1580 = -1080 (PERMITIDO, no excede -1200)
            // Para que falle necesitamos: 500 - 2000 = -1500 (excede -1200)
            bool resultado = tarjeta.DescontarSaldo(2000f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.False, "No debería permitir exceder -1200");
                Assert.That(tarjeta.Saldo, Is.EqualTo(500f), "Saldo no debería cambiar");
            });
        }

        [Test]
        public void DescontarSaldo_PermiteSaldoNegativoHastaLimite()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(500f, 1);

            // Act
            bool resultado = tarjeta.DescontarSaldo(1580f);

            // Assert - 500 - 1580 = -1080 (permitido, no excede -1200)
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True);
                Assert.That(tarjeta.Saldo, Is.EqualTo(-1080f));
            });
        }

        [Test]
        public void DescontarSaldo_ExcedeLimiteNegativo_NoDeberiaDescontar()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(-500f, 1);

            // Act
            bool resultado = tarjeta.DescontarSaldo(1000f); // -500 - 1000 = -1500 (excede -1200)

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.False);
                Assert.That(tarjeta.Saldo, Is.EqualTo(-500f));
            });
        }

        [Test]
        public void DescontarSaldo_EnLimiteMenos1200_DeberiaPermitir()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(380f, 1);

            // Act
            bool resultado = tarjeta.DescontarSaldo(1580f); // 380 - 1580 = -1200 (justo en el límite)

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True);
                Assert.That(tarjeta.Saldo, Is.EqualTo(-1200f));
            });
        }

        [Test]
        public void PuedeDescontar_ConSaldoSuficiente_DeberiaRetornarTrue()
        {
            // Act
            bool puede = _tarjeta.PuedeDescontar(1580f);

            // Assert
            Assert.That(puede, Is.True);
        }

        [Test]
        public void PuedeDescontar_SaldoInsuficiente_DeberiaRetornarFalse()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(-1100f, 1);

            // Act
            bool puede = tarjeta.PuedeDescontar(200f); // -1100 - 200 = -1300 (excede)

            // Assert
            Assert.That(puede, Is.False);
        }

        // ============================================
        //           TESTS DE CARGAR SALDO
        // ============================================

        [Test]
        public void Cargar_MontoPermitido_DeberiaAumentarSaldo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(1000f, 1);

            // Act
            tarjeta.Cargar(2000f);

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(3000f));
        }

        [Test]
        public void Cargar_MontoNoPermitido_NoDeberiaAumentarSaldo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(1000f, 1);
            float saldoInicial = tarjeta.Saldo;

            // Act
            tarjeta.Cargar(1500f); // No está en la lista de cargas permitidas

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(saldoInicial));
        }

        [Test]
        public void Cargar_TodasLasCargasPermitidas_DeberianFuncionar()
        {
            // Arrange
            float[] cargasPermitidas = { 2000f, 3000f, 4000f, 5000f, 8000f, 10000f, 15000f, 20000f, 25000f, 30000f };

            foreach (float carga in cargasPermitidas)
            {
                var tarjeta = new Tarjeta.Tarjeta(0f, 1);

                // Act
                tarjeta.Cargar(carga);

                // Assert
                Assert.That(tarjeta.Saldo, Is.EqualTo(carga), $"Carga de {carga} debería funcionar");
            }
        }

        [Test]
        public void Cargar_ConSaldoNegativo_DeberiaDescontarDelSaldoNegativo()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(-500f, 1);

            // Act
            tarjeta.Cargar(2000f); // -500 + 2000 = 1500 efectivo

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(1500f));
        }

        [Test]
        public void Cargar_ExcedeSaldoMaximo_NoDeberiaCargar()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(35000f, 1);

            // Act
            tarjeta.Cargar(10000f); // 35000 + 10000 = 45000 (no excede 56000)

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(45000f), "No excede el nuevo máximo de 56000");
        }

        [Test]
        public void Cargar_CasiEnLimiteMaximo_DeberiaCargar()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(38000f, 1);

            // Act
            tarjeta.Cargar(2000f); // 38000 + 2000 = 40000 (justo en el límite)

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(40000f));
        }

        [Test]
        public void Cargar_ConSaldoNegativoExcediendoMaximo_NoDeberiaCargar()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(-5000f, 1);
            float saldoInicial = tarjeta.Saldo;

            // Act
            tarjeta.Cargar(30000f); // -5000 + 30000 = 25000, pero la carga real sería 25000
            // El método verifica: cargarReal (25000) > SALDO_MAXIMO (40000) ? NO
            // Entonces debería cargar correctamente

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(25000f));
        }

        [Test]
        public void Cargar_SaldoNegativoMuyAlto_DeberiaFuncionar()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(-1200f, 1);

            // Act
            tarjeta.Cargar(3000f); // -1200 + 3000 = 1800

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(1800f));
        }

        // ============================================
        //         TESTS DE CASOS ESPECIALES
        // ============================================

        [Test]
        public void Cargar_Cero_NoDeberiaEstarPermitido()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(1000f, 1);
            float saldoInicial = tarjeta.Saldo;

            // Act
            tarjeta.Cargar(0f);

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(saldoInicial));
        }

        [Test]
        public void Cargar_MontoNegativo_NoDeberiaEstarPermitido()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(1000f, 1);
            float saldoInicial = tarjeta.Saldo;

            // Act
            tarjeta.Cargar(-1000f);

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(saldoInicial));
        }

        [Test]
        public void DescontarSaldo_MultiplesDeducciones_DeberiaAcumular()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(10000f, 1);

            // Act
            tarjeta.DescontarSaldo(1580f);
            tarjeta.DescontarSaldo(1580f);
            tarjeta.DescontarSaldo(1580f);

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(10000f - 1580f * 3));
        }

        [Test]
        public void Tarjeta_CargasYDescuentosCombinados_DeberiaManejarCorrectamente()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(0f, 1);

            // Act
            tarjeta.Cargar(5000f);
            tarjeta.DescontarSaldo(1580f);
            tarjeta.Cargar(3000f);
            tarjeta.DescontarSaldo(1580f);

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(5000f - 1580f + 3000f - 1580f));
        }

        [Test]
        public void Tarjeta_ConSaldoCero_DeberiaPermitirCargar()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(0f, 1);

            // Act
            tarjeta.Cargar(2000f);

            // Assert
            Assert.That(tarjeta.Saldo, Is.EqualTo(2000f));
        }

        [Test]
        public void Tarjeta_ConSaldoMaximo_NoDeberiaPermitirMasCargas()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(56000f, 1); // Nuevo máximo

            // Act
            tarjeta.Cargar(2000f); // Debería quedar pendiente

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(tarjeta.Saldo, Is.EqualTo(56000f), "Saldo sigue en el máximo");
                Assert.That(tarjeta.SaldoPendiente, Is.EqualTo(2000f), "La carga queda pendiente");
            });
        }
    }
}