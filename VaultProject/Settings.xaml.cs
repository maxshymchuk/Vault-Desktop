using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;




/*
 * Переписать на реестр
 * Убрать статик
 */
namespace VaultProject
{
  /// <summary>
  /// Логика взаимодействия для Settings.xaml
  /// </summary>
  public partial class Settings : Page
  {
    public Settings()
    {
      InitializeComponent();
      Settings_DataPath.Text = $"{Vault.file.path}\\{Vault.file.name}";
      Settings_PasswordBox.Password = Login.password;
      Settings_SecureName.Password = Crypto.passPhrase;
    }

    private void Settings_ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
      if (Settings_DataPath.Text != "")
      {
        Vault.file.path = Settings_DataPath.Text;
      }
      if (Settings_PasswordBox.Password != "")
      {
        Login.password = Settings_PasswordBox.Password;
      }
      if (Settings_SecureName.Password != "")
      {
        Crypto.passPhrase = Settings_SecureName.Password;
        Vault.RewriteFile();
      }
    }

    private void Settings_OpenDataPath_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      Nullable<bool> result = dialog.ShowDialog();
      if (result == true)
      {
        Vault.file.path = System.IO.Path.GetDirectoryName(dialog.FileName);
        Vault.file.name = System.IO.Path.GetFileName(dialog.FileName);
      }
    }
  }
}
