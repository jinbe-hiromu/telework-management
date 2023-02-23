namespace WorkScheduler
{
    internal static partial class PopupSizeGetter
    {
        public static partial Size Get()
        {
            var bounds = UIKit.UIScreen.MainScreen.Bounds;
            var width = bounds.Width;
            var height = bounds.Height;
            return new Size(width-10, height-10);
        }
    }
}
