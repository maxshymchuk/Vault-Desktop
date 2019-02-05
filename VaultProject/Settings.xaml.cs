using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace VaultProject
{
  public partial class Settings : Window
  {
    public Settings()
    {
      InitializeComponent();
      Settings_DataPath.Text = R.Get("DataPath");
      Settings_FileName.Text = R.Get("FileName");
      Settings_PasswordBox.Password = R.Get("Password");
      Settings_AutoRunButton.Content = R.IsAutoRunSet() ? "UNSET" : "SET";
    }

    private void Settings_ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
      if (Settings_DataPath.Text != "")
      {
        R.Set("DataPath", Settings_DataPath.Text);
        R.Set("FileName", Settings_FileName.Text);
      }
      if (Settings_PasswordBox.Password != "")
      {
        R.Set("Password", Settings_PasswordBox.Password);
      }
      Settings_Close();
    }

    private void Settings_Close()
    {
      this.Close();
    }

    private void Settings_OpenDataPath_Click(object sender, RoutedEventArgs e)
    {
      CommonOpenFileDialog dlg = new CommonOpenFileDialog
      {
        Title = "Choose folder",
        IsFolderPicker = true,
        EnsureFileExists = true,
        EnsurePathExists = true,
        EnsureReadOnly = false,
        EnsureValidNames = true,
        Multiselect = false,
        ShowPlacesList = true
      };
      if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
      {
        string folder = dlg.FileName;
        Settings_DataPath.Text = folder;
      }
    }

    private void Settings_ButtonClick(object sender, RoutedEventArgs e)
    {
      if (sender == Settings_AutoRunButton)
      {
        if (R.IsAutoRunSet())
        {
          R.UnsetAutoRun();
          Settings_AutoRunButton.Content = "SET";
        }
        else
        {
          R.SetAutoRun();
          Settings_AutoRunButton.Content = "UNSET";
        }
      }
      if (sender == Settings_SecureWordButton)
      {
        string newSecureWord = UsefulFunctions.GetUniqueKey(16);
        R.Set("SecureWord", newSecureWord);
        Vault.RewriteFile();
      }
      if (sender == Settings_EraseButton)
      {
        Vault.EraseData();
      }
    }

    private void Settings_CancelButton_Click(object sender, RoutedEventArgs e)
    {
      Settings_Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      (new Vault()).Show();
    }
  }
}

