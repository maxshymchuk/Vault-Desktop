using System.Windows;
using System.Windows.Input;

namespace VaultProject
{
  /// <summary>
  /// Логика взаимодействия для Login.xaml
  /// </summary>
  public partial class Login : Window
  {
    public static string password;

    public Login()
    {
      InitializeComponent();
      Login_Text.Focus();
      password = "pass";
    }

    private void FormDrag(object sender, MouseButtonEventArgs e)
    { 
      DragMove();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
      if (sender == Login_Close)
      {
        Close();
      }
      if (sender == Login_Enter)
      {
        if (Login_Text.Password == password)
        {
          (new Vault()).Show();
          Close();
        }
      }
    }
  }
}
