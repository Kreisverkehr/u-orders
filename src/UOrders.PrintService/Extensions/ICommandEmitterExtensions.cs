using ESCPOS_NET.Emitters;
using System.Text;

namespace UOrders.PrintService.Extensions
{
    public static class ICommandEmitterExtensions
    {
        public static byte[] PrintLine(this ICommandEmitter emitter, string data, Encoding encoding) => 
            Print(emitter, data + "\n", encoding);

        public static byte[] Print(this ICommandEmitter emitter, string data, Encoding encoding) =>
            encoding.GetBytes(data.Replace("\r\n", "\n").Replace("\r", "\n"));
    }
}
