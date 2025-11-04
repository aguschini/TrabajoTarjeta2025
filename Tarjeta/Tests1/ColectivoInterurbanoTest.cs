using NUnit.Framework;
using System;
using TrabajoTarjeta;
using Tarjeta;

namespace Tests1
{
    public class ColectivoInterurbanoTest
    {
        private ColectivoInterurbano _colectivoInterurbano;
        private TiempoFalso _tiempo;

        [SetUp]
        public void Setup()
        {
            _colectivoInterurbano = new ColectivoInterurbano("Gálvez");
            _tiempo = new TiempoFalso(2024, 11, 4, 10, 0); // Lunes a las 10:00
        }

        #region Tests con Tarjeta Normal

        [Test]
        public void ColectivoInterurbano_TarjetaNormal_DeberiaCobrar3000()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(5000f, 1);
            float saldoInicial = tarjeta.Saldo;

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto.TotalAbonado, Is.EqualTo(3000f));
            Assert.That(tarjeta.Saldo, Is.EqualTo(saldoInicial - 3000f));
        }

        [Test]
        public void ColectivoInterurbano_TarjetaNormalSinSaldo_DeberiaRetornarNull()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(1000f, 1);

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto, Is.Null);
        }

        [Test]
        public void ColectivoInterurbano_TarjetaNormalConSaldoNegativo_DeberiaPagar()
        {
            // Arrange
            // Saldo inicial: 1900 permite pagar 3000 quedando en -1100 (dentro del límite de -1200)
            var tarjeta = new Tarjeta.Tarjeta(1900f, 1);

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto, Is.Not.Null);
            Assert.That(tarjeta.Saldo, Is.EqualTo(-1100f)); // 1900 - 3000 = -1100
        }

        #endregion

        #region Tests con Medio Boleto Estudiantil

        [Test]
        public void ColectivoInterurbano_MedioBoletoEstudiantil_DeberiaCobrar1500()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 2);

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto.TotalAbonado, Is.EqualTo(1500f)); // 3000 / 2
        }

        [Test]
        public void ColectivoInterurbano_MedioBoletoEstudiantil_DosViajesEnElDia_DeberiaPagar1500CadaUno()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(10000f, 2);

            // Act - Primer viaje
            Boleto boleto1 = _colectivoInterurbano.PagarCon(medioBoleto, _tiempo);
            
            // Avanzar 6 minutos
            _tiempo.AgregarMinutos(6);
            
            // Segundo viaje
            Boleto boleto2 = _colectivoInterurbano.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.That(boleto1.TotalAbonado, Is.EqualTo(1500f));
            Assert.That(boleto2.TotalAbonado, Is.EqualTo(1500f));
        }

        [Test]
        public void ColectivoInterurbano_MedioBoletoEstudiantil_TresViajesEnElDia_TerceroPagaCompleto()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(15000f, 2);

            // Act - Primer viaje
            Boleto boleto1 = _colectivoInterurbano.PagarCon(medioBoleto, _tiempo);
            _tiempo.AgregarMinutos(6);
            
            // Segundo viaje
            Boleto boleto2 = _colectivoInterurbano.PagarCon(medioBoleto, _tiempo);
            _tiempo.AgregarMinutos(6);
            
            // Tercer viaje
            Boleto boleto3 = _colectivoInterurbano.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.That(boleto1.TotalAbonado, Is.EqualTo(1500f));
            Assert.That(boleto2.TotalAbonado, Is.EqualTo(1500f));
            Assert.That(boleto3.TotalAbonado, Is.EqualTo(3000f)); // Paga completo
        }

        [Test]
        public void ColectivoInterurbano_MedioBoletoEstudiantil_FueraDe5Minutos_DeberiaRechazar()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 2);

            // Act - Primer viaje
            Boleto boleto1 = _colectivoInterurbano.PagarCon(medioBoleto, _tiempo);
            
            // Avanzar solo 4 minutos (menos de 5)
            _tiempo.AgregarMinutos(4);
            
            // Intentar segundo viaje
            Boleto boleto2 = _colectivoInterurbano.PagarCon(medioBoleto, _tiempo);

            // Assert
            Assert.That(boleto1, Is.Not.Null);
            Assert.That(boleto2, Is.Null); // Rechazado por no pasar 5 minutos
        }

        [Test]
        public void ColectivoInterurbano_MedioBoletoEstudiantil_FinDeSemana_DeberiaRechazar()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 2);
            var tiempoSabado = new TiempoFalso(2024, 11, 9, 10, 0); // Sábado

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(medioBoleto, tiempoSabado);

            // Assert
            Assert.That(boleto, Is.Null);
        }

        [Test]
        public void ColectivoInterurbano_MedioBoletoEstudiantil_FueraDeHorario_DeberiaRechazar()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(5000f, 2);
            var tiempoNoche = new TiempoFalso(2024, 11, 4, 23, 0); // Lunes a las 23:00

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(medioBoleto, tiempoNoche);

            // Assert
            Assert.That(boleto, Is.Null);
        }

        #endregion

        #region Tests con Boleto Gratuito Estudiantil

        [Test]
        public void ColectivoInterurbano_BoletoGratuitoEstudiantil_DeberiaCobrarCero()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(1000f, 3);
            float saldoInicial = boletoGratuito.Saldo;

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto.TotalAbonado, Is.EqualTo(0f));
            Assert.That(boletoGratuito.Saldo, Is.EqualTo(saldoInicial)); // No descontó
        }

        [Test]
        public void ColectivoInterurbano_BoletoGratuitoEstudiantil_DosViajesGratis()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(1000f, 3);

            // Act
            Boleto boleto1 = _colectivoInterurbano.PagarCon(boletoGratuito, _tiempo);
            Boleto boleto2 = _colectivoInterurbano.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.That(boleto1.TotalAbonado, Is.EqualTo(0f));
            Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f));
        }

        [Test]
        public void ColectivoInterurbano_BoletoGratuitoEstudiantil_TercerViajePagaCompleto()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 3);

            // Act
            Boleto boleto1 = _colectivoInterurbano.PagarCon(boletoGratuito, _tiempo);
            Boleto boleto2 = _colectivoInterurbano.PagarCon(boletoGratuito, _tiempo);
            Boleto boleto3 = _colectivoInterurbano.PagarCon(boletoGratuito, _tiempo);

            // Assert
            Assert.That(boleto1.TotalAbonado, Is.EqualTo(0f));
            Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f));
            Assert.That(boleto3.TotalAbonado, Is.EqualTo(3000f)); // Paga completo
        }

        [Test]
        public void ColectivoInterurbano_BoletoGratuitoEstudiantil_FinDeSemana_DeberiaRechazar()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 3);
            var tiempoDomingo = new TiempoFalso(2024, 11, 10, 10, 0); // Domingo

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(boletoGratuito, tiempoDomingo);

            // Assert
            Assert.That(boleto, Is.Null);
        }

        [Test]
        public void ColectivoInterurbano_BoletoGratuitoEstudiantil_FueraDeHorario_DeberiaRechazar()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(5000f, 3);
            var tiempoMadrugada = new TiempoFalso(2024, 11, 4, 5, 0); // Lunes a las 5:00

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(boletoGratuito, tiempoMadrugada);

            // Assert
            Assert.That(boleto, Is.Null);
        }

        #endregion

        #region Tests con Franquicia Completa

        [Test]
        public void ColectivoInterurbano_FranquiciaCompleta_DeberiaCobrarCero()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(1000f, 4);
            float saldoInicial = franquicia.Saldo;

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.That(boleto, Is.Not.Null);
            Assert.That(boleto.TotalAbonado, Is.EqualTo(0f));
            Assert.That(franquicia.Saldo, Is.EqualTo(saldoInicial));
        }

        [Test]
        public void ColectivoInterurbano_FranquiciaCompleta_VariosViajes_SiempreGratis()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(1000f, 4);

            // Act
            Boleto boleto1 = _colectivoInterurbano.PagarCon(franquicia, _tiempo);
            Boleto boleto2 = _colectivoInterurbano.PagarCon(franquicia, _tiempo);
            Boleto boleto3 = _colectivoInterurbano.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.That(boleto1.TotalAbonado, Is.EqualTo(0f));
            Assert.That(boleto2.TotalAbonado, Is.EqualTo(0f));
            Assert.That(boleto3.TotalAbonado, Is.EqualTo(0f));
        }

        [Test]
        public void ColectivoInterurbano_FranquiciaCompleta_FinDeSemana_DeberiaRechazar()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(1000f, 4);
            var tiempoSabado = new TiempoFalso(2024, 11, 9, 10, 0); // Sábado

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(franquicia, tiempoSabado);

            // Assert
            Assert.That(boleto, Is.Null);
        }

        [Test]
        public void ColectivoInterurbano_FranquiciaCompleta_FueraDeHorario_DeberiaRechazar()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(1000f, 4);
            var tiempoNoche = new TiempoFalso(2024, 11, 4, 22, 30); // Lunes a las 22:30

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(franquicia, tiempoNoche);

            // Assert
            Assert.That(boleto, Is.Null);
        }

        #endregion

        #region Tests de Boleto

        [Test]
        public void ColectivoInterurbano_BoletoDeberiaTenerDatosCorrectos()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(10000f, 123);

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto.LineaColectivo, Is.EqualTo("Gálvez"));
            Assert.That(boleto.TipoTarjeta, Is.EqualTo("Tarjeta"));
            Assert.That(boleto.IdTarjeta, Is.EqualTo(123));
            Assert.That(boleto.Fecha, Is.EqualTo(new DateTime(2024, 11, 4, 10, 0, 0)));
        }

        [Test]
        public void ColectivoInterurbano_BoletoDeberiaTenerSaldoRestanteCorrecto()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(10000f, 1);

            // Act
            Boleto boleto = _colectivoInterurbano.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto.SaldoRestante, Is.EqualTo(7000f)); // 10000 - 3000
        }

        #endregion

        #region Tests de Uso Frecuente

        [Test]
        public void ColectivoInterurbano_UsoFrecuente_Viaje30DeberiaTener20PorcientoDescuento()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(100000f, 1);
            
            // Simular 29 viajes
            for (int i = 0; i < 29; i++)
            {
                _colectivoInterurbano.PagarCon(tarjeta, _tiempo);
                _tiempo.AgregarMinutos(10);
            }

            // Act - Viaje 30
            Boleto boleto30 = _colectivoInterurbano.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto30.TotalAbonado, Is.EqualTo(2400f)); // 3000 * 0.8
        }

        [Test]
        public void ColectivoInterurbano_UsoFrecuente_Viaje60DeberiaTener25PorcientoDescuento()
        {
            // Arrange
            var tarjeta = new Tarjeta.Tarjeta(200000f, 1);
            
            // Simular 59 viajes
            for (int i = 0; i < 59; i++)
            {
                _colectivoInterurbano.PagarCon(tarjeta, _tiempo);
                _tiempo.AgregarMinutos(10);
            }

            // Act - Viaje 60
            Boleto boleto60 = _colectivoInterurbano.PagarCon(tarjeta, _tiempo);

            // Assert
            Assert.That(boleto60.TotalAbonado, Is.EqualTo(2250f)); // 3000 * 0.75
        }

        #endregion

        #region Tests de Comparación con Colectivo Urbano

        [Test]
        public void ColectivoInterurbano_TarjetaNormal_DeberiaCobrarMasQueUrbano()
        {
            // Arrange
            var tarjetaInterurbano = new Tarjeta.Tarjeta(10000f, 1);
            var tarjetaUrbano = new Tarjeta.Tarjeta(10000f, 2);
            var colectivoUrbano = new Colectivo("125");

            // Act
            Boleto boletoInterurbano = _colectivoInterurbano.PagarCon(tarjetaInterurbano, _tiempo);
            Boleto boletoUrbano = colectivoUrbano.PagarCon(tarjetaUrbano, _tiempo);

            // Assert
            Assert.That(boletoInterurbano.TotalAbonado, Is.GreaterThan(boletoUrbano.TotalAbonado));
            Assert.That(boletoInterurbano.TotalAbonado, Is.EqualTo(3000f));
            Assert.That(boletoUrbano.TotalAbonado, Is.EqualTo(1580f));
        }

        #endregion
    }
}
