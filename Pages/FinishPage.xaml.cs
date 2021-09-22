using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace kurs.Pages
{
    public partial class FinishPage : Page
    {
        public FinishPage()
        {
            InitializeComponent();

            Name.Content = ClientWindow.Client.Name;
            LoginLabel.Content = ClientWindow.Client.Name;
            Phone.Content = ClientWindow.Client.Phone;
            Email.Content = ClientWindow.Client.Email;

            string[] workersDraft = ClientWindow.Order.ID_Special.Split(' ');
            string workers = "";
            foreach(var worker in workersDraft)
                if(worker != "")
                    workers += MainWindow.DataBase.Special.Where(x => x.ID == Convert.ToInt32(worker)).Select(x => x.Name).ToArray()[0] + ", ";

            team.Content = workers.Remove(workers.Length - 2);
            time.Content = ClientWindow.Order.Total_time;
            cost.Content = ClientWindow.Order.Cost;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.DataBase.Acc.Add(new Acc() {
                ID = MainWindow.DataBase.Acc.Max(x => x.ID) + 1,
                Login = ClientWindow.Client.Name,
                SysAdmin = -1,
                Password = Password.Text
            });
            MainWindow.DataBase.Client.Add(ClientWindow.Client);
            MainWindow.DataBase.SaveChanges();
            MainWindow.DataBase.Order.Add(ClientWindow.Order);
            MainWindow.DataBase.SaveChanges();
            MainWindow.MainSnackbar.MessageQueue.Enqueue("Order saved!");
            ClientWindow.Window.Close();
        }
    }
}
