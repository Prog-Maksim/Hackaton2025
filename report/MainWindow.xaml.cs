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
        
        LoadPriceCategoryOne(result, powerCost, energyCost);

        
        List<double> series = result.Select(r => r.EnergyByHours.Sum()).ToList();

        SeriesData.Values = new ChartValues<double>(series);
        
        AxisX.Labels = result
            .Select(r => DateTime.Parse(r.Data).ToString("yyyy-MM-dd"))
            .ToList();
    }

    private void LoadPriceCategoryOne(List<Root> result, double powerCost, (double, double, double) energyCost)
    {
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
        double tariff;
        
        
        if (totalPower < 670)
            tariff = energyCost.Item1;
        else if (totalPower < 10000)
            tariff = energyCost.Item2;
        else
            tariff = energyCost.Item3;
        
        cost += CostCalculator.PriceCategory1(totalPower, tariff); 
        cost = Math.Round(cost, 2);
        
        List<PriceData> datas = new()
        {
            new PriceData { PriceCategory = "Ценовая категория 1", Price = cost.ToString()},
        };

        AddPriceData(datas);
    }

    private void LoadPriceCategoryTwo()
    {
        
    }
    
    public void AddPriceData(List<PriceData> priceData)
    {
        foreach (var data in priceData)
            prices.Add(new PriceData { PriceCategory = data.PriceCategory, Price = data.Price + '\u20bd', Tag = data.Tag });
        
        PriceCategory.ItemsSource = prices;
    }
}