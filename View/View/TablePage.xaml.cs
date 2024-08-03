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
using System.Windows.Input;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace View;


public partial class TablePage : ContentPage, INotifyPropertyChanged
{

    private ObservableCollection<string> tables;

    private ObservableCollection<ColumnModel> tableColumns;
    private DatabaseSchema openDatabase;
    private TableSchema openTable;
    private CancellationTokenSource cancellationTokenSource;

    public TablePage()
    {
        InitializeComponent();

        tables = new ObservableCollection<string>();
        TableList.ItemsSource = tables;

        tableColumns = new ObservableCollection<ColumnModel>();
        TableView.ItemsSource = tableColumns;
        BookmarksCollectionView.ItemsSource = UserContext.Databases;
        BindingContext = this;
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
            var gatherData = await UserContext.User.File_GetDatabseContentAsync(selectedItem);
            if (!gatherData.Status)
            {
                await DisplayAlert("Error", gatherData.Message, "OK");
                return;
            }

            openDatabase = UserContext.User.Databases.FirstOrDefault(key => key.Key.Name == selectedItem).Key;
            if (openDatabase == null)
            {
                await DisplayAlert("Error", "Database can not be open", "OK");
                return;
            }

            tableColumns.Clear();

            tables.Clear();
            foreach (var table in openDatabase.Tables)
                tables.Add(table.TableName);

        }
    }

    private async Task<ColumnSchema> CreateColumnFrame(string columnName)
    {
        var column = openTable.TableColumns.FirstOrDefault(n => n.ColumnName == columnName);
        if (column == null)
        {
            await DisplayAlert("Error", "Table can not be found", "OK");
        }

        var model = new ColumnModel
        {
            ColumnName = column.ColumnName,
            IsItForeignKey = column.IsItForeignKey,
            IsItPrimaryKey = column.IsItPrimaryKey,
            ColumnData = new ObservableCollection<string>(),
            ColumnDataType = column.ColumnDataType
        };

        tableColumns.Add(model);

        return column;
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        int batchSize = 2;
        bool dataRemaining;

        do
        {
            dataRemaining = false;

            foreach (var column in openTable.TableColumns)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var model = tableColumns.FirstOrDefault(c => c.ColumnName == column.ColumnName);
                if (model != null)
                {
                    var columnDataList = column.ColumnData.ToList();
                    int currentCount = model.ColumnData.Count;

                    if (currentCount < columnDataList.Count)
                    {
                        dataRemaining = true;
                        int itemsToLoad = Math.Min(batchSize, columnDataList.Count - currentCount);

                        for (int i = 0; i < itemsToLoad; i++)
                        {
                            model.ColumnData.Add(columnDataList[currentCount + i] == null ? "  " : columnDataList[currentCount + i]);
                        }
                    }
                }
            }

            await Task.Delay(150);

        } while (dataRemaining);
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var border = sender as Border;

        if (border != null)
        {
            var context = border.BindingContext as string;

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

            cancellationTokenSource = new CancellationTokenSource();

            if (context != null)
            {
                var table = openDatabase.Tables.FirstOrDefault(n => n.TableName == context);
                if (table == null)
                {
                    await DisplayAlert("Error", "Table can not be found", "OK");
                    return;
                }

                openTable = table;

                tableColumns.Clear();

                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Cancel();
                }

                cancellationTokenSource = new CancellationTokenSource();

                foreach (var column in openTable.TableColumns)
                {
                    await CreateColumnFrame(column.ColumnName);
                }

                await LoadDataAsync(cancellationTokenSource.Token);
            }
        }
    }
}

public class ColumnModel
{
    public string ColumnName { get; set; }
    public ObservableCollection<string> ColumnData { get; set; }
    public string ColumnDataType { get; set; }
    public bool IsItPrimaryKey { get; set; }
    public bool IsItForeignKey { get; set; }
    public bool isLoading { get; set; }
    public int itemsLoaded { get; set; }
}





