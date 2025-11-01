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
    }

}
