using System;
using System.Threading.Tasks;

namespace Jumble.Extensions.RaidProtection
{
    public class Utils
    {
        public static async Task SetTimeout(Delegate action, int delay, params object[] args)
        {
            Task.Run(() =>
            {
                Task.Delay(delay).ContinueWith(new Action<Task>((_) => action.DynamicInvoke(args)));
            });
        }
    }
}