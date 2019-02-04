using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaultProject
{
  public class Record : INotifyPropertyChanged
  {
    private string id;
    private string note;
    private string pass;
    private bool isChecked;

    public string Id
    {
      get { return id; }
      set
      {
        id = value;
        NotifyPropertyChanged();
      }
    }

    public string Note
    {
      get { return note; }
      set
      {
        note = value;
        NotifyPropertyChanged();
      }
    }

    public string Pass
    {
      get { return pass; }
      set
      {
        pass = value;
        NotifyPropertyChanged();
      }
    }

    public bool IsChecked
    {
      get { return isChecked; }
      set
      {
        isChecked = value;
        NotifyPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
