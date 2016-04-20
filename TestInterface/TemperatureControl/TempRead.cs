using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace TestInterface.TemperatureControl
{
    public class HTS221 : IDisposable
    {
        // I2C Address
        public const byte C_Addr = 0x5F;
        public const byte C_RegId = 0x0F;
        public const byte C_Id = 0xBC;

        // Registers
        public const byte C_WhoAmI = 0x0F;
        public const byte C_AvConf = 0x10;
        public const byte C_Ctrl1 = 0x20;
        public const byte C_Ctrl2 = 0x21;
        public const byte C_Ctrl3 = 0x22;
        public const byte C_Status = 0x27;
        public const byte C_HumidityOutL = 0x28;
        public const byte C_HumidityOutH = 0x29;
        public const byte C_TempOutL = 0x2A;
        public const byte C_TempOutH = 0x2B;
        public const byte C_H0H2 = 0x30;
        public const byte C_H1H2 = 0x31;
        public const byte C_T0C8 = 0x32;
        public const byte C_T1C8 = 0x33;
        public const byte C_T1T0 = 0x35;
        public const byte C_H0T0Out = 0x36;
        public const byte C_H1T0Out = 0x3A;
        public const byte C_T0Out = 0x3C;
        public const byte C_T1Out = 0x3E;

        private I2cDevice device;
        private Func<Int16, float> convertReading;

        private byte Read8(byte addr, string msg)
        {
            try
            {
                byte[] wb = { addr };
                var rb = new byte[1];
                device.WriteRead(wb, rb);
                return rb[0];
            }
            catch (Exception e)
            {
                throw new Exception(msg, e);
            }
        }

        private UInt16 Read16LE(byte addr, string msg)
        {
            try
            {
                byte[] wb = { addr };
                byte[] rb = new byte[2];
                device.WriteRead(wb, rb);
                return (UInt16)(((UInt16)rb[1] << 8) | (UInt16)rb[0]);
            }
            catch (Exception e)
            {
                throw new Exception(msg, e);
            }
        }

        private async Task GetDevice()
        {
            var aqs = I2cDevice.GetDeviceSelector();
            var infos = await DeviceInformation.FindAllAsync(aqs);
            var settings = new I2cConnectionSettings(C_Addr)
            {
                BusSpeed = I2cBusSpeed.FastMode
            };
            device = await I2cDevice.FromIdAsync(infos[0].Id, settings);
        }

        private Func<Int16, float> GetConverter()
        {
            byte rawMsb = Read8(C_T1T0 + 0x80, "failed to read T1T0");

            byte t0Lsb = Read8(C_T0C8 + 0x80, "failed to read T0C8");
            UInt16 t0c8 = (UInt16)(((UInt16)(rawMsb & 0x03) << 8) | (UInt16)t0Lsb);
            float t0 = t0c8 / 8.0f;

            byte t1Lsb = Read8(C_T1C8 + 0x80, "failed to read T1C8");
            UInt16 t1c8 = (UInt16)(((UInt16)(rawMsb & 0x0C) << 6) | (UInt16)t1Lsb);
            float t1 = t1c8 / 8.0f;

            Int16 t0out = (Int16)Read16LE(C_T0Out + 0x80, "failed to read T0Out");
            Int16 t1out = (Int16)Read16LE(C_T1Out + 0x80, "failed to read T1Out");

            // calibration slope
            float m = (t1 - t0) / (t1out - t0out);
            // y intercept
            float k = t0 - (m * t0out);

            return t => t * m + k;
        }

        public float ReadTemperature()
        {
            var status = Read8(C_Status, "failed to read Status");
            if ((status & 1) == 1)
            {
                var rawData = (Int16)Read16LE(C_TempOutL + 0x80, "failed to read TempOutL");
                var t = convertReading(rawData);
                return t;
            }
            return 0.0f;
        }

        public void Init()
        {
            Task.Run(async () =>
            {
                await GetDevice().ConfigureAwait(false);
            }).Wait(5000);
            if (device == null)
            {
                throw new Exception("failed to get device");
            }

            byte[] data = new byte[2];
            data[0] = C_Ctrl1;
            data[1] = 0x87;
            device.Write(data);

            data[0] = C_AvConf;
            data[1] = 0x1B;
            device.Write(data);

            convertReading = GetConverter();
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                device.Dispose();
                disposed = true;
            }
        }
    }

}
