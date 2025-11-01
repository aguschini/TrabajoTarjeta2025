using NUnit.Framework;
using ManejoDeTiempos;

namespace ManejoDeTiemposTest
{
    public class Tests
    {
        public TiempoFalso tiempo;
        public Carrito carrito;
        public Producto producto;
        public Producto producto2;

        [SetUp]
        public void Setup()
        {
            tiempo = new TiempoFalso();
            carrito = new Carrito();
            producto = new Producto("Chocolate", 10);
            producto2 = new Producto("Yerba", 5);
        }

        [Test]
        public void SinDescuento()
        {
            carrito.Agregar(producto, 10);
            double total = carrito.Cobrar(tiempo);
            double subtotal = carrito.GetSubTotal();
            Assert.That(total, Is.EqualTo(100));
            Assert.That(total, Is.EqualTo(subtotal));
        }
        [Test]
        public void ConDescuento()
        {
            carrito.Agregar(producto, 10);
            tiempo.AgregarDias(5);
            double total = carrito.Cobrar(tiempo);
            double subtotal = carrito.GetSubTotal();
            Assert.That(total, Is.EqualTo(85));
            Assert.That(total, Is.LessThan(subtotal));
        }
        [Test]
        public void DifTiempo()
        {
            carrito.Agregar(producto, 10);
            double total = carrito.Cobrar(tiempo);
            Ticket ticket1 = new Ticket(carrito.productos, tiempo, total);

            tiempo.AgregarMinutos(5);
            carrito = new Carrito();
            carrito.Agregar(producto2, 2);
            double total2 = carrito.Cobrar(tiempo);
            Ticket ticket2 = new Ticket(carrito.productos, tiempo, total);

            Assert.That((ticket2.fecha - ticket1.fecha).Minutes, Is.EqualTo(5));
        }
    }
}