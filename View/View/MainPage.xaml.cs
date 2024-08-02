using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using View.AppServices;
using Windows.Devices.AllJoyn;



namespace View
{
    public partial class MainPage : ContentPage
    {
        private TablePage tablePage;


        public ObservableCollection<string> SavedDatabases { get; set; }

        public MainPage()
        {
            InitializeComponent();

            PanelsCollectionView.ItemsSource = UserContext.Shemats;

            SavedDatabases = new ObservableCollection<string>();
            UserDatabasesView.ItemsSource = SavedDatabases;


        }

        private async void PlusButton_Clicked(object sender, EventArgs e)
        {
            // Scale the button up
            await PlusButton.ScaleTo(1.2, 100);
            // Scale the button back down
            await PlusButton.ScaleTo(1.0, 100);

            try
            {

                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select a file"
                });

                if (result != null)
                {
                    string filePath = result.FullPath;
                    string fileName = result.FileName;

                    if (UserContext.Shemats.Any(n => n == fileName))
                        fileName = fileName + "-" + UserContext.Shemats.Count.ToString();

                    var response = await UserContext.User.File_AddDatabaseAsync(filePath, 3, fileName);
                    if (!response.Status)
                        await DisplayAlert("Error", response.Message, "OK");
                    else
                    {
                        UserContext.Shemats.Add(fileName);
                        UserContext.Databases.Add(fileName);

                        PlusButton.BackgroundColor = Color.FromHex("#03DAC6");
                        await Task.Delay(600);
                        PlusButton.BackgroundColor = Color.FromHex("#3700B3");
                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }


        }


        private async void OnPanelTapped(object sender, EventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                var frame = border.Parent as Frame;
                if (frame != null)
                {
                    var dbLabel = frame.FindByName<Label>("DbLabel");
                    var nameLabel = frame.Parent.FindByName<Label>("NameLabel");
                    var saveButton = frame.Parent.FindByName<Button>("SaveButton");
                    var reButton = frame.Parent.FindByName<Button>("ReButton");
                    var delButton = frame.Parent.FindByName<Button>("DelButton");

                    await Task.Delay(250);

                    if (nameLabel != null && saveButton != null && reButton != null && delButton != null)
                    {
                        if (nameLabel.IsVisible)
                        {
                            nameLabel.IsVisible = false;

                            saveButton.IsVisible = true;
                            saveButton.IsEnabled = true;

                            reButton.IsVisible = true;
                            reButton.IsEnabled = true;

                            delButton.IsVisible = true;
                            delButton.IsEnabled = true;
                        }
                        else
                        {
                            nameLabel.IsVisible = true;

                            saveButton.IsVisible = false;
                            saveButton.IsEnabled = false;

                            reButton.IsVisible = false;
                            reButton.IsEnabled = false;

                            delButton.IsVisible = false;
                            delButton.IsEnabled = false;
                        }
                    }
                }
            }
        }

        private void OnPointerEntered(object sender, PointerEventArgs e)
        {
            VisualStateManager.GoToState((VisualElement)sender, "MouseOver");
        }

        private void OnPointerExited(object sender, PointerEventArgs e)
        {
            VisualStateManager.GoToState((VisualElement)sender, "Normal");
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {

            var button = sender as Button;
            if (button != null)
            {

                // Animacja podskakiwania
                await button.TranslateTo(0, -10, 100);  // przesuń w górę
                await button.TranslateTo(0, 0, 100);    // przesuń z powrotem w dół

                var context = button.BindingContext as string;
                if (context != null)
                {
                    var response = await UserContext.User.API_AddDatabaseSchemaAsync(context);
                    if (!response.Status)
                        await DisplayAlert("Error", response.Message, "OK");

                    SavedDatabases.Add(context);

                }
            }
        }

        private async void DelButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var context = button.BindingContext as string;
                if (context != null)
                {
                    var response = await UserContext.User.File_DeleteDatabaseAsync(context);
                    if (!response.Status)
                        await DisplayAlert("Error", response.Message, "OK");

                    UserContext.Shemats.Remove(context);
                    if (UserContext.Databases.Any(n => n == context))
                        UserContext.Databases.Remove(context);


                }
            }
        }

        private async void ReButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var context = button.BindingContext as string;
                if (context != null)
                {

                    var result = await FilePicker.PickAsync(new PickOptions
                    {
                        PickerTitle = "Please select a file"
                    });

                    if (result != null)
                    {
                        string filePath = result.FullPath;
                        string fileName = result.FileName;


                        var response = await UserContext.User.File_UpdateDatabaseAsync(filePath, 3, context);
                        if (!response.Status)
                            await DisplayAlert("Error", response.Message, "OK");

                        // Animacja obrotu
                        await button.RotateTo(360, 500);  // obrót o 360 stopni w ciągu 500 ms
                        button.Rotation = 0;  // resetuje obrót do 0 stopni
                    }

                }
            }
        }

        private async void Sigup_label_Tapped(object sender, TappedEventArgs e)
        {
            var label = sender as Label;
            if (label != null)
            {
                await Task.Delay(250);

                var border = label.Parent as Grid;
                if (border != null)
                {
                    var infoLabel = border.FindByName<Label>("Info_Label");
                    var loginButton = border.FindByName<Button>("Login_Button");
                    var passBox = border.FindByName<Entry>("Passwor_box");
                    var rePassBox = border.FindByName<Entry>("RePasswor_box");
                    var mailBox = border.FindByName<Entry>("Mail_box");
                    var welcLabel = border.FindByName<Label>("Welcom_label");
                    var logLabel = border.FindByName<Label>("Login_label");

                    if (label.Text == "Sing up")
                    {
                        label.Text = "Sing in";
                        infoLabel.Text = "Already have an account?";
                        loginButton.Text = "Register";
                        rePassBox.IsEnabled = true;
                        rePassBox.IsVisible = true;
                        welcLabel.Text = "Lets get started!";
                        logLabel.Text = "Register";

                    }
                    else
                    {
                        label.Text = "Sing up";
                        infoLabel.Text = "Don't have an account?";
                        loginButton.Text = "Login";
                        rePassBox.IsEnabled = false;
                        rePassBox.IsVisible = false;
                        welcLabel.Text = "Welcocome back! Please login to your account";
                        logLabel.Text = "Login";
                    }
                }
            }
        }

        private async void Login_Button_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var grid = button.Parent as Grid;
                if (grid != null)
                {
                    var passBox = grid.FindByName<Entry>("Passwor_box");
                    var rePassBox = grid.FindByName<Entry>("RePasswor_box");
                    var mailBox = grid.FindByName<Entry>("Mail_box");

                    if (button.Text == "Login")
                    {
                        var response = await UserContext.User.API_AuthorizeUserAsync(mailBox.Text, passBox.Text);
                        if (!response.Status)
                            await DisplayAlert("Error", response.Message, "OK");
                        else
                        {
                            var loginPanel = grid.Parent as Border;

                            loginPanel.IsVisible = false;

                            var parent = loginPanel.Parent as Grid;

                            var userPanel = grid.FindByName<Border>("User_border");

                            userPanel.IsVisible = true;

                            passBox.Text = "";
                            rePassBox.Text = "";
                            mailBox.Text = "";

                            // Animacja podskakiwania
                            await button.TranslateTo(0, -10, 100);  // przesuń w górę
                            await button.TranslateTo(0, 0, 100);    // przesuń z powrotem w dół


                            var result = await UserContext.User.API_GetAllSavedDatabasesAsync();

                            foreach (var database in UserContext.User.SavedDatabases)
                            {
                                SavedDatabases.Add(database.Name);
                            }
                        }

                    }
                    else
                    {
                        if(rePassBox.Text != passBox.Text)
                            await DisplayAlert("Error", "Password don't match!", "OK");

                        var response = await UserContext.User.API_RegisterUserAsync(mailBox.Text, passBox.Text);
                        if (!response.Status)
                            await DisplayAlert("Error", response.Message, "OK");

                        await DisplayAlert("Info", "You have been registered!", "OK");

                        // Animacja podskakiwania
                        await button.TranslateTo(0, -10, 100);  // przesuń w górę
                        await button.TranslateTo(0, 0, 100);    // przesuń z powrotem w dół

                        passBox.Text = "";
                        rePassBox.Text = "";
                        mailBox.Text = "";

                    }


                }

            }
        }

        private void LogoutButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                UserContext.User.Authorized = false;

                var grid = button.Parent as Grid;
                if (grid != null)
                {
                    var userPanel = grid.Parent as Border;

                    userPanel.IsVisible = false;

                    var loginPanel = grid.FindByName<Border>("Login_border");

                    loginPanel.IsVisible = true;
                }
            }
               
        }

        private async void Delete_buttor_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var context = button.BindingContext as string;
                if (context != null)
                {
                    var response = await UserContext.User.API_DeleteDatabaseAsync(context);
                    if (!response.Status)
                        await DisplayAlert("Error", response.Message, "OK");
                    else
                        SavedDatabases.Remove(context);
                }
            }
        }

        private async void Download_label_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var context = button.BindingContext as string;
                if (context != null)
                {
                    var response = await UserContext.User.API_GetDatabaseSchemaAsync(context);
                    if (!response.Status)
                        await DisplayAlert("Error", response.Message, "OK");
                    else
                        UserContext.Shemats.Add(context);

                }
            }
        }
    }
}
