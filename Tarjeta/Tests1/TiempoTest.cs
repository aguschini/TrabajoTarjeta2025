using NUnit.Framework;
using System;
using TrabajoTarjeta;
using Tarjeta;

namespace Tests1
{
    public class TiempoTest
    {
        private TiempoFalso _tiempoFalso;
        private Tiempo _tiempoReal;

        [SetUp]
        public void Setup()
        {
            _tiempoFalso = new TiempoFalso(2024, 11, 1);
            _tiempoReal = new Tiempo();
        }

        [Test]
        public void TiempoFalso_DeberiaRetornarFechaInicial()
        {
            // Arrange
            DateTime fechaEsperada = new DateTime(2024, 11, 1);

            // Act
            DateTime fecha = _tiempoFalso.Now();

            // Assert
            Assert.That(fecha, Is.EqualTo(fechaEsperada));
        }

        [Test]
        public void TiempoFalso_AgregarDias_DeberiaAvanzarFecha()
        {
            // Arrange
            DateTime fechaInicial = _tiempoFalso.Now();

            // Act
            _tiempoFalso.AgregarDias(5);
            DateTime fechaDespues = _tiempoFalso.Now();

            // Assert
            Assert.That((fechaDespues - fechaInicial).Days, Is.EqualTo(5));
            Assert.That(fechaDespues, Is.EqualTo(new DateTime(2024, 11, 6)));
        }

        [Test]
        public void TiempoFalso_AgregarMinutos_DeberiaAvanzarTiempo()
        {
            // Arrange
            DateTime fechaInicial = _tiempoFalso.Now();

            // Act
            _tiempoFalso.AgregarMinutos(30);
            DateTime fechaDespues = _tiempoFalso.Now();

            // Assert
            Assert.That((fechaDespues - fechaInicial).TotalMinutes, Is.EqualTo(30));
        }

        [Test]
        public void TiempoFalso_MultiplesAgregarDias_DeberiaAcumular()
        {
            // Arrange & Act
            _tiempoFalso.AgregarDias(2);
            _tiempoFalso.AgregarDias(3);
            DateTime fecha = _tiempoFalso.Now();

            // Assert
            Assert.That(fecha, Is.EqualTo(new DateTime(2024, 11, 6)));
        }

        [Test]
        public void TiempoFalso_AgregarMinutosYDias_DeberiaAcumularCorrectamente()
        {
            // Act
            _tiempoFalso.AgregarMinutos(30);
            _tiempoFalso.AgregarDias(1);
            _tiempoFalso.AgregarMinutos(15);
            DateTime fecha = _tiempoFalso.Now();

            // Assert
            DateTime fechaEsperada = new DateTime(2024, 11, 2, 0, 45, 0);
            Assert.That(fecha, Is.EqualTo(fechaEsperada));
        }

        [Test]
        public void Tiempo_Now_DeberiaRetornarFechaActual()
        {
            // Act
            DateTime ahora = _tiempoReal.Now();
            DateTime sistemaAhora = DateTime.Now;

            // Assert - Debería ser aproximadamente la misma hora (diferencia menor a 1 segundo)
            Assert.That((sistemaAhora - ahora).TotalSeconds, Is.LessThan(1));
        }

        [Test]
        public void TiempoFalso_UsadoEnColectivo_DeberiaTenerFechaCorrecta()
        {
            // Arrange
            var colectivo = new Colectivo("125");
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);
            var tiempo = new TiempoFalso(2024, 10, 15);

            // Act
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempo);

            // Assert
            Assert.That(boleto.Fecha, Is.EqualTo(new DateTime(2024, 10, 15)));
        }

        [Test]
        public void TiempoFalso_DosBoletosConDiferenciaDeTiempo_DeberiaTenerFechasDiferentes()
        {
            // Arrange
            var colectivo = new Colectivo("125");
            var tarjeta = new Tarjeta.Tarjeta(10000f, 1);
            var tiempo = new TiempoFalso(2024, 11, 1);

            // Act - Primer boleto
            Boleto boleto1 = colectivo.PagarCon(tarjeta, tiempo);

            // Avanzar 10 minutos
            tiempo.AgregarMinutos(10);

            // Act - Segundo boleto
            Boleto boleto2 = colectivo.PagarCon(tarjeta, tiempo);

            // Assert
            TimeSpan diferencia = boleto2.Fecha - boleto1.Fecha;
            Assert.That(diferencia.TotalMinutes, Is.EqualTo(10));
        }

        [Test]
        public void TiempoFalso_CambioDeDia_DeberiaReflejarse()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 30); // Último día de noviembre

            // Act
            tiempo.AgregarDias(1); // Debería pasar a diciembre

            // Assert
            DateTime fecha = tiempo.Now();
            Assert.That(fecha.Month, Is.EqualTo(12));
            Assert.That(fecha.Day, Is.EqualTo(1));
        }

        [Test]
        public void TiempoFalso_AgregarMinutosCruzandoDia_DeberiaFuncionar()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 1); // 00:00 del día 1

            // Act
            tiempo.AgregarMinutos(1440); // 24 horas = 1440 minutos

            // Assert
            DateTime fecha = tiempo.Now();
            Assert.That(fecha.Day, Is.EqualTo(2));
            Assert.That(fecha.Hour, Is.EqualTo(0));
        }
    }
}