using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum FMode : byte
{
  Read, Write
}

public enum FWriteMode : byte
{
  Rewrite, Append
}

public struct SFile
{
  public StreamReader reader;
  public StreamWriter writer;
  public string path;
  public string name;
}

namespace VaultProject
{
  public static class F
  {
    public static void CreateDir(string path)
    {
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
    }
      
    public static void CreateFile(string path, string name)
    {
      FileInfo file = new FileInfo($"{path}\\{name}");
      if (!file.Exists)
      {
        file.Create();
      }
    }

    public static dynamic OpenFile(string path, string name, FMode mode, FWriteMode wMode = FWriteMode.Append)
    {
      FileInfo checkFile = new FileInfo($"{path}\\{name}");
      if (!checkFile.Exists)
      {
        checkFile.Create().Dispose();
      }
      switch (mode)
      {
        case FMode.Write:
          return new StreamWriter($"{path}\\{name}", wMode == FWriteMode.Append ? true : false, Encoding.Default);
        default:
          return new StreamReader($"{path}\\{name}", Encoding.Default);
      }
    }
  }
}
