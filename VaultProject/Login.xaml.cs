using System.Windows;
using System.Windows.Input;

namespace VaultProject
{
  /// <summary>
  /// Логика взаимодействия для Login.xaml
  /// </summary>
  public partial class Login : Window
  {
    public Login()
    {
      InitializeComponent();
      Login_Text.Focus();
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
        if (Login_Text.Password == "pass")
        {
          (new Vault()).Show();
          Close();
        }
      }
    }
  }
}
