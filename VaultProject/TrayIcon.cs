using System.Windows;
using System.Windows.Forms;

namespace Vault
{
  class TrayIcon
  {
    private static NotifyIcon trayIcon = null;
    private static Window _target = null;

    public static object target {
      get
      {
        return _target;
      }
    } 
    public static void Init()
    {
      trayIcon = new NotifyIcon()
      {
        Icon = Properties.Resources.icon,
        Text = "Vault",
        Visible = false
      };
      trayIcon.MouseClick += (object sender, System.Windows.Forms.MouseEventArgs e) =>
      {
        _target.Show();
        _target.WindowState = WindowState.Normal;
        _target.Activate();
        Hide();
      };
    }

    public static void Show(object target)
    {
      _target = target as Window;
      trayIcon.Visible = true;
    }

    public static void Hide()
    {
      trayIcon.Visible = false;
    }
  }
}
