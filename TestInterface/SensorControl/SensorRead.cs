﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TestInterface.SensorControl

{
    public static class Device
    {
        public static async Task<I2cDevice> GetAsync(int slaveAddr, I2cBusSpeed speed = I2cBusSpeed.StandardMode, I2cSharingMode sharing = I2cSharingMode.Exclusive)
        {
            var aqs = I2cDevice.GetDeviceSelector();
            var infos = await DeviceInformation.FindAllAsync(aqs);
            var settings = new I2cConnectionSettings(slaveAddr)
            {
                BusSpeed = speed,
                SharingMode = sharing
            };
            return await I2cDevice.FromIdAsync(infos[0].Id, settings);
        }

        public static byte Read8(I2cDevice device, byte addr, string errorMsg = "")
        {
            try
            {
                var wb = new byte[] { addr };
                var rb = new byte[1];
                device.WriteRead(wb, rb);
                return rb[0];
            }
            catch (Exception e)
            {
                throw new Exception(errorMsg, e);
            }
        }

        public static UInt16 Read16LE(I2cDevice device, byte addr, string msg)
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

        public static UInt32 Read24LE(I2cDevice device, byte addr, string errorMsg = "")
        {
            try
            {
                var wb = new byte[] { addr };
                var rb = new byte[3];
                device.WriteRead(wb, rb);
                return ((UInt32)rb[2] << 16) | ((UInt32)rb[1] << 8) | (UInt32)rb[0];
            }
            catch (Exception e)
            {
                throw new Exception(errorMsg, e);
            }
        }

        public static void WriteByte(I2cDevice device, byte addr, byte val, string errorMsg = "")
        {
            try
            {
                var buf = new byte[2] { addr, val };
                device.Write(buf);
            }
            catch (Exception e)
            {
                throw new Exception(errorMsg, e);
            }
        }
    }

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
        private Func<Int16, float> convertTemperature;
        private Func<Int16, float> convertHumidity;

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

        private Func<Int16, float> GetTemperatureConverter()
        {
            byte rawMsb = Read8(C_T1T0 + 0x80, "failed to read T1T0");

            byte t0Lsb = Read8(C_T0C8 + 0x80, "failed to read T0C8");
            byte t1Lsb = Read8(C_T1C8 + 0x80, "failed to read T1C8");

            UInt16 t0c8 = (UInt16)(((UInt16)(rawMsb & 0x03) << 8) | (UInt16)t0Lsb);
            UInt16 t1c8 = (UInt16)(((UInt16)(rawMsb & 0x0C) << 6) | (UInt16)t1Lsb);

            float t0 = t0c8 / 8.0f;
            float t1 = t1c8 / 8.0f;

            Int16 t0out = (Int16)Read16LE(C_T0Out + 0x80, "failed to read T0Out");
            Int16 t1out = (Int16)Read16LE(C_T1Out + 0x80, "failed to read T1Out");

            float m = (t1 - t0) / (t1out - t0out);
            float b = t0 - (m * t0out);

            return t => t * m + b;
        }

        private Func<Int16, float> GetHumidityConverter()
        {
            byte h0h2 = Read8(C_H0H2 + 0x80, "failed to read H0H2");
            byte h1h2 = Read8(C_H1H2 + 0x80, "failed to read H1H2");

            float h0 = h0h2 * 0.5f;
            float h1 = h1h2 * 0.5f;

            Int16 h0t0out = (Int16)Read16LE(C_H0T0Out + 0x80, "failed to read H0T0Out");
            Int16 h1t0out = (Int16)Read16LE(C_H1T0Out + 0x80, "failed to read H1T0Out");

            float m = (h1 - h0) / (h0t0out - h1t0out);
            float b = h0 - (m * h0t0out);

            return t => t * m + b;
        }

        public float ReadTemperature()
        {
            var status = Read8(C_Status, "failed to read Status");
            if ((status & 1) == 1)
            {
                var rawData = (Int16)Read16LE(C_TempOutL + 0x80, "failed to read TempOutL");
                var t = convertTemperature(rawData);
                return t;
            }
            return 0.0f;
        }

        public float ReadHumidity()
        {
            var status = Read8(C_Status, "failed to read Status");
            if ((status & 2) == 2)
            {
                var rawData = (Int16)Read16LE(C_HumidityOutL + 0x80, "failed to read HumidityOutL");
                var t = convertHumidity(rawData);
                return t;
            }
            return 0.0f;
        }

        public void Init()
        {
            Task.Run(async () =>
            {
                await GetDevice().ConfigureAwait(false);
            }).Wait();
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

            convertTemperature = GetTemperatureConverter();
            convertHumidity = GetHumidityConverter();
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
    public class LPS25H : IDisposable
    {
        private const byte C_Addr = 0x5C;

        private const byte C_ResConf = 0x10;
        private const byte C_Ctrl1 = 0x20;
        private const byte C_Ctrl2 = 0x21;
        private const byte C_Status = 0x27;
        private const byte C_PressOutXL = 0x28;
        private const byte C_FifoCtrl = 0x2E;

        private const float pressureFactor = 1.0f / 4096.0f;

        private I2cDevice device;

        public void Init()
        {
            Task.Run(async () =>
            {
                device = await Device.GetAsync(C_Addr, I2cBusSpeed.FastMode);
            }).Wait(5000);
            if (device == null)
            {
                throw new Exception("failed to get LPS25H device");
            }

            // PD ODR2-0 DIFF BDU RESET SIM
            //  1    100    0   1     0   0
            // Active mode, 25Hz, default, non-continous, disable, default
            Device.WriteByte(device, C_Ctrl1, 0xC4, "failed to set C_Ctrl1");

            // AVGP1-0 AVGT1-0
            //      01      01
            // N. internal average: 32, 16 (pressure, temperature)
            Device.WriteByte(device, C_ResConf, 0x05, "failed to set C_ResConf");

            // F_MODE2 F_MODE1 F_MODE0
            //       1       1       0
            // FIFO_MEAN_MODE: running average filtered pressure
            Device.WriteByte(device, C_FifoCtrl, 0xC0, "failed to set C_FifoCtrl");

            // BOOT FIFO_EN WTM_EN FIFO_MEAN I2C_EN SWRESET AUTOZERO ONESHOT
            //    0    1         0         0      0       0        0       0
            // normal, enable, disable, disable, i2c enable, normal, normal, waiting
            Device.WriteByte(device, C_Ctrl2, 0x40, "failed to set C_Ctrl2");
        }

        public float ReadPressure()
        {
            var status = Device.Read8(device, C_Status, "failed to read C_Status");
            if ((status & 2) == 2)
            {
                Int32 rawData = (Int32)Device.Read24LE(device, C_PressOutXL + 0x80, "failed to read C_PressOutXL");
                return rawData * pressureFactor;
            }
            return 0.0f;
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
