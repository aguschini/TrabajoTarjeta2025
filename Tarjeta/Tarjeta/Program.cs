using System;

namespace Tarjeta
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iteración 1 - Simulación transporte urbano (ejemplo rápido)");

            var tarjeta = new Tarjeta(0, 12345);
            tarjeta.Cargar(2000);

            var colectivo = new Colectivo("101");
            var boleto = colectivo.pagarCon(tarjeta);

            if (boleto != null)
            {
                Console.WriteLine($"Boleto emitido. Precio: {boleto.Precio}. Saldo resto: {tarjeta.Saldo}");
            }
            else
            {
                Console.WriteLine("No se pudo pagar el viaje.");
            }
        }
    }
}
