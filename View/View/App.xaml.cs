using Microsoft.Maui.Platform;
using Microsoft.Maui.Handlers;
#if WINDOWS
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace View
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();



            //App window size and position configuration
            WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
#if WINDOWS
            var windowWidth = 1350;
            var windowHeight = 900;
            var nativeWindow = handler.PlatformView;
            var appWindow = nativeWindow.GetAppWindow();

            if (appWindow is not null)
            {
                appWindow.Resize(new SizeInt32(windowWidth, windowHeight));

                var windowId = appWindow.Id;
                DisplayArea displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);
                if (displayArea is not null)
                {
                    PointInt32 CenteredPosition = new()
                    {
                        X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2),
                        Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2)
                    };
                    appWindow.Move(CenteredPosition);
                }
            }
            nativeWindow.Activate();
#endif
            });
        }


        
    }
}
