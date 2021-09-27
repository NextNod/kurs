using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace kurs
{
    public partial class PersonalAccount : Window
    {
        Order selectedOrder = new();
        readonly Client client;
        private readonly ObservableCollection<OrderObject> listGrid;
        private readonly ObservableCollection<string> listWorkers;
        private readonly ObservableCollection<string> listBoxObject;
        int selectedIndex = -1;
        readonly List<string> services = MainWindow.DataBase.Serves.Select(x => x.Name).ToList();

        class OrderObject
        {
            public string Type { get; set; }
            public double Cost { get; set; }
            public int Workers { get; set; }
            public DateTime Finish { get; set; }
        }

        public PersonalAccount(Acc acc)
        {
            InitializeComponent();
            string? name = acc.Login;
            client = MainWindow.DataBase.Client.Where(x => x.Name == name).First();
            Label.Content = $"Welcome back {name}!";
            var sourse = MainWindow.DataBase.Order.Where(x => x.ID_Client == client.ID).ToList();

            listGrid = new ObservableCollection<OrderObject>();
            listBoxObject = new ObservableCollection<string>();
            listWorkers = new ObservableCollection<string>();

            foreach (var item in sourse)
            {
                listGrid.Add(new OrderObject()
                {
                    Type = MainWindow.DataBase.Serves.Where(x => x.ID == item.ID_servese).First().Name,
                    Cost = item.Cost,
                    Finish = item.Order_date.AddDays(item.Total_time),
                    Workers = item.ID_Special.Split(' ').Length - 1
                });
            }

            Service.ItemsSource = services;
            Workers.ItemsSource = listWorkers;
            WorkersList.ItemsSource = listBoxObject;
            DataGrid.ItemsSource = listGrid;
            snackbar.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                OrderObject? selected = DataGrid.SelectedItem as OrderObject;
                selectedOrder = MainWindow.DataBase.Order.Where(x => x.ID_Client == client.ID && x.Cost == selected.Cost).First();
                var workers = selectedOrder.ID_Special.Split(' ');

                listBoxObject.Clear();
                for (int i = 0; i < workers.Length - 1; i++)
                    listBoxObject.Add(MainWindow.DataBase.Special.Where(x => x.ID == Convert.ToInt32(workers[i])).First().Name);

                for (int i = 0; i < services.Count; i++)
                {
                    if (selected.Type == services[i])
                    {
                        Service.SelectedIndex = i;
                        break;
                    }
                }

                ClearButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
            }
            catch { } 
        }

        private void Service_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                listWorkers.Clear();
                int jobID = MainWindow.DataBase.Serves.Where(x => x.Name == (string)Service.SelectedItem).First().ID_job;
                var workers = MainWindow.DataBase.Special.Where(x => x.ID_job == jobID).ToList();

                var allWorkers = new List<int>();
                var sourseWorkers = MainWindow.DataBase.Order.Select(x => x.ID_Special).ToList();
                foreach (var item in sourseWorkers)
                    foreach (var item2 in item.Split(' '))
                        if (item2 != "")
                            allWorkers.Add(Convert.ToInt32(item2));

                foreach (var item in workers)
                    if(!allWorkers.Contains(item.ID))
                        listWorkers.Add(item.Name);

                if (selectedIndex == DataGrid.SelectedIndex)
                    listBoxObject.Clear();
                    
                selectedIndex = DataGrid.SelectedIndex;
            } catch { }
        }

        private void Workers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!listBoxObject.Contains((string)Workers.SelectedItem))
                listBoxObject.Add((string)Workers.SelectedItem);

            ClearButton.IsEnabled = !(listBoxObject.Count == 0);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            listBoxObject.Clear();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var service = MainWindow.DataBase.Serves.Where(x => x.Name == (string)Service.SelectedItem).First();
            string workers = "";
            
            selectedOrder.Total_time = service.Time_to_done;
            foreach (var item in listBoxObject)
            {
                selectedOrder.Total_time -= selectedOrder.Total_time * 0.15;
                workers += MainWindow.DataBase.Special.Where(x => x.Name == item).First().ID + " ";
            }
            
            selectedOrder.ID_servese = service.ID;
            selectedOrder.Cost = service.Cost * listBoxObject.Count;
            selectedOrder.ID_Special = workers;



            MainWindow.DataBase.Order.Update(selectedOrder);
            ClearButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            DataGrid.SelectedIndex = -1;
            listGrid.Clear();

            var sourse = MainWindow.DataBase.Order.Where(x => x.ID_Client == client.ID).ToList();
            foreach (var item in sourse)
            {
                listGrid.Add(new OrderObject()
                {
                    Type = MainWindow.DataBase.Serves.Where(x => x.ID == item.ID_servese).First().Name,
                    Cost = item.Cost,
                    Finish = item.Order_date.AddDays(item.Total_time),
                    Workers = item.ID_Special.Split(' ').Length - 1
                });
            }

            MainWindow.DataBase.SaveChanges();

            listBoxObject.Clear();
            listWorkers.Clear();
            Service.SelectedIndex = -1;

            snackbar.MessageQueue.Enqueue("Changes saved!");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var service = MainWindow.DataBase.Serves.Where(x => x.Name == (string)Service.SelectedItem).First();
            string workers = "";

            selectedOrder.Total_time = service.Time_to_done;
            foreach (var item in listBoxObject)
            {
                selectedOrder.Total_time -= selectedOrder.Total_time * 0.15;
                workers += MainWindow.DataBase.Special.Where(x => x.Name == item).First().ID + " ";
            }

            selectedOrder.ID = MainWindow.DataBase.Order.Max(x => x.ID) + 1;
            selectedOrder.ID_Client = client.ID;
            selectedOrder.Order_date = DateTime.Now;
            selectedOrder.ID_servese = service.ID;
            selectedOrder.Cost = service.Cost * listBoxObject.Count;
            selectedOrder.ID_Special = workers;


            MainWindow.DataBase.Order.Add(selectedOrder);
            ClearButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            DataGrid.SelectedIndex = -1;
            listGrid.Clear();

            var sourse = MainWindow.DataBase.Order.Where(x => x.ID_Client == client.ID).ToList();
            foreach (var item in sourse)
            {
                listGrid.Add(new OrderObject()
                {
                    Type = MainWindow.DataBase.Serves.Where(x => x.ID == item.ID_servese).First().Name,
                    Cost = item.Cost,
                    Finish = item.Order_date.AddDays(item.Total_time),
                    Workers = item.ID_Special.Split(' ').Length - 1
                });
            }

            MainWindow.DataBase.SaveChanges();

            listBoxObject.Clear();
            listWorkers.Clear();
            Service.SelectedIndex = -1;

            snackbar.MessageQueue.Enqueue("New order was added!");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (listGrid.Count != 0)
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Excel table|*.xlsx",
                    FileName = "Report.xlsx"
                };
                saveFileDialog.FileOk += (o, ee) => 
                {
                    FileInfo filePath = new(saveFileDialog.FileName);

                    using (var excelPack = new ExcelPackage(filePath))
                    {
                        var ws = excelPack.Workbook.Worksheets.Add("Orders");
                        ws.Cells.LoadFromDataTable(AdminPanel.ToDataTable(listGrid.ToList()), true, OfficeOpenXml.Table.TableStyles.Light8);
                        for (int i = 1; i < 5; i++) ws.Column(i).Width = 15;
                        excelPack.Save();
                    }

                    snackbar.MessageQueue.Enqueue("The data was successfully exported!");
                };
                saveFileDialog.ShowDialog();
            }
            else {
                snackbar.MessageQueue.Enqueue("No data to export!");
            }
        }
    }
}