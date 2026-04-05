using Android.App;
using Android.Content;
using Android.OS;

namespace StalkerPDA.Services
{
    public class BatteryMonitor
    {
        public int GetBatteryLevel()
        {
            var filter = new IntentFilter(Intent.ActionBatteryChanged);
            var batteryStatus = Application.Context.RegisterReceiver(null, filter);

            int level = batteryStatus?.GetIntExtra(BatteryManager.ExtraLevel, -1) ?? -1;
            int scale = batteryStatus?.GetIntExtra(BatteryManager.ExtraScale, -1) ?? -1;

            if (level == -1 || scale == -1) return 100;

            return (int)((level / (float)scale) * 100);
        }
    }
}