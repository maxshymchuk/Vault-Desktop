using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Text;

namespace VaultProject
{
  public partial class Vault : Window
  {
    private const int ID_LENGTH = 16;

    public ObservableCollection<Record> recordList { get; set; }
    public ObservableCollection<Record> checkedList { get; set; }

    private Record editableRecord = null;
    private object regKey = null;
    private SFile file;

    public Vault()
    {
      InitializeComponent();
      Loaded += Vault_Loaded;
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
      file.name = "data";
      F.CreateDir(file.path = $"{Environment.ExpandEnvironmentVariables("%appdata%")}\\Vault");
      file.reader = F.OpenFile(file.path, file.name, FMode.Read);
      recordList = new ObservableCollection<Record>();
      while (!file.reader.EndOfStream)
      {
        string line = Crypto.Decrypt(file.reader.ReadLine());
        var (id, note, pass) = (
          line.Substring(0, ID_LENGTH), 
          line.Substring(ID_LENGTH + 1, line.IndexOf('\t') - ID_LENGTH - 1), 
          line.Substring(line.IndexOf('\t') + 1)
        );
        recordList.Add(new Record() { Id = id, Note = note, Pass = pass });
      }
      file.reader.Close();
      file.writer = F.OpenFile(file.path, file.name, FMode.Write);
      file.writer.AutoFlush = true;
      checkedList = new ObservableCollection<Record>();
      listbox.ItemsSource = recordList;
      UpdateStatus();
    }

    private void RewriteFile()
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
      if (sender == DeleteButton && checkedList != null)
      {
        foreach (Record rec in checkedList)
        {
          recordList.Remove(rec);
        }
        checkedList = new ObservableCollection<Record>();
        RewriteFile();
      }
      if (sender == SyncButton)
      {
        //var wc = new WebClient();
        //wc.DownloadFile("https://b5wliw.db.files.1drv.com/y4mhh9ejh9k50uGN-Dnrd7AFP9fMfavttXYklwf5x6rQxg5p6ceQU1ZAC_WyNLNAsp248lMIf1ODeMJVVvG07X40ZNbhhLAKeINUTYrhpgZSyzomGKyIBfzeG9I0_1w-mJXi9PWTXksYwIQwsjgZmE-gW1yXPot61BSPw4nygG1IWY5W5zfExzAXXZZN8RsJOzS/list.txt?download&psid=1", "text.txt");
      }
      if (sender == SelectButton && recordList != null)
      {
        foreach (Record rec in recordList)
        {
          checkedList.Add(rec);
          rec.IsChecked = true;
        }
      }
      if (sender == UnselectButton)
      {
        checkedList = new ObservableCollection<Record>();
        foreach (Record rec in recordList)
        {
          rec.IsChecked = false;
        }
      }
      if (sender == LogoutButton)
      {
        (new Login()).Show();
        Close();
      }
      if (sender == SettingsButton)
      {
        // nothing
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
      file.writer.Close();
    }
  }
}
