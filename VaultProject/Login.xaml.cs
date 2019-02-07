using System.Windows;
using System.Windows.Input;

public enum LoginCallback
{
  Passed, NotPassed, Shutdown
}

namespace VaultProject
{
  public partial class Login : Window
  {
    private string password;

    private App.D_IsLogin _delegate;
    public Login(App.D_IsLogin sender)
    {
      InitializeComponent();
      Login_Text.Focus();
      password = Crypto.Decrypt(R.Get("Password"), CryptoMode.Password);
      _delegate = sender;

      if (password == "")
      {
        _delegate(LoginCallback.Passed);
        Close();
      }
      else
      {
        _delegate(LoginCallback.NotPassed);
        Show();
      }
        
    }

    private void FormDrag(object sender, MouseButtonEventArgs e)
    { 
      DragMove();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
      if (sender == Login_Close)
      {
        _delegate(LoginCallback.Shutdown);
        Close();
      }
      if (sender == Login_Enter)
      {
        if (Login_Text.Password == password)
        {
          _delegate(LoginCallback.Passed);
          Close();
        }
      }
    }
  }
}
