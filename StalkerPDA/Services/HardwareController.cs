using Android.App;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;

namespace StalkerPDA.Services
{
    public class HardwareController
    {
        private Vibrator _vibrator;

        public HardwareController()
        {
            _vibrator = (Vibrator)Application.Context.GetSystemService(Context.VibratorService);
        }

        public void VibrateShort()
        {
            if (_vibrator != null && _vibrator.HasVibrator)
            {
                _vibrator.Vibrate(VibrationEffect.CreateOneShot(50, VibrationEffect.DefaultAmplitude));
            }
        }

        public async void VibrateSleepWarning()
        {
            if (_vibrator != null && _vibrator.HasVibrator)
            {
                for (int i = 0; i < 3; i++)
                {
                    _vibrator.Vibrate(VibrationEffect.CreateOneShot(400, VibrationEffect.DefaultAmplitude));
                    await Task.Delay(600);
                }
            }
        }
    }
}