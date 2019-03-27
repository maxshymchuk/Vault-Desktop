using System.Windows;
using System.Windows.Input;

public enum LoginCallback
{
  Passed, NotPassed, Shutdown
}

public enum Status
{
  Logged, Closed
}

namespace VaultProject
{
  public partial class Login : Window
  {
    private string password;
    private Status status = Status.Closed;

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

    private void Window_KeyUp(object sender, KeyEventArgs e)
    {
      if (Login_Text.Password == password)
      {
        status = Status.Logged;
        _delegate(LoginCallback.Passed);
        Close();
      }
    }

    private void Window_Closed(object sender, System.EventArgs e)
    {
      if (status == Status.Closed)
      {
        _delegate(LoginCallback.Shutdown);
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (sender == Login_CloseButton)
      {
        Close();
      }
      if (sender == Login_MinimizeButton)
      {
        WindowState = WindowState.Minimized;
      }
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        DragMove();
      }
    }
  }
}
