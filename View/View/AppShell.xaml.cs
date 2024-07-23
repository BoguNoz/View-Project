using System.Globalization;
using View.Helpers;

namespace View
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if (Expand.IsVisible)
            {
                var animation = new Animation((current) =>
                {
                    FlyoutWidth = current;
                }, 65,250,null);

                animation.Commit(this,"expand", finished: (value,cancelled) =>
                {
                    Expand.IsVisible = false;
                    Minimize.IsVisible = true;
                });
            }
            else
            {
                var animation = new Animation((current) =>
                {
                    FlyoutWidth = current;
                }, 250, 65, null);

                animation.Commit(this, "expand", finished: (value, cancelled) =>
                {
                    Expand.IsVisible = true;
                    Minimize.IsVisible = false;
                });
            }
        }
    }
}
