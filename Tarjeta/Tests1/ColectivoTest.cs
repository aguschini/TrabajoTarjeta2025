using NUnit.Framework;
using Tarjeta;

namespace Tests1
{
    public class ColectivoTest
    {
        private Tarjeta.Colectivo _colectivo;
        private Tarjeta.Tarjeta _tarjeta;

        [SetUp]
        public void Setup()
        {
            _colectivo = new Tarjeta.Colectivo("125");
        }

        [Test]
        public void Constructor_DeberiaCrearColectivoConLineaCorrecta()
        {
            // Assert
            Assert.That(_colectivo.Linea, Is.EqualTo("125"));
        }

        [Test]
        public void Pagar_SaldoSuficiente_DeberiaDescontarSaldo()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(2000f, 1); // Saldo suficiente

            // Act
            _colectivo.Pagar(_tarjeta);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(2000f - 1580f));
        }

        [Test]
        public void Pagar_SaldoInsuficiente_DeberiaMantenerSaldo()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(200f, 1); // Saldo insuficiente

            // Act
            _colectivo.Pagar(_tarjeta);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(200));
        }

        [Test]
        public void Pagar_SaldoExacto_DeberiaQuedarEnCero()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(1580f, 1); // Saldo exacto

            // Act
            _colectivo.Pagar(_tarjeta);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(0f));
        }
    }
}
