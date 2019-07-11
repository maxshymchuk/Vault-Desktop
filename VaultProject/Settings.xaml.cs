using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace VaultProject
{
  public partial class Settings : Window
  {

    private App.D_IsSetupOver _delegate;

    public Settings(App.D_IsSetupOver sender)
    {
      InitializeComponent();
      Settings_DataPath.Text = R.Get<string>("DataPath");
      Settings_FileName.Text = R.Get<string>("FileName");
      Settings_LogoutTimeout.Text = (R.Get<int>("LogoutTimeout")).ToString();
      Settings_PasswordBox.Password = Crypto.Decrypt(R.Get<string>("Password"), CryptoMode.Password);
      Settings_BackupPath.Text = R.Get<string>("BackupPath");
      Settings_BackupInterval.Text = (R.Get<int>("BackupInterval")).ToString();
      Settings_AutoRunButton.Content = R.IsAutoRunSet() ? "UNSET" : "SET";
      Settings_AutoRunMode.Content = R.Get<string>("AutoRunWindowMode");
      _delegate = sender;
    }

    private void Settings_ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
      dynamic defs = R.Default();
      R.Set("DataPath", Settings_DataPath.Text != "" ? Settings_DataPath.Text : defs.DataPath);
      R.Set("FileName", Settings_FileName.Text != "" ? Settings_FileName.Text : defs.FileName);
      R.Set("LogoutTimeout", Settings_LogoutTimeout.Text != "" ? System.Convert.ToInt32(Settings_LogoutTimeout.Text) : defs.LogoutTimeout);
      R.Set("BackupPath", Settings_BackupPath.Text != "" ? Settings_BackupPath.Text : defs.BackupPath);
      R.Set("BackupInterval", Settings_BackupInterval.Text != "" ? System.Convert.ToInt32(Settings_BackupInterval.Text) : defs.BackupInterval);
      R.Set("Password", Crypto.Encrypt(Settings_PasswordBox.Password, CryptoMode.Password));
      R.Set("AutoRunWindowMode", Settings_AutoRunMode.Content.ToString());
      Close();
    }

    private void Settings_OpenPath_Click(object sender, RoutedEventArgs e)
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
        if (sender == Settings_OpenBackupPath)
        {
          Settings_BackupPath.Text = folder;
        }
        if (sender == Settings_OpenDataPath)
        {
          Settings_DataPath.Text = folder;
        }
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
        string newSecureWord = UsefulFunctions.GetUniqueKey(32);
        R.Set("SecureWord", newSecureWord);
        Vault.RewriteFile();
      }
      if (sender == Settings_EraseButton)
      {
        Vault.EraseData();
      }
      if (sender == Settings_OpenDir)
      {
        System.Diagnostics.Process.Start(R.Get<string>("DataPath"));
      }
      if (sender == Settings_UnplannedBackup)
      {
        Vault.UnplannedBackup();
      }
      if (sender == Settings_AutoRunMode)
      {
        Settings_AutoRunMode.Content = Settings_AutoRunMode.Content.ToString() == WindowState.Minimized.ToString() ? WindowState.Normal.ToString() : WindowState.Minimized.ToString();
      }
    }

    private void Settings_CancelButton_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      _delegate(true);
    }

  }
}

