using NUnit.Framework;
using Tarjeta;
using System;

namespace Tests1
{
    public class BoletoTest
    {
        private Boleto _boleto;

    [SetUp]
        public void Setup()
        {
            // Crear un Boleto de prueba con valores fijos
            _boleto = new Boleto(
                fecha: new DateTime(2025, 11, 1),
                tipoTarjeta: "MedioBoletoEstudiantil",
                linea: "Linea 42",
                total: 790f,
                saldoRestante: 210f,
                id: 123
            );
        }

        [Test]
        public void Constructor_DeberiaCrearBoletoConValoresCorrectos()
        {
            Assert.That(_boleto.Fecha, Is.EqualTo(new DateTime(2025, 11, 1)));
            Assert.That(_boleto.TipoTarjeta, Is.EqualTo("MedioBoletoEstudiantil"));
            Assert.That(_boleto.LineaColectivo, Is.EqualTo("Linea 42"));
            Assert.That(_boleto.TotalAbonado, Is.EqualTo(790f));
            Assert.That(_boleto.SaldoRestante, Is.EqualTo(210f));
            Assert.That(_boleto.IdTarjeta, Is.EqualTo(123));
        }

        [Test]
        public void Propiedades_DeberianPoderModificarValores()
        {
            // Arrange
            var nuevaFecha = new DateTime(2025, 12, 1);
            var nuevoTipo = "BoletoGratuitoEstudiantil";
            var nuevaLinea = "Linea 15";
            float nuevoTotal = 0f;
            float nuevoSaldo = 500f;
            int nuevoId = 999;

            // Act
            _boleto.Fecha = nuevaFecha;
            _boleto.TipoTarjeta = nuevoTipo;
            _boleto.LineaColectivo = nuevaLinea;
            _boleto.TotalAbonado = nuevoTotal;
            _boleto.SaldoRestante = nuevoSaldo;
            _boleto.IdTarjeta = nuevoId;

            // Assert
            Assert.That(_boleto.Fecha, Is.EqualTo(nuevaFecha));
            Assert.That(_boleto.TipoTarjeta, Is.EqualTo(nuevoTipo));
            Assert.That(_boleto.LineaColectivo, Is.EqualTo(nuevaLinea));
            Assert.That(_boleto.TotalAbonado, Is.EqualTo(nuevoTotal));
            Assert.That(_boleto.SaldoRestante, Is.EqualTo(nuevoSaldo));
            Assert.That(_boleto.IdTarjeta, Is.EqualTo(nuevoId));
        }

        [Test]
        public void Boleto_ConDiferentesTiposTarjeta_DeberiaAlmacenarCorrectamente()
        {
            // Arrange & Act
            var boletoNormal = new Boleto(DateTime.Now, "Tarjeta", "125", 1580f, 1000f, 1);
            var boletoMedio = new Boleto(DateTime.Now, "MedioBoletoEstudiantil", "125", 790f, 1000f, 2);
            var boletoGratuito = new Boleto(DateTime.Now, "BoletoGratuitoEstudiantil", "125", 0f, 1000f, 3);
            var boletoFranquicia = new Boleto(DateTime.Now, "FranquiciaCompleta", "125", 0f, 1000f, 4);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boletoNormal.TipoTarjeta, Is.EqualTo("Tarjeta"));
                Assert.That(boletoMedio.TipoTarjeta, Is.EqualTo("MedioBoletoEstudiantil"));
                Assert.That(boletoGratuito.TipoTarjeta, Is.EqualTo("BoletoGratuitoEstudiantil"));
                Assert.That(boletoFranquicia.TipoTarjeta, Is.EqualTo("FranquiciaCompleta"));
            });
        }

        [Test]
        public void Boleto_ConSaldoNegativo_DeberiaAlmacenar()
        {
            // Arrange & Act
            var boleto = new Boleto(DateTime.Now, "Tarjeta", "125", 1580f, -580f, 1);

            // Assert
            Assert.That(boleto.SaldoRestante, Is.EqualTo(-580f), "Debería permitir saldo negativo");
        }

        [Test]
        public void Boleto_ConDiferentesLineas_DeberiaAlmacenarCorrectamente()
        {
            // Arrange & Act
            var boleto1 = new Boleto(DateTime.Now, "Tarjeta", "125", 1580f, 1000f, 1);
            var boleto2 = new Boleto(DateTime.Now, "Tarjeta", "42", 1580f, 1000f, 1);
            var boleto3 = new Boleto(DateTime.Now, "Tarjeta", "K", 1580f, 1000f, 1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.LineaColectivo, Is.EqualTo("125"));
                Assert.That(boleto2.LineaColectivo, Is.EqualTo("42"));
                Assert.That(boleto3.LineaColectivo, Is.EqualTo("K"));
            });
        }

        [Test]
        public void Boleto_FechasDiferentes_DeberiaAlmacenarCorrectamente()
        {
            // Arrange
            var fecha1 = new DateTime(2024, 1, 1, 10, 30, 0);
            var fecha2 = new DateTime(2024, 12, 31, 23, 59, 59);

            // Act
            var boleto1 = new Boleto(fecha1, "Tarjeta", "125", 1580f, 1000f, 1);
            var boleto2 = new Boleto(fecha2, "Tarjeta", "125", 1580f, 1000f, 1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.Fecha, Is.EqualTo(fecha1));
                Assert.That(boleto2.Fecha, Is.EqualTo(fecha2));
            });
        }

        [Test]
        public void Boleto_ConSaldoCero_DeberiaAlmacenar()
        {
            // Arrange & Act
            var boleto = new Boleto(DateTime.Now, "Tarjeta", "125", 1580f, 0f, 1);

            // Assert
            Assert.That(boleto.SaldoRestante, Is.EqualTo(0f));
        }

        [Test]
        public void Boleto_ConTotalCero_DeberiaAlmacenar()
        {
            // Arrange & Act
            var boleto = new Boleto(DateTime.Now, "BoletoGratuitoEstudiantil", "125", 0f, 1000f, 1);

            // Assert
            Assert.That(boleto.TotalAbonado, Is.EqualTo(0f));
        }

        [Test]
        public void Boleto_ConDiferentesIds_DeberiaAlmacenarCorrectamente()
        {
            // Arrange & Act
            var boleto1 = new Boleto(DateTime.Now, "Tarjeta", "125", 1580f, 1000f, 1);
            var boleto2 = new Boleto(DateTime.Now, "Tarjeta", "125", 1580f, 1000f, 999);
            var boleto3 = new Boleto(DateTime.Now, "Tarjeta", "125", 1580f, 1000f, 12345);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.IdTarjeta, Is.EqualTo(1));
                Assert.That(boleto2.IdTarjeta, Is.EqualTo(999));
                Assert.That(boleto3.IdTarjeta, Is.EqualTo(12345));
            });
        }
    }

}
