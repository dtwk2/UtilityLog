using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace CustomWindow {

    /// <summary>
    /// Based on
    /// <a href="https://blog.magnusmontin.net/2013/03/16/how-to-create-a-custom-window-in-wpf/"></a>
    /// </summary>
    public class Window : System.Windows.Window {

        private enum Direction {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        private HwndSource hwndSource;

        protected override void OnInitialized(EventArgs e) {
            SourceInitialized += (s, _) => hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            base.OnInitialized(e);
        }

        static Window() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
        }

        public Window() {
            PreviewMouseMove += (s, e) => {
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                    Cursor = Cursors.Arrow;
            };
        }

        public override void OnApplyTemplate() {
            if (GetTemplateChild("PART_MinimizeButton") is Button partMinimizeButton)
                partMinimizeButton.Click += (s, e) => WindowState = WindowState.Minimized; ;

            if (GetTemplateChild("PART_RestoreButton") is Button partRestoreButton)
                partRestoreButton.Click += (s, _) => WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal; ;

            if (GetTemplateChild("PART_CloseButton") is Button partCloseButton)
                partCloseButton.Click += (s, _) => Close();

            if (GetTemplateChild("PART_ResizeGrid") is ContentControl{ Content: Grid{Children:{} children} } partResizeGrid) {
                foreach (UIElement element in children) {
                    if (element is Rectangle resizeRectangle) {
                        resizeRectangle.PreviewMouseDown += ResizeRectangle_PreviewMouseDown;
                        resizeRectangle.MouseMove += (sender, _) => Cursor = GetCursor(sender as Rectangle);
                    }
                }
            }
            base.OnApplyTemplate();
        }

        protected void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (!(sender is Rectangle rectangle))
                throw new NullReferenceException();

            Cursor = GetCursor(rectangle);
            ResizeWindow(GetResizeDirection(rectangle), hwndSource);
            static void ResizeWindow(Direction direction, HwndSource hwndSource) {
                SendMessage(hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
            }
        }

        private Direction GetResizeDirection(Rectangle rectangle) =>
            rectangle.Name switch {
                "top" => (Direction.Top),
                "bottom" => (Direction.Bottom),
                "left" => (Direction.Left),
                "right" => (Direction.Right),
                "topLeft" => (Direction.TopLeft),
                "topRight" => (Direction.TopRight),
                "bottomLeft" => (Direction.BottomLeft),
                "bottomRight" => (Direction.BottomRight),
                _ => throw new Exception("Unexpected rectangle name, " + rectangle.Name)
            };

        static Cursor GetCursor(Rectangle rectangle) =>
            rectangle?.Name switch {
                "top" => Cursors.SizeNS,
                "bottom" => Cursors.SizeNS,
                "left" => Cursors.SizeWE,
                "right" => Cursors.SizeWE,
                "topLeft" => Cursors.SizeNWSE,
                "topRight" => Cursors.SizeNESW,
                "bottomLeft" => Cursors.SizeNESW,
                "bottomRight" => Cursors.SizeNWSE,
                _ => throw new Exception("Unexpected rectangle name, " + rectangle.Name)
            };
    }
}
