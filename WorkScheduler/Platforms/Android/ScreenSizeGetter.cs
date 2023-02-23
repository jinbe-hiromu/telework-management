using Android.Hardware.Lights;

namespace WorkScheduler
{
    internal static partial class PopupSizeGetter
    {
        public static partial Size Get()
        {
            var displayMetrics = Android.App.Application.Context.Resources.DisplayMetrics;
            var width = displayMetrics.WidthPixels / displayMetrics.Density;
            var height = displayMetrics.HeightPixels / displayMetrics.Density;
            return new Size(width-10,height-10);
        }
    }
}
