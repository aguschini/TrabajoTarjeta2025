using NUnit.Framework;
using Tarjeta;
using TrabajoTarjeta;

namespace Tests1
{
    public class ColectivoTest
    {
        private Tarjeta.Colectivo _colectivo;
        private Tarjeta.Tarjeta _tarjeta;
        private TiempoFalso _tiempo;

        [SetUp]
        public void Setup()
        {
            _colectivo = new Tarjeta.Colectivo("125");
            _tiempo = new TiempoFalso(2024, 11, 1);
        }

        [Test]
        public void Constructor_DeberiaCrearColectivoConLineaCorrecta()
        {
            // Assert
            Assert.That(_colectivo.Linea, Is.EqualTo("125"));
        }

        [Test]
        public void PagarCon_SaldoSuficiente_DeberiaDescontarSaldoYRetornarBoleto()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(2000f, 1);

            // Act
            Boleto boleto = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(2000f - 1580f));
            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto.TotalAbonado, Is.EqualTo(1580f));
            Assert.That(boleto.SaldoRestante, Is.EqualTo(420f));
            Assert.That(boleto.LineaColectivo, Is.EqualTo("125"));
            Assert.That(boleto.TipoTarjeta, Is.EqualTo("Tarjeta"));
            Assert.That(boleto.IdTarjeta, Is.EqualTo(1));
        }

        [Test]
        public void PagarCon_SaldoInsuficiente_DeberiaMantenerSaldoYRetornarNull()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(200f, 1);

            // Act
            Boleto boleto = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(200f));
            Assert.That(boleto, Is.Null);
        }

        [Test]
        public void PagarCon_SaldoExacto_DeberiaQuedarEnCeroYRetornarBoleto()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(1580f, 1);

            // Act
            Boleto boleto = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(0f));
            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto.TotalAbonado, Is.EqualTo(1580f));
            Assert.That(boleto.SaldoRestante, Is.EqualTo(0f));
        }

        [Test]
        public void PagarCon_MedioBoleto_DeberiaDescontarMitadDePrecio()
        {
            // Arrange
            var tarjetaMedioBoleto = new MedioBoletoEstudiantil(2000f, 2);

            // Act
            Boleto boleto = _colectivo.PagarCon(tarjetaMedioBoleto, _tiempo);

            // Assert
            Assert.That(tarjetaMedioBoleto.Saldo, Is.EqualTo(2000f - 790f));
            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto.TotalAbonado, Is.EqualTo(790f));
        }

        [Test]
        public void PagarCon_BoletoGratuito_NoDeberiaDescontarSaldo()
        {
            // Arrange
            var tarjetaGratuita = new BoletoGratuitoEstudiantil(2000f, 3);

            // Act
            Boleto boleto = _colectivo.PagarCon(tarjetaGratuita, _tiempo);

            // Assert
            Assert.That(tarjetaGratuita.Saldo, Is.EqualTo(2000f));
            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto.TotalAbonado, Is.EqualTo(0f));
        }

        [Test]
        public void PagarCon_FranquiciaCompleta_NoDeberiaDescontarSaldo()
        {
            // Arrange
            var tarjetaFranquicia = new FranquiciaCompleta(2000f, 4);

            // Act
            Boleto boleto = _colectivo.PagarCon(tarjetaFranquicia, _tiempo);

            // Assert
            Assert.That(tarjetaFranquicia.Saldo, Is.EqualTo(2000f));
            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto.TotalAbonado, Is.EqualTo(0f));
        }

        [Test]
        public void Pagar_LegacyMethod_DeberiaSeguirFuncionando()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(2000f, 1);

            // Act
            _colectivo.Pagar(_tarjeta, _tiempo);

            // Assert
            Assert.That(_tarjeta.Saldo, Is.EqualTo(2000f - 1580f));
        }

        [Test]
        public void PagarCon_DeberiaTenerFechaCorrecta()
        {
            // Arrange
            _tarjeta = new Tarjeta.Tarjeta(2000f, 1);
            var fechaEsperada = new System.DateTime(2024, 11, 1);

            // Act
            Boleto boleto = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            Assert.That(boleto.Fecha, Is.EqualTo(fechaEsperada));
        }
    }
}