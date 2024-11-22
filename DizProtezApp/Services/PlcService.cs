using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using NModbus;
using NModbus.Device;

namespace DizProtezApp.Services
{
    public class PlcService
    {
        private IModbusMaster _master;
        private readonly Random _random = new Random(); // Rasgele değerler için Random nesnesi

        public PlcService()
        {
            try
            {
                // Modbus TCP bağlantısı için zaman aşımı
                var task = Task.Run(() =>
                {
                    TcpClient tcpClient = new TcpClient();
                    tcpClient.Connect("192.168.1.5", 502);
                    return tcpClient;
                });

                // 3 saniyelik bir zaman aşımı belirliyoruz
                if (task.Wait(TimeSpan.FromSeconds(3)))
                {
                    TcpClient tcpClient = task.Result;
                    _master = new ModbusFactory().CreateMaster(tcpClient);
                    Console.WriteLine("PLC bağlantısı başarılı.");
                }
                else
                {
                    Console.WriteLine("PLC bağlantısı zaman aşımına uğradı.");
                    // Kullanıcıya uygun bir uyarı verebilirsiniz
                }
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"PLC bağlantı hatası: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PLC bağlantı hatası: {ex.Message}");
            }
        }


        public void Start()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        #region
                        //// 0. biti açık yap
                        //_registerValue = SetBit(_registerValue, 0);
                        //WriteRegister(0, _registerValue); // D0 (adres 100) değerini Modbus cihazına yaz
                        //Console.WriteLine($"0. bit açıldı: {Convert.ToString(_registerValue, 2).PadLeft(16, '0')}");

                        //// 0. biti kapat
                        //_registerValue = ClearBit(_registerValue, 0);
                        //WriteRegister(0, _registerValue); // D0 (adres 100) değerini Modbus cihazına yaz
                        //Console.WriteLine($"0. bit kapatıldı: {Convert.ToString(_registerValue, 2).PadLeft(16, '0')}");
                        #endregion



                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Bit değiştirme hatası: {ex.Message}");
                    }
                }
            });
        }

        //public ushort[] ReadData(ushort startAddress, ushort numRegisters)
        //{
        //    try
        //    {

        //        // Modbus register değerlerini oku
        //        return _master.ReadHoldingRegisters(1, startAddress, numRegisters);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Veri okuma hatası: {ex.Message}");
        //        return null;
        //    }
        //}

        //public void WriteData(ushort address, ushort value)
        //{
        //    try
        //    {
        //        // Modbus register adresine veri yaz
        //        _master.WriteSingleRegister(1, address, value); // Slave ID: 1, Address: D0, Value: Örneğin 123
        //        Console.WriteLine($"Adres {address} içine {value} yazıldı.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Veri yazma hatası: {ex.Message}");
        //    }
        //}

        //private ushort[] FloatToRegisters(float value)
        //{
        //    byte[] bytes = BitConverter.GetBytes(value); // Float'i byte array'e dönüştür
        //    ushort[] registers = new ushort[2];
        //    registers[0] = BitConverter.ToUInt16(bytes, 0); // İlk 2 byte (low-order word)
        //    registers[1] = BitConverter.ToUInt16(bytes, 2); // Sonraki 2 byte (high-order word)
        //    return registers;
        //}

        //public void WriteFloatData(ushort startAddress, float value)
        //{
        //    try
        //    {
        //        ushort[] registers = FloatToRegisters(value); // Float değeri 2 register'a dönüştür
        //        _master.WriteMultipleRegisters(1, startAddress, registers); // Slave ID: 1
        //        Console.WriteLine($"Adres {startAddress} ve {startAddress + 1} içine {value} yazıldı.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Float veri yazma hatası: {ex.Message}");
        //    }
        //}

        //private ushort _registerValue = 0; // D0 register'inin başlangıç değeri



        //// Belirtilen bitin değerini 1 yapar
        //public ushort SetBit(ushort registerValue, int bitIndex)
        //{
        //    return (ushort)(registerValue | (1 << bitIndex));
        //}

        //// Belirtilen bitin değerini 0 yapar
        //public ushort ClearBit(ushort registerValue, int bitIndex)
        //{
        //    return (ushort)(registerValue & ~(1 << bitIndex));
        //}

        //// Modbus register'ına veri yazma
        //private void WriteRegister(ushort address, ushort value)
        //{
        //    try
        //    {
        //        // Modbus yazma işlemi
        //        // Örnek: Modbus master nesnesini burada kullanarak değeri yaz
        //        _master.WriteSingleRegister(1, address, value); // Slave ID: 1
        //        Console.WriteLine($"Adres {address} içerisine {value} yazıldı.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Modbus yazma hatası: {ex.Message}");
        //    }
        //}




    }
}
