using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.ObjectModel;
using View.AppServices;
using View.DBSchema.Schemats;
using View.Helpers;

namespace View;

public partial class SchemaPage : ContentPage
{
    ObservableCollection<TableSchema> DatabaseTables;

    public SchemaPage()
    {
        InitializeComponent();

        DatabaseTables = new ObservableCollection<TableSchema>();

        BookmarksCollectionView.ItemsSource = UserContext.Shemats;

        LogicalSchema.ItemsSource = DatabaseTables;
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
            var response = UserContext.User.Databases.FirstOrDefault(d => d.Key.Name == selectedItem).Key;
            if (response == null)
            {
                await DisplayAlert("Error", "Database not found", "OK");
                return;
            }

            DatabaseTables.Clear();

            foreach (var table in response.Tables)
            {
                DatabaseTables.Add(table);
            }
        }
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Ensure the value does not exceed 300
        if (e.NewValue > 200)
        {
            ((Slider)sender).Value = 200;
        }

        foreach (var item in DatabaseTables)
        {
            // Optionally, update the size of items if needed
            // This might be necessary if binding does not automatically update
        }

        LogicalSchema.ItemsSource = LogicalSchema.ItemsSource;
    }
}


