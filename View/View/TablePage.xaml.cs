using System.Collections.ObjectModel;
using View.AppServices;
using View.DBSchema.Schemats;
using View.DBSchema;
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.Core;


namespace View;

public partial class TablePage : ContentPage
{

    public ObservableCollection<string> DatabaseTables { get; set; }

    public TablePage()
    {

        InitializeComponent();

        DatabaseTables = new ObservableCollection<string>();

        BookmarksCollectionView.ItemsSource = UserContext.Databases;

        TablesCollectionView.ItemsSource = DatabaseTables;

    }

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        VisualStateManager.GoToState((VisualElement)sender, "MouseOver");
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        VisualStateManager.GoToState((VisualElement)sender, "Normal");
    }


    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as string;

        if (selectedItem != null)
        {

            var database = UserContext.User.Databases.FirstOrDefault(key => key.Key.Name == selectedItem); 
            if(database.Key == null)
                await DisplayAlert("Error", "Database can not be open", "OK");

            DatabaseTables.Clear();
            
            foreach(var table in database.Key.Tables) 
            {
                DatabaseTables.Add(table.TableName);
            }
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }
}


