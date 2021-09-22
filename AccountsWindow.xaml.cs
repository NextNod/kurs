using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace kurs
{
    /// <summary>
    /// Interaction logic for AccountsWindow.xaml
    /// </summary>
    public partial class AccountsWindow : Window
    {
        private readonly List<string> acsecc = new List<string>() { "Read", "Write & Read", "Denied" };
        private ObservableCollection<string?> vs;

        public AccountsWindow()
        {
            InitializeComponent();
            AccountSnack.MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(4000));

            Client.ItemsSource = acsecc;
            Order.ItemsSource = acsecc;
            Service.ItemsSource = acsecc;
            Specialist.ItemsSource = acsecc;
            Job.ItemsSource = acsecc;

            vs = new ObservableCollection<string?>(MainWindow.DataBase.Acc.Select(x => x.Login).AsEnumerable());
            AccountsList.ItemsSource = vs;
        }

        private Acc? acc;

        private void AccountsListChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                acc = MainWindow.DataBase.Acc.Where(x => x.Login == (string)AccountsList.SelectedItem).ToList()[0];

                Client.IsEnabled = !(acc.SysAdmin == 1);
                Order.IsEnabled = !(acc.SysAdmin == 1);
                Service.IsEnabled = !(acc.SysAdmin == 1);
                Specialist.IsEnabled = !(acc.SysAdmin == 1);
                Job.IsEnabled = !(acc.SysAdmin == 1);

                Client.SelectedIndex = acc.Client;
                Order.SelectedIndex = acc.Order;
                Service.SelectedIndex = acc.Serves;
                Specialist.SelectedIndex = acc.Special;
                Job.SelectedIndex = acc.Job;

                Login.Text = acc.Login;
                Password.Text = acc.Password;
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (acc != null)
            {
                for(int i = 0; i < vs.Count; i++)
                    if(vs[i] == acc.Login)
                        vs[i] = Login.Text;

                acc.Login = Login.Text;
                acc.Password = Password.Text;

                acc.Client = Client.SelectedIndex;
                acc.Order = Order.SelectedIndex;
                acc.Serves = Service.SelectedIndex;
                acc.Special = Specialist.SelectedIndex;
                acc.Job = Job.SelectedIndex;

                MainWindow.DataBase.Acc.Update(acc);
                MainWindow.DataBase.SaveChanges();
                AccountSnack.MessageQueue.Enqueue("Changes saved!");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string login = "Account" + Random.Shared.Next();
            var newAccount = new Acc() { 
                ID = MainWindow.DataBase.Acc.Max(x => x.ID) + 1,
                Login = login,
                Password = null,
                SysAdmin = 0,
                Client = (int)DB.Acsess.Denien,
                Order = (int)DB.Acsess.Denien,
                Serves = (int)DB.Acsess.Denien,
                Special = (int)DB.Acsess.Denien,
                Job = (int)DB.Acsess.Denien
            };

            MainWindow.DataBase.Acc.Add(newAccount);
            vs.Add(login);
            MainWindow.DataBase.SaveChanges();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var deleteLogin = (string)AccountsList.SelectedItem;
            AccountSnack.MessageQueue.Enqueue($"Do you want delete \"{deleteLogin}\"?", "Yes", () => { 
                AccountSnack.MessageQueue.Clear();
                var deleteAcc = MainWindow.DataBase.Acc.Where(x => x.Login == deleteLogin).ToList()[0];
                MainWindow.DataBase.Acc.Remove(deleteAcc);
                vs.Remove(deleteLogin);
                AccountsList.SelectedIndex = vs.Count - 1;
                MainWindow.DataBase.SaveChanges();
                AccountSnack.MessageQueue.Enqueue("Account deleted!");
            });
        }
    }
}
