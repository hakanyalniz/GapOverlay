using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace GapOverlay;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{   
    // The mouse coordinates are stored as below structure
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    // Calling the below function in user32.dll will give us mouse coordinates
    // then assign them into POINT
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);
    private DispatcherTimer? mouseTimer;

    public MainWindow()
    {
        InitializeComponent();
        // After the above is initialized and window is shown, run below
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        mouseTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        mouseTimer.Tick += MouseTimer_Tick;
        mouseTimer.Start();
    }

    private void MouseTimer_Tick(object? sender, EventArgs e)
    {
        GetCursorPos(out POINT p);

        double centerX = ActualWidth / 2;
        double delta = (p.X - centerX) / ActualWidth;

        double gapCenter = 0.5 + delta * 0.1;

        DoorMask.GradientStops[1].Offset = gapCenter - 0.05;
        DoorMask.GradientStops[2].Offset = gapCenter - 0.02;
        DoorMask.GradientStops[3].Offset = gapCenter + 0.02;
        DoorMask.GradientStops[4].Offset = gapCenter + 0.05;
    }

}