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
        key = Registry.CurrentUser.CreateSubKey(@"Software\Vault");
        Set("DataPath", $"{Environment.ExpandEnvironmentVariables("%appdata%")}\\Vault");
        Set("FileName", "data");
        Set("Password", "pass");
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

    public static string Get(string name)
    {
      return key.GetValue(name).ToString();
    }
  }
}
