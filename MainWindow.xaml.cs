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
     // The size of the width must be written here, for example 0.49 - 0.51 = 0.02 / 2
    private double halfGap = 0.01;
    private double fade = 0.01;

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
        // Subscribe to key down event
        this.KeyDown += MainWindow_KeyDown;

        #if !DEBUG
            DebugTextBox.Visibility = Visibility.Collapsed; // hide in Release
        #endif
    }

    // The timer loop that calls MouseTimer tick on interval
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        mouseTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        mouseTimer.Tick += MouseTimer_Tick;
        mouseTimer.Start();
    }

    // Each interval, it decides new position based on mouse coordinates
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

        // private double halfGap = 0.01;
        // private double fade = 0.01;
        DoorMask.GradientStops[1].Offset = gapCenter - halfGap - fade; // 0.48
        DoorMask.GradientStops[2].Offset = gapCenter - halfGap;        // 0.49
        DoorMask.GradientStops[3].Offset = gapCenter + halfGap;        // 0.51
        DoorMask.GradientStops[4].Offset = gapCenter + halfGap + fade; // 0.52
    }

    // When key is pressed, run this
    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {

        #if DEBUG
            LogDebug($"halfGap = {halfGap}, fade = {fade}");
        #endif
        if (e.Key == Key.Left)
        {
            if (halfGap >= 0.45) // bugs out if halfGap is any higher
            {
                halfGap = 0.45;
            }

            // Ensures that halfGap is 0, while fade is 0.01, 
            // then next click halfGap is 0.01
            // This allows it to start with tiny gap, since tiny gap is halfGap 0, fade 0.01
            halfGap += fade == 0 ? 0 : 0.01;
            fade =  0.01;
        }
        else if (e.Key == Key.Right)
        {
            // Allows for tiny gap while decreasing
            if (halfGap == 0 && fade == 0.01)
            {
                fade = 0;
            }

            halfGap -= 0.01;
            // If halfGap is negative value, then equal 0, or else leave it be
            halfGap = halfGap <= 0 ? 0 : halfGap;
        }

        if (e.Key == Key.Escape)
        {
            Close(); 
        }
        #if DEBUG
            LogDebug($"AFTER // halfGap = {halfGap}, fade = {fade}");
        #endif
  
    }

    private void LogDebug(string message)
    {
        DebugTextBox.AppendText($"{DateTime.Now:HH:mm:ss}: {message}\n");
        DebugTextBox.ScrollToEnd(); // always scroll to show the latest message
    }

}