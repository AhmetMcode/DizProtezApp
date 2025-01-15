using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using Azure.Core;
using NModbus;
using NModbus.Device;

namespace DizProtezApp.Services
{
    public class PlcService
    {
        private IModbusMaster? _master;
        private bool _isRunning;

        // Sabit IP ve Port bilgisi
        private const string PlcIpAddress = "192.168.1.5";
        private const int PlcPort = 502;

        // PLC bağlantı durumu kontrolü
        public bool IsConnected => _master != null;

        public async Task<bool> Connect()
        {
            try
            {
                TcpClient tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(PlcIpAddress, PlcPort); // Async bağlantı
                _master = new ModbusFactory().CreateMaster(tcpClient);
                Console.WriteLine("PLC bağlantısı başarılı.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PLC bağlantı hatası: {ex.Message}");
                return false;
            }
        }

        public void Start()
        {
            if (!IsConnected)
            {
                Console.WriteLine("PLC bağlantısı mevcut değil, Start işlemi başlatılamıyor.");
                return;
            }

            _isRunning = true;
            Task.Run(async () =>
            {
                while (_isRunning)
                {
                    try
                    {

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"PLC döngü hatası: {ex.Message}");
                    }
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;
            CloseConnection();
            Console.WriteLine("PLC işlemleri durduruldu.");
        }

        public void CloseConnection()
        {
            if (!IsConnected)
            {
                Console.WriteLine("PLC zaten bağlantısız.");
                return;
            }

            _master?.Dispose();
            _master = null; // Bağlantıyı null yaparak bağlantı durumunu sıfırla
            Console.WriteLine("PLC bağlantısı kapatıldı.");
        }

        #region Bool Methods
        public bool ReadBool((ushort RegisterAddress, byte BitIndex) address, byte slaveId = 1)
        {
            try
            {
                // Register'dan mevcut değeri oku
                int currentValue = ReadWord(address.RegisterAddress);

                // Belirtilen BitIndex'i kontrol et
                bool result = (currentValue & (1 << address.BitIndex)) != 0;

                Console.WriteLine($"Adres {address.RegisterAddress}, Bit {address.BitIndex} okundu: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bool okuma hatası: {ex.Message}");
                return false;
            }
        }


        public void WriteBool((ushort RegisterAddress, byte BitIndex) address, bool value, byte slaveId = 1)
        {
            try
            {
                // Register'dan mevcut değeri oku
                int currentValue = ReadWord(address.RegisterAddress);

                // Belirtilen BitIndex'i değiştir
                if (value)
                    currentValue |= (1 << address.BitIndex); // Bit'i 1 yap
                else
                    currentValue &= ~(1 << address.BitIndex); // Bit'i 0 yap

                // Güncellenmiş değeri register'a yaz
                WriteWord(address.RegisterAddress, currentValue);

                Console.WriteLine($"Adres {address.RegisterAddress}, Bit {address.BitIndex} yazıldı: {value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bool yazma hatası: {ex.Message}");
            }
        }


        #endregion

        #region Word Methods
        public int ReadWord(ushort registerAddress)
        {
            try
            {
                // _master null ise işlem yapılmaz ve varsayılan değer döner
                if (_master == null)
                {
                    Console.WriteLine("Modbus master bağlantısı mevcut değil.");
                    return 0; // Varsayılan değer
                }

                ushort[] registers = _master.ReadHoldingRegisters(1, registerAddress, 1);

                // ushort'u int'e dönüştür ve geri döndür
                return registers[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WORD okuma hatası: {ex.Message}");
                return 0;
            }
        }



        public void WriteWord(ushort registerAddress, int value)
        {
            try
            {
                // _master null ise işlem yapılmaz
                if (_master == null)
                {
                    Console.WriteLine("Modbus master bağlantısı mevcut değil.");
                    return;
                }

                // Değerin ushort aralığında olduğundan emin olun
                if (value < ushort.MinValue || value > ushort.MaxValue)
                {
                    Console.WriteLine($"WORD yazma hatası: Değer ushort aralığında değil (Değer: {value}).");
                    return;
                }

                // int değeri ushort'a dönüştürerek yaz
                _master.WriteSingleRegister(1, registerAddress, (ushort)value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WORD yazma hatası: {ex.Message}");
            }
        }


        #endregion

        #region Real (Float) Methods
        public int ReadDWord(ushort registerAddress)
        {
            try
            {
                if (_master == null)
                {
                    Console.WriteLine("Modbus master bağlantısı mevcut değil.");
                    return 0;
                }

                // İki 16-bit register oku
                ushort[] registers = _master.ReadHoldingRegisters(1, registerAddress, 2);
                if (registers.Length < 2)
                {
                    Console.WriteLine("DWORD okuma hatası: Beklenen register sayısı alınamadı.");
                    return 0;
                }

                // Low Word ve High Word'den 32-bit signed int oluştur
                int result = (registers[1] << 16) | registers[0];

                Console.WriteLine($"DWORD okundu: {result} (Register Address: {registerAddress})");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DWORD okuma hatası: {ex.Message}");
                return 0;
            }
        }





        public void WriteDWord(ushort registerAddress, int value)
        {
            try
            {
                if (_master == null)
                {
                    Console.WriteLine("Modbus master bağlantısı mevcut değil. DWORD yazma işlemi başarısız.");
                    return;
                }

                // Int değeri 32-bit olarak byte dizisine dönüştür
                byte[] intBytes = BitConverter.GetBytes(value);


                // Byte dizisinden 16-bit register'ları oluştur
                ushort lowWord = BitConverter.ToUInt16(intBytes, 0);  // İlk iki byte (Low Word)
                ushort highWord = BitConverter.ToUInt16(intBytes, 2); // Son iki byte (High Word)

                // Modbus ile iki 16-bit register'a yaz
                _master.WriteMultipleRegisters(1, registerAddress, new ushort[] { lowWord, highWord });

                Console.WriteLine($"DWORD değeri yazıldı: {value} (Register Address: {registerAddress})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DWORD yazma hatası (Register Address: {registerAddress}, Value: {value}): {ex.Message}");
            }
        }







        #endregion
        public float ReadReal(ushort startAddress)
        {
            try
            {
                if (_master == null)
                {
                    Console.WriteLine("Modbus master bağlantısı mevcut değil.");
                    return 0f;
                }

                // 2 adet 16-bit register oku
                ushort[] registers = _master.ReadHoldingRegisters(1, startAddress, 2);

                // Register'ları byte dizisine dönüştür
                byte[] bytes = new byte[4];

                // Eğer PLC Big Endian bekliyorsa
                bytes[0] = (byte)(registers[1] >> 8); // High Byte of High Word
                bytes[1] = (byte)(registers[1]);      // Low Byte of High Word
                bytes[2] = (byte)(registers[0] >> 8); // High Byte of Low Word
                bytes[3] = (byte)(registers[0]);      // Low Byte of Low Word

                // Byte dizisini float değere dönüştür
                float result = BitConverter.ToSingle(bytes, 0);

                Console.WriteLine($"Okunan REAL değer: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"REAL okuma hatası: {ex.Message}");
                return 0f; // Hata durumunda varsayılan değer
            }
        }


        public void WriteReal(ushort registerAddress, float value)
        {
            try
            {
                if (_master == null)
                {
                    Console.WriteLine("Modbus master bağlantısı mevcut değil. REAL yazma işlemi başarısız.");
                    return;
                }

                // Float değeri IEEE 754 formatında 32-bit integer olarak al
                uint floatAsInt = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);

                // 32-bit integerı byte'lara böl
                byte[] bytes = new byte[4];
                bytes[0] = (byte)(floatAsInt >> 24); // En yüksek byte
                bytes[1] = (byte)(floatAsInt >> 16); // İkinci byte
                bytes[2] = (byte)(floatAsInt >> 8);  // Üçüncü byte
                bytes[3] = (byte)(floatAsInt);       // En düşük byte

                // Eğer endian sırasını ters çevirmek gerekiyorsa (Little Endian -> Big Endian)
                Array.Reverse(bytes);

                // Byte'ları 16-bit register'lara dönüştür
                ushort lowWord = (ushort)((bytes[0] << 8) | bytes[1]); // İlk iki byte (Low Word)
                ushort highWord = (ushort)((bytes[2] << 8) | bytes[3]); // Son iki byte (High Word)

                // Modbus ile iki 16-bit register'a yaz
                _master.WriteMultipleRegisters(1, registerAddress, new ushort[] { highWord, lowWord });

                Console.WriteLine($"REAL yazıldı: {value} (Register Address: {registerAddress})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"REAL yazma hatası (Register Address: {registerAddress}, Value: {value}): {ex.Message}");
            }
        }





    }
}
