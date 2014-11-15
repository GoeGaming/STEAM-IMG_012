// Decompiled with JetBrains decompiler
// Type: SteamStealer.Properties.Settings
// Assembly: img_012, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 502588B3-DCFF-462A-9C8C-46E9EB112CFD
// Assembly location: C:\Users\Scars\Desktop\img_012.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace SteamStealer.Properties
{
  [CompilerGenerated]
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        Settings settings = Settings.defaultInstance;
        return settings;
      }
    }
  }
}
