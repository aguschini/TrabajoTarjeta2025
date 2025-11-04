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

        #region Tests de TiempoFalso - Constructor con Fecha

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
        public void TiempoFalso_ConstructorConFecha_DeberiaTenerHoraCero()
        {
            // Act
            DateTime fecha = _tiempoFalso.Now();

            // Assert
            Assert.That(fecha.Hour, Is.EqualTo(0));
            Assert.That(fecha.Minute, Is.EqualTo(0));
            Assert.That(fecha.Second, Is.EqualTo(0));
        }

        #endregion

        #region Tests de TiempoFalso - Constructor con Fecha y Hora

        [Test]
        public void TiempoFalso_ConstructorConHora_DeberiaRetornarFechaYHoraCorrecta()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15, 14, 30);
            DateTime fechaEsperada = new DateTime(2024, 11, 15, 14, 30, 0);

            // Act
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha, Is.EqualTo(fechaEsperada));
        }

        [Test]
        public void TiempoFalso_ConstructorConHoraMedianoche_DeberiaFuncionar()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 1, 1, 0, 0);

            // Act
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Hour, Is.EqualTo(0));
            Assert.That(fecha.Minute, Is.EqualTo(0));
        }

        [Test]
        public void TiempoFalso_ConstructorConHora23_59_DeberiaFuncionar()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 12, 31, 23, 59);

            // Act
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Hour, Is.EqualTo(23));
            Assert.That(fecha.Minute, Is.EqualTo(59));
        }

        #endregion

        #region Tests de AgregarDias

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
        public void TiempoFalso_AgregarDias_CambioDeAño_DeberiaFuncionar()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 12, 31);

            // Act
            tiempo.AgregarDias(1);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Year, Is.EqualTo(2025));
            Assert.That(fecha.Month, Is.EqualTo(1));
            Assert.That(fecha.Day, Is.EqualTo(1));
        }

        [Test]
        public void TiempoFalso_AgregarDiasNegativo_DeberiaRetroceder()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15);

            // Act
            tiempo.AgregarDias(-5);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha, Is.EqualTo(new DateTime(2024, 11, 10)));
        }

        [Test]
        public void TiempoFalso_AgregarCeroDias_NoDeberiaModificarFecha()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15);
            DateTime fechaInicial = tiempo.Now();

            // Act
            tiempo.AgregarDias(0);
            DateTime fechaFinal = tiempo.Now();

            // Assert
            Assert.That(fechaFinal, Is.EqualTo(fechaInicial));
        }

        #endregion

        #region Tests de AgregarMinutos

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

        [Test]
        public void TiempoFalso_AgregarMinutosCruzandoHora_DeberiaFuncionar()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 1, 10, 45);

            // Act
            tiempo.AgregarMinutos(30);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Hour, Is.EqualTo(11));
            Assert.That(fecha.Minute, Is.EqualTo(15));
        }

        [Test]
        public void TiempoFalso_AgregarMinutosNegativo_DeberiaRetroceder()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15, 10, 30);

            // Act
            tiempo.AgregarMinutos(-15);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Hour, Is.EqualTo(10));
            Assert.That(fecha.Minute, Is.EqualTo(15));
        }

        [Test]
        public void TiempoFalso_AgregarCeroMinutos_NoDeberiaModificarTiempo()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15, 10, 30);
            DateTime tiempoInicial = tiempo.Now();

            // Act
            tiempo.AgregarMinutos(0);
            DateTime tiempoFinal = tiempo.Now();

            // Assert
            Assert.That(tiempoFinal, Is.EqualTo(tiempoInicial));
        }

        #endregion

        #region Tests de AgregarHoras

        [Test]
        public void TiempoFalso_AgregarHoras_DeberiaAvanzarTiempo()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 1, 10, 0);

            // Act
            tiempo.AgregarHoras(5);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Hour, Is.EqualTo(15));
            Assert.That(fecha.Minute, Is.EqualTo(0));
        }

        [Test]
        public void TiempoFalso_AgregarHorasCruzandoDia_DeberiaFuncionar()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 1, 20, 0);

            // Act
            tiempo.AgregarHoras(6);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Day, Is.EqualTo(2));
            Assert.That(fecha.Hour, Is.EqualTo(2));
        }

        [Test]
        public void TiempoFalso_AgregarHorasNegativo_DeberiaRetroceder()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15, 10, 0);

            // Act
            tiempo.AgregarHoras(-3);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Hour, Is.EqualTo(7));
        }

        [Test]
        public void TiempoFalso_AgregarCeroHoras_NoDeberiaModificarTiempo()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15, 10, 30);
            DateTime tiempoInicial = tiempo.Now();

            // Act
            tiempo.AgregarHoras(0);
            DateTime tiempoFinal = tiempo.Now();

            // Assert
            Assert.That(tiempoFinal, Is.EqualTo(tiempoInicial));
        }

        [Test]
        public void TiempoFalso_Agregar24Horas_DeberiaAvanzarUnDia()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 1, 10, 0);

            // Act
            tiempo.AgregarHoras(24);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Day, Is.EqualTo(2));
            Assert.That(fecha.Hour, Is.EqualTo(10));
        }

        #endregion

        #region Tests de EstablecerHora

        [Test]
        public void TiempoFalso_EstablecerHora_DeberiaCambiarHoraManteniendonFecha()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15, 10, 30);

            // Act
            tiempo.EstablecerHora(14, 45);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Year, Is.EqualTo(2024));
            Assert.That(fecha.Month, Is.EqualTo(11));
            Assert.That(fecha.Day, Is.EqualTo(15));
            Assert.That(fecha.Hour, Is.EqualTo(14));
            Assert.That(fecha.Minute, Is.EqualTo(45));
        }

        [Test]
        public void TiempoFalso_EstablecerHoraMedianoche_DeberiaFuncionar()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15, 10, 30);

            // Act
            tiempo.EstablecerHora(0, 0);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Hour, Is.EqualTo(0));
            Assert.That(fecha.Minute, Is.EqualTo(0));
        }

        [Test]
        public void TiempoFalso_EstablecerHora23_59_DeberiaFuncionar()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15, 10, 30);

            // Act
            tiempo.EstablecerHora(23, 59);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Hour, Is.EqualTo(23));
            Assert.That(fecha.Minute, Is.EqualTo(59));
        }

        [Test]
        public void TiempoFalso_EstablecerHoraVariasVeces_DeberiaTomarUltimoValor()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 15, 10, 30);

            // Act
            tiempo.EstablecerHora(8, 0);
            tiempo.EstablecerHora(14, 30);
            tiempo.EstablecerHora(18, 45);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Hour, Is.EqualTo(18));
            Assert.That(fecha.Minute, Is.EqualTo(45));
        }

        #endregion

        #region Tests de Combinaciones

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
        public void TiempoFalso_CombinacionHorasMinutosDias_DeberiaFuncionarCorrectamente()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 1, 10, 0);

            // Act
            tiempo.AgregarHoras(2);
            tiempo.AgregarMinutos(30);
            tiempo.AgregarDias(1);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha, Is.EqualTo(new DateTime(2024, 11, 2, 12, 30, 0)));
        }

        [Test]
        public void TiempoFalso_EstablecerHoraDespuesDeAgregar_DeberiaMantenerFecha()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 11, 1, 10, 0);

            // Act
            tiempo.AgregarDias(5);
            tiempo.EstablecerHora(14, 30);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha, Is.EqualTo(new DateTime(2024, 11, 6, 14, 30, 0)));
        }

        #endregion

        #region Tests de Tiempo Real

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
        public void Tiempo_Now_LlamadasConsecutivas_DeberianTenerDiferenciaMinima()
        {
            // Act
            DateTime tiempo1 = _tiempoReal.Now();
            DateTime tiempo2 = _tiempoReal.Now();

            // Assert
            TimeSpan diferencia = tiempo2 - tiempo1;
            Assert.That(diferencia.TotalMilliseconds, Is.LessThan(100)); // Menos de 100ms
        }

        [Test]
        public void Tiempo_Constructor_DeberiaCrearInstanciaValida()
        {
            // Act
            var tiempo = new Tiempo();
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(tiempo, Is.Not.Null);
            Assert.That(fecha, Is.Not.EqualTo(default(DateTime)));
        }

        #endregion

        #region Tests de Integración con Colectivo

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
        public void TiempoFalso_ConHora_BoletosDeberiaTenerHoraCorrecta()
        {
            // Arrange
            var colectivo = new Colectivo("125");
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);
            var tiempo = new TiempoFalso(2024, 11, 15, 14, 30);

            // Act
            Boleto boleto = colectivo.PagarCon(tarjeta, tiempo);

            // Assert
            Assert.That(boleto.Fecha.Hour, Is.EqualTo(14));
            Assert.That(boleto.Fecha.Minute, Is.EqualTo(30));
        }

        #endregion

        #region Tests de Casos Extremos

        [Test]
        public void TiempoFalso_AñoBisiesto_DeberiaManejarCorrectamente()
        {
            // Arrange - 2024 es año bisiesto
            var tiempo = new TiempoFalso(2024, 2, 28);

            // Act
            tiempo.AgregarDias(1);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Day, Is.EqualTo(29)); // 29 de febrero existe en año bisiesto
        }

        [Test]
        public void TiempoFalso_AñoNoBisiesto_DeberiaSaltarA1DeMarzo()
        {
            // Arrange - 2023 no es año bisiesto
            var tiempo = new TiempoFalso(2023, 2, 28);

            // Act
            tiempo.AgregarDias(1);
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Month, Is.EqualTo(3));
            Assert.That(fecha.Day, Is.EqualTo(1));
        }

        [Test]
        public void TiempoFalso_AgregarGranCantidadDeDias_DeberiaFuncionar()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 1, 1);

            // Act
            tiempo.AgregarDias(365); // Un año completo (2024 es bisiesto, tiene 366 días)
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Year, Is.EqualTo(2024));
            Assert.That(fecha.Month, Is.EqualTo(12));
            Assert.That(fecha.Day, Is.EqualTo(31)); // 366 días desde 1/1/2024
        }

        [Test]
        public void TiempoFalso_AgregarGranCantidadDeMinutos_DeberiaFuncionar()
        {
            // Arrange
            var tiempo = new TiempoFalso(2024, 1, 1, 0, 0);

            // Act
            tiempo.AgregarMinutos(60 * 24 * 7); // Una semana en minutos
            DateTime fecha = tiempo.Now();

            // Assert
            Assert.That(fecha.Day, Is.EqualTo(8));
        }

        #endregion
    }
}
