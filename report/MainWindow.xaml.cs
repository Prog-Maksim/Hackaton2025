using System.Collections.ObjectModel;
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
using LiveCharts;
using LiveCharts.Wpf;
using report.Models;
using report.Scripts;

namespace report;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly List<Root> _datas;
    private readonly double tariff;
        
    public class PriceData
    {
        public required string PriceCategory { get; set; }
        public required string Price { get; set; }
        public string? Tag { get; set; }
    }
    
    private ObservableCollection<PriceData> prices = new ();
    
    public MainWindow(List<Root> result, double powerCost, (double, double, double) energyCost)
    {
        InitializeComponent();
        _datas = result;
        
        double[,] energy = new double[result.Count, 24];
        double totalPower = 0;

        for (int day = 0; day < result.Count; day++)
        {
            for (int hour = 0; hour < 24; hour++)
            {
                double power = result[day].EnergyByHours[hour];
                energy[day, hour] = power;
                totalPower += power;
            }
        }
        double cost = powerCost * totalPower;
        
        
        if (totalPower < 670)
            tariff = energyCost.Item1;
        else if (totalPower < 10000)
            tariff = energyCost.Item2;
        else
            tariff = energyCost.Item3;
        
        LoadPriceCategoryOne(result, totalPower, cost);
        
        
        Plot1.AxisX.Labels = result
            .Select(r => DateTime.Parse(r.Data).ToString("dd.MM.yyyy"))
            .ToList();
        
        CalculatePower();
    }

    private void LoadPriceCategoryOne(List<Root> result, double totalPower, double cost)
    {
        cost += CostCalculator.PriceCategory1(totalPower, tariff); 
        cost = Math.Round(cost, 2);
        
        List<PriceData> datas = new()
        {
            new PriceData { PriceCategory = "Ценовая категория 1", Price = cost.ToString()},
        };

        AddPriceData(datas);
        
    }

    private void CalculatePrice()
    {
        double[,] power1 = new double[_datas.Count, 24];

        for (int i = 0; i < _datas.Count; i++)
        {
            if (_datas[i].EnergyByHours.Count != 24)
            {
                throw new InvalidOperationException($"Ошибка в дне {_datas[i].Data}: количество значений не равно 24.");
            }

            for (int j = 0; j < 24; j++)
            {
                power1[i, j] = _datas[i].EnergyByHours[j];
            }
        }
        
        double[,] result = CostCalculator.PriceCategory1(power1, tariff);
        
        int days = result.GetLength(0);
        int hours = result.GetLength(1);

        List<double> series = new List<double>();

        for (int i = 0; i < days; i++)
        {
            double dailySum = 0;
            for (int j = 0; j < hours; j++)
            {
                dailySum += result[i, j];
            }
            series.Add(dailySum);
        }
        Plot1.SeriesData.Values = new ChartValues<double>(series);
    }

    private void CalculatePower()
    {
        List<double> series = _datas.Select(r => Math.Round(r.EnergyByHours.Sum(), 2)).ToList();
        Plot1.SeriesData.Values = new ChartValues<double>(series);
    }
    
    public void AddPriceData(List<PriceData> priceData)
    {
        foreach (var data in priceData)
            prices.Add(new PriceData { PriceCategory = data.PriceCategory, Price = data.Price + '\u20bd', Tag = data.Tag });
        
        PriceCategory.ItemsSource = prices;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        AddFile mainMenu = new AddFile();
        mainMenu.Show();
        Close();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            int selectedIndex = ComboBoxMain.SelectedIndex;

            if (selectedIndex == 0)
                CalculatePower();
            else if (selectedIndex == 1)
                CalculatePrice();
        }
        catch (Exception exception)
        {
            
        }
    }
}