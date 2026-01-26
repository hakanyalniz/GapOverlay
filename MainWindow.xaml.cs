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

        // The second integer value can be changed to change the movement range
        // of the gap 
        double gapCenter = 0.5 + delta * 0.1;

        // Change the value of the integers here and at MainWindow.xaml to change the gap width
        // The halfGap must be half of the normal gap width, since it is divided in half
        // where left and right both in TOTAL make the gap width
        // when we subtract in one side and add in another, they in total make the total gap width

        // The size of the width must be written here, for example 0.49 - 0.51 = 0.02 / 2
        double halfGap = 0.01;
        double fade = 0.01;
        DoorMask.GradientStops[1].Offset = gapCenter - halfGap - fade; // 0.48
        DoorMask.GradientStops[2].Offset = gapCenter - halfGap;        // 0.49
        DoorMask.GradientStops[3].Offset = gapCenter + halfGap;        // 0.51
        DoorMask.GradientStops[4].Offset = gapCenter + halfGap + fade; // 0.52
    }

}