using System;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;

namespace VaultProject
{
  public static class R
  {
    public static RegistryKey key = null;
    private static RegistryKey autorun = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

    public static void Init()
    {
      key = Registry.CurrentUser.OpenSubKey(@"Software\Vault", true);

      if (key == null)
      {
        dynamic defs = Default();
        key = Registry.CurrentUser.CreateSubKey(@"Software\Vault");
        Set("DataPath", defs.DataPath);
        Set("FileName", defs.FileName);
        Set("LogoutTimeout", defs.LogoutTimeout);
        Set("BackupPath", defs.BackupPath);
        Set("BackupInterval", defs.BackupInterval);
        Set("LastBackup", DateTime.Now.ToString());
        Set("Password", Crypto.Encrypt("pass", CryptoMode.Password));
        Set("SecureWord", UsefulFunctions.GetUniqueKey(32));
      }
    }

    public static void SetAutoRun()
    {
      autorun.SetValue("Vault", Assembly.GetExecutingAssembly().Location);
    }

    public static void UnsetAutoRun()
    {
      autorun.DeleteValue("Vault");
    }

    public static bool IsAutoRunSet()
    {
      return autorun.GetValue("Vault") != null;
    }

    public static void Set(string name, string value)
    {
      key.SetValue(name, value, RegistryValueKind.String);
    }

    public static void Set(string name, int value)
    {
      key.SetValue(name, value, RegistryValueKind.DWord);
    }

    public static T Get<T>(string name)
    {
      return (T)key.GetValue(name);
    }

    public static object Default()
    {
      return new {
        DataPath = $"{Environment.ExpandEnvironmentVariables("%appdata%")}\\Vault",
        FileName = "data",
        LogoutTimeout = 5,
        BackupPath = $"{Environment.ExpandEnvironmentVariables("%HOMEPATH%")}",
        BackupInterval = 7
      };
    }

  }
}
