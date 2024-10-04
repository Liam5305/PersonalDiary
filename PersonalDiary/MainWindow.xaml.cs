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

namespace PersonalDiary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            labelDate.Content = DateTime.Now.Year.ToString();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Home homePage = new Home();
            homePage.Show();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            string exitMessage = "Are you sure you wish to close?";
            string caption = "App Closing";
            var result = MessageBox.Show(exitMessage, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}