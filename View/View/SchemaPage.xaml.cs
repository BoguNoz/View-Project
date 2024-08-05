using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Controls.Shapes;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using View.AppServices;
using View.DBSchema.Schemats;
using View.Helpers;

namespace View;

public partial class SchemaPage : ContentPage, INotifyPropertyChanged
{
    ObservableCollection<TableModel> DatabaseTables;

    private Queue<TableModel> On;
    private Queue<TableModel> Off;

    private bool taped = false;

    public SchemaPage()
    {
        InitializeComponent();

        DatabaseTables = new ObservableCollection<TableModel>();

        BookmarksCollectionView.ItemsSource = UserContext.Shemats;

        LogicalSchema.ItemsSource = DatabaseTables;

        On = new Queue<TableModel>();
        Off = new Queue<TableModel>();
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
                var model = new TableModel
                {
                    TableName = table.TableName,
                    TableColumns = table.TableColumns.ToList(),
                    Relationships = table.Relationships.ToList(),
                    PointColor = "#3700B3",
                    IsVisible = "True",
                };

                DatabaseTables.Add(model);
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

        Task.Delay(100);

        LogicalSchema.ItemsSource = LogicalSchema.ItemsSource;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var table = sender as Border;
        if(table != null)
        {
            var context = table.BindingContext as TableModel;

            if(taped == false)
            {
                var color = "#03DAC6";

                Dictionary<string, int> relations = new Dictionary<string, int>();

                relations.TryAdd(context.TableName, 0);

                foreach (var name in context.Relationships)
                {
                    var result = relations.TryAdd(name, 1);
                    if (result == false)
                        color = "#FDFD96";
                }

                foreach (var tab in DatabaseTables)
                {
                    if (relations.ContainsKey(tab.TableName)) On.Enqueue(tab);
                    else Off.Enqueue(tab);
                }


                Ping(color);

                taped = true;
            }
            else
            {

                UnPing();
                taped = false;
            }
          
        }
    }


    private void Ping(string color)
    {
        foreach(var table in Off)
        {
            table.IsVisible = "False";
        }

        foreach (var table in On)
        {
            table.PointColor = color;
        }
    }

    private void UnPing()
    {
        var c1 = Off.Count;
        var c2 = On.Count;

        for (int i = 0; i < c1; i++)
        {
            var temp = Off.Dequeue();
            temp.IsVisible = "True";
        }

        for (int i = 0; i < c2; i++)
        {
            var temp = On.Dequeue();
            temp.PointColor = "#3700B3";
        }
    }
}


public class TableModel : INotifyPropertyChanged
{

    private string pointColor;

    private string isVisible;

    public string TableName { get; set; } = string.Empty;

    public List<ColumnSchema> TableColumns { get; set; }

    public List<string> Relationships { get; set; }


    public string IsVisible
    {
        get => isVisible;
        set
        {
            if (isVisible != value)
            {
                isVisible = value;
                OnPropertyChanged();
            }
        }
    }

    public string PointColor
    {
        get => pointColor;
        set
        {
            if (pointColor != value)
            {
                pointColor = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


