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

        // ============================================
        //    TESTS DE ITERACIÓN 3 - SALDO PENDIENTE
        // ============================================

        [Test]
        public void Cargar_ExcediendoMaximo_DeberiaAcreditarHastaMaximoYDejarPendiente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(50000f, 1);

            // Act
            _tarjeta.Cargar(10000f); // 50000 + 10000 = 60000 (excede 56000)

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(56000f), "Saldo debe estar en el máximo (56000)");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(4000f), "Excedente: 60000 - 56000 = 4000");
            });
        }

        [Test]
        public void Cargar_YaEnMaximo_TodoDeberiaQuedarPendiente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(56000f, 1); // Ya está al máximo

            // Act
            _tarjeta.Cargar(20000f); // Todo debe quedar pendiente

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(56000f), "Saldo sigue en el máximo");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(20000f), "Todo queda pendiente");
            });
        }

        [Test]
        public void Cargar_VariasCargasExcediendoMaximo_DeberiaAcumularPendiente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(50000f, 1);

            // Act
            _tarjeta.Cargar(10000f); // Primera carga: 4000 pendiente
            _tarjeta.Cargar(5000f);  // Segunda carga: 5000 más pendiente

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(56000f), "Saldo en el máximo");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(9000f), "Total pendiente: 4000 + 5000 = 9000");
            });
        }

        [Test]
        public void AcreditarCarga_SinSaldoPendiente_NoDeberiaHacerNada()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(10000f, 1);

            // Act
            _tarjeta.AcreditarCarga();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(10000f), "Saldo no cambia");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(0f), "Sin saldo pendiente");
            });
        }

        [Test]
        public void AcreditarCarga_ConSaldoPendiente_DeberiaAcreditarHastaMaximo()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(54000f, 1);
            _tarjeta.Cargar(10000f); // 54000 + 10000 = 64000, acredita hasta 56000, pendiente 8000

            // Act
            _tarjeta.Saldo = 50000f; // Simular consumo
            _tarjeta.AcreditarCarga();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(56000f), "Acredita hasta el máximo");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(2000f), "Quedan 2000 pendientes: 8000 - 6000");
            });
        }

        [Test]
        public void DescontarSaldo_ConSaldoPendiente_DeberiaAcreditarAutomaticamente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(55000f, 1);
            _tarjeta.Cargar(10000f); // Saldo: 56000, Pendiente: 9000

            // Act - Realizar un viaje
            _tarjeta.DescontarSaldo(1580f); // Saldo: 54420, luego acredita automáticamente

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(56000f),
                    "Después del descuento y acreditación debería estar en 56000");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(7420f),
                    "Pendiente: 9000 - 1580 = 7420");
            });
        }

        [Test]
        public void DescontarSaldo_ConSaldoPendienteParcial_AcreditaSoloLoNecesario()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(55000f, 1);
            _tarjeta.Cargar(5000f); // Saldo: 56000, Pendiente: 4000

            // Act - Consumo pequeño
            _tarjeta.DescontarSaldo(500f); // Saldo: 55500, acredita 500

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(56000f), "Acredita lo justo para llegar al máximo");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(3500f), "Pendiente: 4000 - 500 = 3500");
            });
        }

        [Test]
        public void Cargar_ConSaldoNegativoYExcediendoMaximo_DeberiaFuncionarCorrectamente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(-1000f, 1);

            // Act
            _tarjeta.Cargar(30000f); // -1000 + 30000 = 29000 (carga real), no excede

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(29000f), "Descuenta deuda: 30000 - 1000 = 29000");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(0f), "Sin saldo pendiente");
            });
        }

        [Test]
        public void Cargar_ConSaldoNegativoYExcediendoMucho_DeberiaDejarPendiente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(-1000f, 1);

            // Act
            _tarjeta.Cargar(30000f); // -1000 + 30000 = 29000 real
            _tarjeta.Cargar(30000f); // 29000 + 30000 = 59000, acredita hasta 56000, sobra 3000

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(56000f), "Debería estar en el máximo");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(3000f), "Excedente pendiente");
            });
        }

        [Test]
        public void SaldoPendiente_DespuesDeVariosViajes_DeberiaAcreditarseProgresivamente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(54000f, 1);
            _tarjeta.Cargar(20000f); // Saldo: 56000, Pendiente: 18000

            // Act - Múltiples viajes
            _tarjeta.DescontarSaldo(1580f); // Saldo vuelve a 56000, Pendiente: 16420
            _tarjeta.DescontarSaldo(1580f); // Saldo vuelve a 56000, Pendiente: 14840
            _tarjeta.DescontarSaldo(1580f); // Saldo vuelve a 56000, Pendiente: 13260

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(56000f), "Saldo siempre en el máximo");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(13260f),
                    "Pendiente: 18000 - (1580 * 3) = 13260");
            });
        }

        [Test]
        public void SaldoPendiente_AcreditacionCompleta_DeberiaLlegarACero()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(54000f, 1);
            _tarjeta.Cargar(3000f); // Saldo: 56000, Pendiente: 1000

            // Act - Consumir más que el pendiente
            _tarjeta.DescontarSaldo(2000f); // Saldo: 54000, acredita 1000, Pendiente: 0

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(55000f), "Saldo: 54000 + 1000 = 55000");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(0f), "Ya no hay saldo pendiente");
            });
        }

        [Test]
        public void AcreditarCarga_TarjetaEnMaximo_NoDeberiaAcreditar()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(56000f, 1);
            _tarjeta.Cargar(5000f); // Pendiente: 5000

            // Act - La tarjeta ya está al máximo
            _tarjeta.AcreditarCarga();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(56000f), "Saldo no cambia");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(5000f), "Pendiente no se acredita");
            });
        }

        [Test]
        public void Cargar_ConSaldoNegativoExcediendoMaximoDespuesDeCubrir_DeberiaDejarPendiente()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(-5000f, 1);

            // Act
            _tarjeta.Cargar(30000f); // -5000 + 30000 = 25000 real (OK)
            float saldoDespuesPrimera = _tarjeta.Saldo;

            _tarjeta.Cargar(30000f); // 25000 + 30000 = 55000 (OK, no excede)
            float saldoDespuesSegunda = _tarjeta.Saldo;

            _tarjeta.Cargar(5000f); // 55000 + 5000 = 60000 (excede 56000)

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(saldoDespuesPrimera, Is.EqualTo(25000f), "Primera carga: 25000");
                Assert.That(saldoDespuesSegunda, Is.EqualTo(55000f), "Segunda carga: 55000");
                Assert.That(_tarjeta.Saldo, Is.EqualTo(56000f), "Tercera carga: máximo");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(4000f), "Pendiente: 4000");
            });
        }

        [Test]
        public void DescontarSaldo_SinSaldoPendiente_NoDeberiaInvocarAcreditar()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(5000f, 1);

            // Act
            _tarjeta.DescontarSaldo(1580f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_tarjeta.Saldo, Is.EqualTo(3420f), "Descuenta normalmente");
                Assert.That(_tarjeta.SaldoPendiente, Is.EqualTo(0f), "Sin pendiente");
            });
        }
    }
}