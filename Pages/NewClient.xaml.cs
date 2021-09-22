using System.Windows.Controls;
using System.Linq;
using kurs;

namespace kurs.Pages
{
    /// <summary>
    /// Interaction logic for NewClient.xaml
    /// </summary>
    public partial class NewClient : Page
    {
        public NewClient()
        {
            InitializeComponent();
            if (ClientWindow.Client != null)
            {
                Name.Text = ClientWindow.Client.Name;
                Phone.Text = ClientWindow.Client.Phone;
                EMail.Text = ClientWindow.Client.Email;
            }
        }

        private void Next(object sender, System.Windows.RoutedEventArgs e)
        {
            string[] data = new string[] { Name.Text, Phone.Text, EMail.Text };

            if (MainWindow.DataBase.Acc.Where(x => x.Login == Name.Text).ToList().Count() == 0)
            {
                Error.Visibility = System.Windows.Visibility.Collapsed;
                foreach (string item in data)
                    if (item == "")
                        return;

                if (data[1][0] != '+')
                    return;

                foreach (char item in data[2])
                {
                    if (item == '@')
                    {
                        int id = 1;

                        try { id = MainWindow.DataBase.Client.Max(x => x.ID) + 1; }
                        catch { }

                        ClientWindow.Client = new Client()
                        {
                            ID = id,
                            Name = data[0],
                            Phone = data[1],
                            Email = data[2]
                        };
                    }
                }
            }
            else Error.Visibility = System.Windows.Visibility.Visible;

            return;
        }
    }
}
