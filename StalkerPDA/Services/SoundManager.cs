using Android.Content;
using Android.Media;
using Android.Content.Res;

namespace StalkerPDA.Services
{
    public static class SoundManager
    {
        private static MediaPlayer _player;

        public static void PlayClick(Context context)
        {
            // Указываем путь относительно папки Assets
            PlaySoundFromAssets(context, "Sounds/pda_click.mp3");
        }

        public static void PlayNotification(Context context)
        {
            // Указываем путь относительно папки Assets
            PlaySoundFromAssets(context, "Sounds/pda_notification.mp3");
        }

        private static void PlaySoundFromAssets(Context context, string fileName)
        {
            try
            {
                // Останавливаем предыдущий звук
                if (_player != null)
                {
                    if (_player.IsPlaying) _player.Stop();
                    _player.Release();
                }

                _player = new MediaPlayer();

                // Открываем файл из папки Assets
                AssetFileDescriptor afd = context.Assets.OpenFd(fileName);

                // Передаем данные в плеер
                _player.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
                _player.Prepare();
                _player.Start();
            }
            catch
            {
                // Игнорируем ошибку, если файл не найден или поврежден
            }
        }
    }
}