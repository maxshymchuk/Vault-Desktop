using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Threading;

namespace VaultProject
{
  public partial class Vault : Window
  {
    private const int ID_LENGTH = 16;

    public static ObservableCollection<Record> recordList { get; set; }
    public ObservableCollection<Record> checkedList { get; set; }

    private Record editableRecord = null;
    public static SFile file;

    private Settings settings = null;
    private Login login = null;
    private Timer logoutTimer = null;
    private Timer backupTimer = null;

    public Vault()
    {
      InitializeComponent();
      setEnvVar();
      Loaded += Vault_Loaded;
      R.Init();
      login = new Login(new App.D_IsLogin(OnLogin));
    }

    private void setEnvVar()
    {
      const string name = "Path";
      string location = Environment.CurrentDirectory;
      string pathvar = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
      if (!pathvar.Split(';').Contains(location))
      {
        string value = pathvar + $";{location}";
        Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);
      }
    }

    private void OnLogin(LoginCallback callback)
    {
      switch (callback)
      {
        case LoginCallback.Passed:
          Show();
          Visibility = Visibility.Visible;
          break;
        case LoginCallback.NotPassed:
          Hide();
          Visibility = Visibility.Hidden;
          break;
        case LoginCallback.Shutdown:
          Close();
          break;
      }
      login = null;
    }

    private void OnSetupOver(bool isOver = false)
    {
      if (isOver)
      {
        settings = null;
        IsEnabled = true;
      }
    }

    private void EditControl_Click(object sender, RoutedEventArgs e)
    {
      if (sender == EditControl_SaveButton)
      {
        Record item = recordList.Single(i => i.Id == editableRecord.Id);
        item.Note = NoteText.Text;
        item.Pass = PassText.Password;
        RewriteFile();
      }
      EditControl_SaveButton.Visibility = Visibility.Hidden;
      EditControl_CancelButton.Visibility = Visibility.Hidden;
      ButtonRow1.IsEnabled = true;
      ButtonRow2.IsEnabled = true;
      ButtonRow3.IsEnabled = true;
      listbox.IsEnabled = true;
      editableRecord = null;
      NoteText.Text = "";
      PassText.Password = "";
    }

    private void UpdateStatus()
    {
      StatusLabel.Content = "Checked " + checkedList.ToArray().Length.ToString() + "/" + recordList.Count.ToString();
    }

    private void Vault_Loaded(object sender, RoutedEventArgs e)
    {
      // FILE STRUCTURE
      // id-of-element:note\tpass
      file.name = R.Get<string>("FileName");
      F.CreateDir(file.path = R.Get<string>("DataPath"));
      if (file.writer != null) file.writer.Close();
      file.reader = F.OpenFile(file.path, file.name, FMode.Read);
      recordList = new ObservableCollection<Record>();
      while (!file.reader.EndOfStream)
      {
        string decryptResult = Crypto.Decrypt(file.reader.ReadLine());
        if (decryptResult != "")
        {
          var (id, note, pass) = (
            decryptResult.Substring(0, ID_LENGTH),
            decryptResult.Substring(ID_LENGTH + 1, decryptResult.IndexOf('\t') - ID_LENGTH - 1),
            decryptResult.Substring(decryptResult.IndexOf('\t') + 1)
          );
          recordList.Add(new Record() { Id = id, Note = note, Pass = pass });
        }
        else
        {
          break;
        }
      }
      file.reader.Close();
      file.writer = F.OpenFile(file.path, file.name, FMode.Write);
      file.writer.AutoFlush = true;
      checkedList = new ObservableCollection<Record>();
      listbox.ItemsSource = recordList;
      UpdateStatus();
      backupTimer = new Timer()
      {
        Enabled = true,
        Interval = 10,
        AutoReset = false
      };
      backupTimer.Elapsed += Backup;
    }

    public static void RewriteFile()
    {
      file.writer.Close();
      file.writer = F.OpenFile(file.path, file.name, FMode.Write, FWriteMode.Rewrite);
      foreach (Record rec in recordList)
      {
        file.writer.WriteLine(Crypto.Encrypt($"{rec.Id}:{rec.Note}\t{rec.Pass}"));
      }
      file.writer.Close();
      file.writer = F.OpenFile(file.path, file.name, FMode.Write, FWriteMode.Append);
    }

    public static void EraseData()
    {
      recordList.Clear();
      file.writer.Close();
      file.writer = F.OpenFile(file.path, file.name, FMode.Write, FWriteMode.Rewrite);
      file.writer.Close();
      file.writer = F.OpenFile(file.path, file.name, FMode.Write, FWriteMode.Append);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (sender == AddButton)
      {
        if (NoteText.Text != "" && PassText.Password != "")
        {
          var (id, note, pass) = (UsefulFunctions.GetUniqueKey(ID_LENGTH), NoteText.Text, PassText.Password);
          recordList.Add(new Record() { Id = id, Note = note, Pass = pass });

          file.writer.WriteLine(Crypto.Encrypt($"{id}:{note}\t{pass}"));

          NoteText.Text = "";
          PassText.Password = "";
        }
      }
      else if (sender == DeleteButton && checkedList != null)
      {
        foreach (Record rec in checkedList)
        {
          recordList.Remove(rec);
        }
        checkedList = new ObservableCollection<Record>();
        RewriteFile();
      }
      else if (sender == SyncButton)
      {
        //var wc = new WebClient();
        //wc.DownloadFile("https://b5wliw.db.files.1drv.com/y4mhh9ejh9k50uGN-Dnrd7AFP9fMfavttXYklwf5x6rQxg5p6ceQU1ZAC_WyNLNAsp248lMIf1ODeMJVVvG07X40ZNbhhLAKeINUTYrhpgZSyzomGKyIBfzeG9I0_1w-mJXi9PWTXksYwIQwsjgZmE-gW1yXPot61BSPw4nygG1IWY5W5zfExzAXXZZN8RsJOzS/list.txt?download&psid=1", "text.txt");
      }
      else if (sender == SelectButton && recordList != null)
      {
        foreach (Record rec in recordList)
        {
          checkedList.Add(rec);
          rec.IsChecked = true;
        }
      }
      else if (sender == UnselectButton)
      {
        checkedList = new ObservableCollection<Record>();
        foreach (Record rec in recordList)
        {
          rec.IsChecked = false;
        }
      }
      else if (sender == LogoutButton)
      {
        login = new Login(new App.D_IsLogin(OnLogin));
      }
      else if (sender == SettingsButton)
      {
        if (settings == null)
        {
          IsEnabled = false;
          settings = new Settings(new App.D_IsSetupOver(OnSetupOver));
          settings.Show();
        }
      }
      UpdateStatus();
    }

    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
      string id = (sender as CheckBox).Tag.ToString();
      Record item = recordList.Single(i => i.Id == id);

      if ((sender as CheckBox).IsChecked.Value)
      {
        checkedList.Add(item);
      }
      else
      {
        checkedList.Remove(item);
      }

      UpdateStatus();
    }

    private void RecordEdit_Click(object sender, RoutedEventArgs e)
    {
      string id = (sender as Button).Tag.ToString();
      Record item = recordList.Single(i => i.Id == id);

      EditControl_SaveButton.Visibility = Visibility.Visible;
      EditControl_CancelButton.Visibility = Visibility.Visible;
      ButtonRow1.IsEnabled = false;
      ButtonRow2.IsEnabled = false;
      ButtonRow3.IsEnabled = false;
      listbox.IsEnabled = false;
      NoteText.Text = item.Note;
      PassText.Password = item.Pass;

      editableRecord = item;
    }

    private void RecordCopy_Click(object sender, RoutedEventArgs e)
    {
      string id = (sender as Button).Tag.ToString();
      Record item = recordList.Single(i => i.Id == id);

      Clipboard.SetText(item.Pass);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (file.reader != null) file.reader.Close();
      if (file.writer != null) file.writer.Close();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      string word = (sender as TextBox).Text.ToLower();
      IEnumerable<Record> searchList = null;
      searchList = recordList.Where(rec => rec.Note.ToLower().Contains(word));
      if (word != "")
      {
        listbox.ItemsSource = searchList;
      }
      else
      {
        listbox.ItemsSource = recordList;
      }
    }

    public static void UnplannedBackup()
    {
      System.IO.File.Copy(
        $"{R.Get<string>("DataPath")}\\{R.Get<string>("FileName")}",
        $"{R.Get<string>("BackupPath")}\\{R.Get<string>("FileName")}_backup_{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}_{R.Get<string>("SecureWord")}"
      );
    }
    private void Backup(object sender, ElapsedEventArgs e)
    {
      (sender as Timer).Interval = 1 * 3600000; // 1 hour
      DateTime lastBackupTime = Convert.ToDateTime(R.Get<string>("LastBackup"));
      DateTime currentTime = DateTime.Now;
      if ((currentTime - lastBackupTime).TotalDays >= R.Get<int>("BackupInterval"))
      {
        UnplannedBackup();
        R.Set("LastBackup", DateTime.Now.ToString());
      }
    }
    private void StopLogoutTimer()
    {
      logoutTimer?.Stop();
      logoutTimer = null;
    }
    private void StartLogoutTimer()
    {
      const uint coeff = 60000; // minutes to milliseconds
      StopLogoutTimer();
      logoutTimer = new Timer()
      {
        Enabled = true,
        Interval = R.Get<int>("LogoutTimeout") * coeff,
        AutoReset = false
      };
      logoutTimer.Elapsed += OnTimedEvent;
    }

    private void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
      Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => {
        if (login == null) login = new Login(new App.D_IsLogin(OnLogin));
      }));
    }

    private void SearchBox_ClearButton_Click(object sender, RoutedEventArgs e)
    {
      SearchBox.Text = "";
    }

    private void Window_Activated(object sender, EventArgs e)
    {
      StopLogoutTimer();
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
      StartLogoutTimer();
    }
  }
}
