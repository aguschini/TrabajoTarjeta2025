using NUnit.Framework;
using Tarjeta;
using TrabajoTarjeta;

namespace Tests1
{
    public class UsoFrecuenteTest
    {
        private Colectivo _colectivo;
        private TiempoFalso _tiempo;
        private Tarjeta.Tarjeta _tarjeta;

        [SetUp]
        public void Setup()
        {
            _colectivo = new Colectivo("125");
            _tiempo = new TiempoFalso(2024, 11, 1);
            _tarjeta = new Tarjeta.Tarjeta(100000f, 1);
        }

        [Test]
        public void UsoFrecuente_Viajes1a29_TarifaNormal()
        {
            // Act - Primer viaje
            Boleto boleto1 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Act - Viajes 2 al 28
            for (int i = 2; i <= 28; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Viaje 29
            Boleto boleto29 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto1.TotalAbonado, Is.EqualTo(1580f),
                    "Viaje 1: tarifa normal");
                Assert.That(boleto29.TotalAbonado, Is.EqualTo(1580f),
                    "Viaje 29: tarifa normal");
                Assert.That(_tarjeta.ViajesMes, Is.EqualTo(29));
            });
        }

        [Test]
        public void UsoFrecuente_Viajes30a59_Descuento20Porciento()
        {
            // Arrange - Hacer 29 viajes
            for (int i = 1; i <= 29; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Viaje 30 (primer viaje con descuento)
            Boleto boleto30 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Act - Viajes 31 al 58
            for (int i = 31; i <= 58; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Viaje 59
            Boleto boleto59 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            float montoEsperado = 1580f * 0.80f; // $1264
            Assert.Multiple(() =>
            {
                Assert.That(boleto30.TotalAbonado, Is.EqualTo(montoEsperado),
                    "Viaje 30: 20% descuento = $1264");
                Assert.That(boleto59.TotalAbonado, Is.EqualTo(montoEsperado),
                    "Viaje 59: 20% descuento = $1264");
                Assert.That(_tarjeta.ViajesMes, Is.EqualTo(59));
            });
        }

        [Test]
        public void UsoFrecuente_Viajes60a80_Descuento25Porciento()
        {
            // Arrange - Hacer 59 viajes
            for (int i = 1; i <= 59; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Viaje 60 (primer viaje con 25% descuento)
            Boleto boleto60 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Act - Viajes 61 al 79
            for (int i = 61; i <= 79; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Viaje 80
            Boleto boleto80 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            float montoEsperado = 1580f * 0.75f; // $1185
            Assert.Multiple(() =>
            {
                Assert.That(boleto60.TotalAbonado, Is.EqualTo(montoEsperado),
                    "Viaje 60: 25% descuento = $1185");
                Assert.That(boleto80.TotalAbonado, Is.EqualTo(montoEsperado),
                    "Viaje 80: 25% descuento = $1185");
                Assert.That(_tarjeta.ViajesMes, Is.EqualTo(80));
            });
        }

        [Test]
        public void UsoFrecuente_Viaje81EnAdelante_TarifaNormal()
        {
            // Arrange - Hacer 80 viajes
            for (int i = 1; i <= 80; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Viaje 81
            Boleto boleto81 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Act - Viajes 82 al 99
            for (int i = 82; i <= 99; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Viaje 100
            Boleto boleto100 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boleto81.TotalAbonado, Is.EqualTo(1580f),
                    "Viaje 81: vuelve a tarifa normal");
                Assert.That(boleto100.TotalAbonado, Is.EqualTo(1580f),
                    "Viaje 100: tarifa normal");
                Assert.That(_tarjeta.ViajesMes, Is.EqualTo(100));
            });
        }

        [Test]
        public void UsoFrecuente_CambioMes_ReiniciaContador()
        {
            // Arrange - Hacer 50 viajes en noviembre (con descuento)
            for (int i = 1; i <= 50; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Avanzar a diciembre
            _tiempo.AgregarDias(30);

            // Act - Primer viaje de diciembre
            Boleto boletoNuevoMes = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boletoNuevoMes.TotalAbonado, Is.EqualTo(1580f),
                    "Nuevo mes: vuelve a tarifa normal");
                Assert.That(_tarjeta.ViajesMes, Is.EqualTo(1),
                    "Contador reinicia en nuevo mes");
            });
        }

        [Test]
        public void UsoFrecuente_NoAplicaAMedioBoleto()
        {
            // Arrange
            var medioBoleto = new MedioBoletoEstudiantil(100000f, 2);

            // Hacer viajes en DÍAS DIFERENTES para mantener el descuento de medio boleto
            // Día 1: 2 viajes con descuento
            _tiempo = new TiempoFalso(2024, 11, 1);
            _colectivo.PagarCon(medioBoleto, _tiempo);
            _tiempo.AgregarMinutos(10);
            _colectivo.PagarCon(medioBoleto, _tiempo);

            // Día 2: 2 viajes con descuento
            _tiempo.AgregarDias(1);
            _colectivo.PagarCon(medioBoleto, _tiempo);
            _tiempo.AgregarMinutos(10);
            _colectivo.PagarCon(medioBoleto, _tiempo);

            // Día 3: 2 viajes con descuento
            _tiempo.AgregarDias(1);
            _colectivo.PagarCon(medioBoleto, _tiempo);
            _tiempo.AgregarMinutos(10);
            Boleto boleto6 = _colectivo.PagarCon(medioBoleto, _tiempo);

            // Assert - Medio boleto siempre paga 790 en sus primeros 2 viajes del día
            // NO aplica descuento por uso frecuente
            Assert.That(boleto6.TotalAbonado, Is.EqualTo(790f),
                "Medio boleto no aplica descuento por uso frecuente, siempre $790 en sus 2 primeros viajes del día");
        }

        [Test]
        public void UsoFrecuente_NoAplicaABoletoGratuito()
        {
            // Arrange
            var boletoGratuito = new BoletoGratuitoEstudiantil(100000f, 3);

            // Hacer viajes en DÍAS DIFERENTES para mantener viajes gratuitos
            // Día 1: 2 viajes gratuitos
            _tiempo = new TiempoFalso(2024, 11, 1);
            _colectivo.PagarCon(boletoGratuito, _tiempo);
            _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Día 2: 2 viajes gratuitos
            _tiempo.AgregarDias(1);
            _colectivo.PagarCon(boletoGratuito, _tiempo);
            _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Día 3: 2 viajes gratuitos
            _tiempo.AgregarDias(1);
            _colectivo.PagarCon(boletoGratuito, _tiempo);
            Boleto boleto6 = _colectivo.PagarCon(boletoGratuito, _tiempo);

            // Assert - Boleto gratuito siempre gratis en sus primeros 2 viajes del día
            // NO aplica descuento por uso frecuente
            Assert.That(boleto6.TotalAbonado, Is.EqualTo(0f),
                "Boleto gratuito no aplica descuento por uso frecuente, siempre $0 en sus 2 primeros viajes del día");
        }

        [Test]
        public void UsoFrecuente_NoAplicaAFranquiciaCompleta()
        {
            // Arrange
            var franquicia = new FranquiciaCompleta(100000f, 4);

            // Hacer 35 viajes
            for (int i = 1; i <= 35; i++)
            {
                _colectivo.PagarCon(franquicia, _tiempo);
            }

            // Act - Viaje 36
            Boleto boleto36 = _colectivo.PagarCon(franquicia, _tiempo);

            // Assert
            Assert.That(boleto36.TotalAbonado, Is.EqualTo(0f),
                "Franquicia completa no aplica descuento por uso frecuente, siempre $0");
        }

        [Test]
        public void UsoFrecuente_TransicionEntreTramos()
        {
            // Act - Viajes 1 al 28
            for (int i = 1; i <= 28; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Viaje 29 (sin descuento)
            Boleto viaje29 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Act - Viaje 30 (empieza 20% descuento)
            Boleto viaje30 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Act - Viajes 31 al 58
            for (int i = 31; i <= 58; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Viaje 59 (último con 20%)
            Boleto viaje59 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Act - Viaje 60 (empieza 25% descuento)
            Boleto viaje60 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Act - Viajes 61 al 79
            for (int i = 61; i <= 79; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Viaje 80 (último con 25%)
            Boleto viaje80 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Act - Viaje 81 (vuelve a normal)
            Boleto viaje81 = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(viaje29.TotalAbonado, Is.EqualTo(1580f), "Viaje 29: normal");
                Assert.That(viaje30.TotalAbonado, Is.EqualTo(1264f), "Viaje 30: 20% desc");
                Assert.That(viaje59.TotalAbonado, Is.EqualTo(1264f), "Viaje 59: 20% desc");
                Assert.That(viaje60.TotalAbonado, Is.EqualTo(1185f), "Viaje 60: 25% desc");
                Assert.That(viaje80.TotalAbonado, Is.EqualTo(1185f), "Viaje 80: 25% desc");
                Assert.That(viaje81.TotalAbonado, Is.EqualTo(1580f), "Viaje 81: normal");
            });
        }

        [Test]
        public void UsoFrecuente_CalculoTotalGastado_DebeSumarCorrectamente()
        {
            // Arrange
            float totalEsperado = 0f;

            // 29 viajes sin descuento
            totalEsperado += 29 * 1580f;

            // 30 viajes con 20% descuento
            totalEsperado += 30 * (1580f * 0.80f);

            // 21 viajes con 25% descuento
            totalEsperado += 21 * (1580f * 0.75f);

            // 20 viajes más sin descuento
            totalEsperado += 20 * 1580f;

            float saldoInicial = _tarjeta.Saldo;

            // Act - Realizar 100 viajes
            for (int i = 1; i <= 100; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            float totalGastado = saldoInicial - _tarjeta.Saldo;

            // Assert
            Assert.That(totalGastado, Is.EqualTo(totalEsperado).Within(0.01f),
                "El total gastado debe coincidir con la suma de todos los montos");
        }

        [Test]
        public void UsoFrecuente_ContadorEnLimites_FuncionaCorrectamente()
        {
            // Test viaje 29 (límite superior sin descuento)
            for (int i = 1; i <= 29; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }
            Assert.That(_tarjeta.ViajesMes, Is.EqualTo(29));

            // Test viaje 30 (inicio descuento 20%)
            Boleto b30 = _colectivo.PagarCon(_tarjeta, _tiempo);
            Assert.That(b30.TotalAbonado, Is.EqualTo(1264f));
            Assert.That(_tarjeta.ViajesMes, Is.EqualTo(30));

            // Test viaje 59 (fin descuento 20%)
            for (int i = 31; i <= 59; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }
            Assert.That(_tarjeta.ViajesMes, Is.EqualTo(59));

            // Test viaje 60 (inicio descuento 25%)
            Boleto b60 = _colectivo.PagarCon(_tarjeta, _tiempo);
            Assert.That(b60.TotalAbonado, Is.EqualTo(1185f));
            Assert.That(_tarjeta.ViajesMes, Is.EqualTo(60));

            // Test viaje 80 (fin descuento 25%)
            for (int i = 61; i <= 80; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }
            Assert.That(_tarjeta.ViajesMes, Is.EqualTo(80));

            // Test viaje 81 (vuelve a normal)
            Boleto b81 = _colectivo.PagarCon(_tarjeta, _tiempo);
            Assert.That(b81.TotalAbonado, Is.EqualTo(1580f));
            Assert.That(_tarjeta.ViajesMes, Is.EqualTo(81));
        }

        [Test]
        public void UsoFrecuente_CambioAño_ReiniciaContador()
        {
            // Arrange - Hacer 50 viajes en diciembre 2024
            _tiempo = new TiempoFalso(2024, 12, 15);
            for (int i = 1; i <= 50; i++)
            {
                _colectivo.PagarCon(_tarjeta, _tiempo);
            }

            // Act - Avanzar a enero 2025
            _tiempo = new TiempoFalso(2025, 1, 5);
            Boleto boletoNuevoAño = _colectivo.PagarCon(_tarjeta, _tiempo);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(boletoNuevoAño.TotalAbonado, Is.EqualTo(1580f),
                    "Nuevo año: vuelve a tarifa normal");
                Assert.That(_tarjeta.ViajesMes, Is.EqualTo(1),
                    "Contador reinicia en nuevo año");
            });
        }
    }
}