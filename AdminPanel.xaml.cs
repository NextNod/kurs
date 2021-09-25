using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace kurs
{
    public partial class AdminPanel : Window
    {
        class TableOrder
        {
            public int ID { set; get; }
            public DateTime Finish { set; get; }
            public string Service { set; get; }
            public string Client { set; get; }
            public string Workers { set; get; }
            public double Cost { set; get; }

        }

        class TableWorker
        {
            public int ID { set; get; }
            public string Name { set; get; }
            public DateTime Birthday { set; get; }
            public int Expiriance { set; get; }
            public string Job { set; get; }
        }

        class TableService
        {
            public int ID { set; get; }
            public string Name { set; get; }
            public int Time { set; get; }
            public string Job { set; get; }
            public double Cost { set; get; }
        }

        private readonly string[] allTables = new string[] { "Clients", "Orders", "Workers", "Jobs", "Servises" };
        private ObservableCollection<string> tables;
        private int ID;

        public AdminPanel(int ID)
        {
            InitializeComponent();
            this.ID = ID;
            tables = new ObservableCollection<string>();
            SnackAdmin.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));
            Tables.ItemsSource = tables;

            if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.SysAdmin == 1).ToArray()[0])
            {
                foreach (string tableName in allTables)
                    tables.Add(tableName);
            }
            else
            {
                AccountButton.Visibility = Visibility.Hidden;
                if ((DB.Acsess)MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Job).ToArray()[0] != DB.Acsess.Denien)
                    tables.Add(allTables[3]);

                if ((DB.Acsess)MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Order).ToArray()[0] != DB.Acsess.Denien)
                    tables.Add(allTables[1]);

                if ((DB.Acsess)MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Serves).ToArray()[0] != DB.Acsess.Denien)
                    tables.Add(allTables[4]);

                if ((DB.Acsess)MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Special).ToArray()[0] != DB.Acsess.Denien)
                    tables.Add(allTables[2]);

                if ((DB.Acsess)MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Client).ToArray()[0] != DB.Acsess.Denien)
                    tables.Add(allTables[0]);
            }
        }

        private void TablesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool isAdmin = MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.SysAdmin == 1).ToArray()[0];

            switch ((string)Tables.SelectedItem)
            {
                case "Clients":
                    DataGrid.ItemsSource = MainWindow.DataBase.Client.Select(x => x).ToList();
                    if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Client).ToArray()[0] == (int)DB.Acsess.Read)
                        DataGrid.IsReadOnly = true;
                    else DataGrid.IsReadOnly = false;
                    break;

                case "Orders":
                    {
                        var sourse = MainWindow.DataBase.Order.ToList();
                        var orders = new ObservableCollection<TableOrder>();

                        foreach (var item in sourse)
                        {
                            string workers = "";
                            var temp = item.ID_Special.Split(' ');

                            for (int i = 0; i < temp.Length - 1; i++)
                                workers += MainWindow.DataBase.Special.Where(x => x.ID == Convert.ToInt32(temp[i])).First().Name + ", ";

                            orders.Add(new TableOrder()
                            {
                                ID = item.ID,
                                Finish = item.Order_date.AddDays(item.Total_time),
                                Service = MainWindow.DataBase.Serves.Where(x => x.ID == item.ID_servese).First().Name,
                                Client = MainWindow.DataBase.Client.Where(x => x.ID == item.ID_Client).First().Name,
                                Workers = workers.Remove(workers.Length - 2),
                                Cost = item.Cost
                            });
                        }

                        DataGrid.ItemsSource = orders;

                        if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Order).ToArray()[0] == (int)DB.Acsess.Read)
                            DataGrid.IsReadOnly = true;
                        else DataGrid.IsReadOnly = false;
                        break;
                    }

                case "Workers":
                    {
                        var sourse = MainWindow.DataBase.Special.Select(x => x).ToList();
                        var workers = new ObservableCollection<TableWorker>();

                        foreach (var item in sourse)
                            workers.Add(new TableWorker()
                            {
                                ID = item.ID,
                                Birthday = item.Birthday,
                                Name = item.Name,
                                Expiriance = item.Expiriance,
                                Job = MainWindow.DataBase.job.Where(x => x.ID == item.ID_job).First().Name_of_job
                            });

                        DataGrid.ItemsSource = workers;

                        if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Special).ToArray()[0] == (int)DB.Acsess.Read)
                            DataGrid.IsReadOnly = true;
                        else DataGrid.IsReadOnly = false;
                        break;
                    }

                case "Servises":
                    {
                        var sourse = MainWindow.DataBase.Serves.Select(x => x).ToList();
                        var services = new ObservableCollection<TableService>();

                        foreach (var item in sourse)
                            services.Add(new TableService()
                            {
                                ID = item.ID,
                                Name = item.Name,
                                Time = item.Time_to_done,
                                Job = MainWindow.DataBase.job.Where(x => x.ID == item.ID_job).First().Name_of_job,
                                Cost = item.Cost
                            });

                        DataGrid.ItemsSource = services;

                        if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Serves).ToArray()[0] == (int)DB.Acsess.Read)
                            DataGrid.IsReadOnly = true;
                        else DataGrid.IsReadOnly = false;
                        break;
                    }

                case "Jobs":
                    DataGrid.ItemsSource = MainWindow.DataBase.job.Select(x => x).ToList();
                    if (MainWindow.DataBase.Acc.Where(x => x.ID == ID).Select(x => x.Job).ToArray()[0] == (int)DB.Acsess.Read)
                        DataGrid.IsReadOnly = true;
                    else DataGrid.IsReadOnly = false;
                    break;
            }
        }

        private void DataGrid_AddingNewItem(object sender, System.Windows.Controls.AddingNewItemEventArgs e)
        {
            switch ((string)Tables.SelectedItem)
            {
                case "Clients":
                    e.NewItem = new Client()
                    {
                        ID = MainWindow.DataBase.Client.Max(x => x.ID) + 1,
                        Name = "Example"
                    };
                    MainWindow.DataBase.Client.Add((Client)e.NewItem);
                    break;

                case "Orders":
                    {
                        int idServes = MainWindow.DataBase.Serves.Max(x => x.ID);
                        var temp = new Order()
                        {
                            ID = MainWindow.DataBase.Order.Max(x => x.ID) + 1,
                            Cost = MainWindow.DataBase.Serves.Where(x => x.ID == idServes).Select(x => x.Cost).ToArray()[0],
                            Total_time = MainWindow.DataBase.Serves.Where(x => x.ID == idServes).Select(x => x.Time_to_done).ToArray()[0],
                            Order_date = DateTime.Now,
                            ID_Client = MainWindow.DataBase.Client.Max(x => x.ID),
                            ID_servese = idServes,
                            ID_Special = Convert.ToString(MainWindow.DataBase.Special.Max(x => x.ID))
                        };

                        e.NewItem = new TableOrder()
                        {
                            ID = temp.ID,
                            Cost = temp.Cost,
                            Finish = temp.Order_date.AddDays(temp.Total_time),
                            Client = MainWindow.DataBase.Client.Where(x => x.ID == temp.ID_Client).First().Name,
                            Service = MainWindow.DataBase.Serves.Where(x => x.ID == temp.ID_servese).First().Name,
                            Workers = MainWindow.DataBase.Special.Where(x => x.ID == Convert.ToInt32(temp.ID_Special)).First().Name
                        };

                        MainWindow.DataBase.Order.Add(temp);
                        break;
                    }

                case "Workers":
                    {
                        var temp = new Special()
                        {
                            ID = MainWindow.DataBase.Special.Max(x => x.ID) + 1,
                            Birthday = DateTime.MinValue,
                            ID_job = MainWindow.DataBase.job.Max(x => x.ID),
                            Expiriance = 0,
                            Name = "Example"
                        };

                        e.NewItem = new TableWorker()
                        {
                            ID = temp.ID,
                            Birthday = temp.Birthday,
                            Expiriance = temp.Expiriance,
                            Job = MainWindow.DataBase.job.Where(x => x.ID == temp.ID_job).First().Name_of_job,
                            Name = temp.Name
                        };

                        MainWindow.DataBase.Special.Add(temp);
                        break;
                    }

                case "Servises":
                    {
                        var temp = new Serves()
                        {
                            ID = MainWindow.DataBase.Serves.Max(x => x.ID) + 1,
                            Time_to_done = 0,
                            Cost = 0,
                            Name = "Example",
                            ID_job = MainWindow.DataBase.job.Max(x => x.ID)
                        };

                        e.NewItem = new TableService()
                        {
                            ID = temp.ID,
                            Name = temp.Name,
                            Cost = temp.Cost,
                            Time = temp.Time_to_done,
                            Job = MainWindow.DataBase.job.Where(x => x.ID == temp.ID_job).First().Name_of_job
                        };

                        MainWindow.DataBase.Serves.Add(temp);
                        break;
                    }

                case "Jobs":
                    e.NewItem = new job()
                    {
                        ID = MainWindow.DataBase.job.Max(x => x.ID) + 1,
                        Name_of_job = "Example"
                    };
                    MainWindow.DataBase.job.Add((job)e.NewItem);
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var deletItem = DataGrid.SelectedItem;

            switch ((string)Tables.SelectedItem)
            {
                case "Clients":
                    MainWindow.DataBase.Client.Remove((Client)deletItem);
                    break;

                case "Orders":
                    {
                        var temp = MainWindow.DataBase.Order.Where(x => x.ID == ((TableOrder)deletItem).ID).First();
                        MainWindow.DataBase.Order.Remove(temp);
                        break;
                    }

                case "Workers":
                    {
                        var temp = MainWindow.DataBase.Special.Where(x => x.ID == ((TableWorker)deletItem).ID).First();
                        MainWindow.DataBase.Special.Remove(temp);
                        break;
                    }

                case "Servises":
                    {
                        var temp = MainWindow.DataBase.Serves.Where(x => x.ID == ((TableService)deletItem).ID).First();
                        MainWindow.DataBase.Serves.Remove(temp);
                        break;
                    }

                case "Jobs":
                    MainWindow.DataBase.job.Remove((job)deletItem);
                    break;
            }

            TablesChanged(null, null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.DataBase.SaveChanges();
            SnackAdmin.MessageQueue.Enqueue("Change saved!");
        }

        AccountsWindow? window;

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (window == null)
            {
                window = new AccountsWindow();
                window.Closing += (o, e) => window = null;
                window.Show();
            }
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                if (type.FullName == "System.DateTime")
                    dataTable.Columns.Add(prop.Name, typeof(string));
                else
                    dataTable.Columns.Add(prop.Name, type);
            }

            foreach (T item in items)
            {
                var values = new List<object>();
                foreach (var temp in Props)
                {
                    if(temp.PropertyType.FullName == "System.DateTime")
                        values.Add(((DateTime)temp.GetValue(item, null)).ToShortDateString());
                    else
                        values.Add(temp.GetValue(item, null));
                }

                dataTable.Rows.Add(values.ToArray());
            }

            return dataTable;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel table|.xlsx";
                saveFileDialog.ShowDialog();

                FileInfo filePath = new FileInfo(saveFileDialog.FileName);

                var serviceSourse = MainWindow.DataBase.Serves.ToList();
                var services = new List<TableService>();

                foreach (var item in serviceSourse)
                    services.Add(new TableService()
                    {
                        ID = item.ID,
                        Name = item.Name,
                        Time = item.Time_to_done,
                        Job = MainWindow.DataBase.job.Where(x => x.ID == item.ID_job).First().Name_of_job,
                        Cost = item.Cost
                    });

                var workersSourse = MainWindow.DataBase.Special.Select(x => x).ToList();
                var workers = new List<TableWorker>();

                foreach (var item in workersSourse)
                    workers.Add(new TableWorker()
                    {
                        ID = item.ID,
                        Birthday = item.Birthday,
                        Name = item.Name,
                        Expiriance = item.Expiriance,
                        Job = MainWindow.DataBase.job.Where(x => x.ID == item.ID_job).First().Name_of_job
                    });

                var orderSourse = MainWindow.DataBase.Order.ToList();
                var orders = new List<TableOrder>();

                foreach (var item in orderSourse)
                {
                    string worker = "";
                    var temp = item.ID_Special.Split(' ');

                    for (int i = 0; i < temp.Length - 1; i++)
                        worker += MainWindow.DataBase.Special.Where(x => x.ID == Convert.ToInt32(temp[i])).First().Name + ", ";

                    orders.Add(new TableOrder()
                    {
                        ID = item.ID,
                        Finish = item.Order_date.AddDays(item.Total_time),
                        Service = MainWindow.DataBase.Serves.Where(x => x.ID == item.ID_servese).First().Name,
                        Client = MainWindow.DataBase.Client.Where(x => x.ID == item.ID_Client).First().Name,
                        Workers = worker.Remove(worker.Length - 2),
                        Cost = item.Cost
                    });
                }

                List<(DataTable, string)> vars = new List<(DataTable, string)> {
                    (ToDataTable(orders), "Order"),
                    (ToDataTable(MainWindow.DataBase.Client.ToList()), "Client"),
                    (ToDataTable(MainWindow.DataBase.job.ToList()), "Job"),
                    (ToDataTable(services), "Sevice"),
                    (ToDataTable(workers), "Workers")
                };

                using (var excelPack = new ExcelPackage(filePath))
                {
                    foreach (var item in vars)
                    {
                        var ws = excelPack.Workbook.Worksheets.Add(item.Item2);
                        ws.Cells.LoadFromDataTable(item.Item1, true, OfficeOpenXml.Table.TableStyles.Light8);
                    }
                    excelPack.Save();
                }

                SnackAdmin.MessageQueue.Enqueue("Export sucsecc!");
            }
            catch { }
        }
    }
}
