using System.Collections.ObjectModel;
using View.AppServices;
using View.DBSchema.Schemats;
using View.DBSchema;
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Maui.DataGrid;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.PortableExecutable;


namespace View;

public partial class TablePage : ContentPage, INotifyPropertyChanged
{
    private string openDatabase;
    private string tableNavigation;
    private string navigation;

    public string Navigation
    {
        get => navigation;
        set
        {
            if (navigation != value)
            {
                navigation = value;
                OnPropertyChanged();
            }
        }
    }

    public string TableNavigation
    {
        get => tableNavigation;
        set
        {
            if (tableNavigation != value)
            {
                tableNavigation = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;


    public ObservableCollection<string> DatabaseTables { get; set; }

    public TablePage()
    {
        InitializeComponent();


        DatabaseTables = new ObservableCollection<string>();

        BookmarksCollectionView.ItemsSource = UserContext.Databases;

        TablesCollectionView.ItemsSource = DatabaseTables;

        BindingContext = this;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            if (database.Key == null)
                await DisplayAlert("Error", "Database can not be open", "OK");

            DatabaseTables.Clear();
            openDatabase = selectedItem;

            foreach (var table in database.Key.Tables)
            {
                DatabaseTables.Add(table.TableName);
            }

            Navigation = selectedItem.ToUpper();

            var response = await UserContext.User.File_GetDatabseContentAsync(selectedItem);
            if (!response.Status)
                await DisplayAlert("Error", response.Message, "OK");
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var border = sender as Border;

        if (border != null)
        {
            var context = border.BindingContext as string;
            if (context != null)
            {
                if (!UserContext.Databases.Any(n => n == openDatabase))
                {
                    await DisplayAlert("Error", "Table can not be open", "OK");
                    openDatabase = string.Empty;
                    DatabaseTables.Clear();
                    return;
                }

                TableNavigation = context.ToUpper();

                var database = UserContext.User.Databases.FirstOrDefault(d => d.Key.Name == openDatabase).Key;

                var table = database.Tables.FirstOrDefault(t => t.TableName == context);

                if (table == null)
                {
                    await DisplayAlert("Error", "Table not found", "OK");
                    return;
                }

                dataGrid.Columns.Clear(); // Czyœcimy istniej¹ce kolumny

                var rowData = await AddTable(table);

                // Ustawienie Ÿród³a danych
                dataGrid.ItemsSource = rowData;
            }
        }
    }

    private async Task<List<Dictionary<string, string>>> AddTable(TableSchema? table)
    {
        var rowData = new List<Dictionary<string, string>>();
        var columnCount = table.TableColumns.Count;
        var rowCount = table.TableColumns.First().ColumnData.Count;

        foreach (var column in table.TableColumns)
        {
            dataGrid.Columns.Add(new DataGridColumn
            {
                Title = column.ColumnName,
                PropertyName = column.ColumnName
            });
        }

        for (int i = 0; i < rowCount; i++)
        {
            var rowDict = new Dictionary<string, string>();
            foreach (var column in table.TableColumns)
            {
                rowDict[column.ColumnName] = column.ColumnData.ElementAt(i) ?? string.Empty;
            }
            rowData.Add(rowDict);
        }

        return rowData;
    }
}



