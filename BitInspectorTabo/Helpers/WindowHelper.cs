using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System;
using Windows.Foundation;
using Windows.Graphics;

namespace BitInspectorTabo.Helpers
{
    internal static class WindowHelper
    {
        private static double GetWindowDpiScale(Window window)
        {
            return window.Content.XamlRoot.RasterizationScale;
        }

        public static void ResizeClinetWithDpiScale(Window window, double width, double height)
        {
            var dpiScale = GetWindowDpiScale(window);
            window.AppWindow.ResizeClient(new SizeInt32(
                (int)(width * dpiScale),
                (int)(height * dpiScale)
            ));
        }

        public static void FitClientToActualSize(Window window, FrameworkElement rootGrid)
        {
            rootGrid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            ResizeClinetWithDpiScale(window, rootGrid.DesiredSize.Width, rootGrid.DesiredSize.Height);
        }

        public static void LimitMinClientSize(Window window, double width, double height)
        {
            if (window.AppWindow.Presenter is OverlappedPresenter presenter) {
                var dpiScale = GetWindowDpiScale(window);
                presenter.PreferredMinimumWidth = (int)(width * dpiScale);
                presenter.PreferredMinimumHeight = (int)(height * dpiScale);
            }
        }
    }

    public static class ByteSizeString
    {
        private static readonly string[] Units =
            { "B", "KiB", "MiB", "GiB" };

        public static string ToBinarySize(this ulong bytes, int decimalPlaces = 2)
        {
            if (bytes == 0) return "0 B";

            int unitIndex = (int)Math.Floor(Math.Log(bytes, 1024));
            double adjustedSize = bytes / Math.Pow(1024, unitIndex);

            return $"{Math.Round(adjustedSize, decimalPlaces)} {Units[unitIndex]}";
        }
    }
}
