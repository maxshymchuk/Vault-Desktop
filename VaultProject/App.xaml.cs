using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VaultProject
{
  public partial class App : Application
  {
    public delegate void D_IsLogin(LoginCallback callback);
    public delegate void D_IsSetupOver(bool isOver);
  }
}
