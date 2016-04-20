using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestInterface.TemperatureControl
{
    class TempControl
    {
        public string DTReading {get;set;}
        public double Temperature { get; set;}

        //public List<TempControl> CreateTempC()
        //{
        //    List<TempControl> TempDT = new List<TempControl>();
        //    Random rnd = new Random();
        //    for (double i = 1; i <= 31; i++)
        //    {
        //        TempDT.Add(new TempControl() { DTReading = i, Temperature = Math.Round((rnd.NextDouble() * 100)-40, 2)});
        //    }
        //    return TempDT;
        //}
    }
}
