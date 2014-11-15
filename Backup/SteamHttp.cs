// Decompiled with JetBrains decompiler
// Type: SteamHttp
// Assembly: img_012, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 502588B3-DCFF-462A-9C8C-46E9EB112CFD
// Assembly location: C:\Users\Scars\Desktop\img_012.exe

using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

public class SteamHttp
{
  public static bool ObtainsessionID(CookieContainer cookie)
  {
    HttpWebResponse httpWebResponse = (HttpWebResponse) null;
    StreamReader streamReader = (StreamReader) null;
    try
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create("http://steamcommunity.com");
      httpWebRequest.Method = "GET";
      ((NameValueCollection) httpWebRequest.Headers)["Origin"] = "http://steamcommunity.com";
      httpWebRequest.Referer = "http://steamcommunity.com";
      ((NameValueCollection) httpWebRequest.Headers)["Accept-Encoding"] = "gzip,deflate";
      ((NameValueCollection) httpWebRequest.Headers)["Accept-Language"] = "en-us,en";
      ((NameValueCollection) httpWebRequest.Headers)["Accept-Charset"] = "iso-8859-1,*,utf-8";
      httpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.3; en-US; Valve Steam Client/1393366296; ) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Safari/535.19";
      httpWebRequest.CookieContainer = cookie;
      httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
      httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
      httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse();
      if (httpWebResponse.StatusCode == HttpStatusCode.OK)
      {
        streamReader = new StreamReader(httpWebResponse.GetResponseStream());
        streamReader.ReadToEnd();
        return !streamReader.ReadToEnd().Contains("g_steamID = false;") && httpWebResponse.Cookies["sessionid"] != null;
      }
    }
    catch (Exception ex)
    {
    }
    finally
    {
      if (streamReader != null)
        streamReader.Close();
      if (httpWebResponse != null)
        httpWebResponse.Close();
    }
    return false;
  }

  public static string SteamWebRequest(CookieContainer cookie, string url, string data = null, string lasturl = "")
  {
    HttpWebResponse httpWebResponse = (HttpWebResponse) null;
    StreamReader streamReader = (StreamReader) null;
    HttpWebRequest httpWebRequest1 = (HttpWebRequest) null;
    try
    {
      httpWebRequest1 = (HttpWebRequest) null;
      HttpWebRequest httpWebRequest2 = !url.StartsWith("http") ? (HttpWebRequest) WebRequest.Create("http" + (data != null ? "s" : "") + "://steamcommunity.com/" + url) : (HttpWebRequest) WebRequest.Create(url);
      httpWebRequest2.Referer = "http://steamcommunity.com/" + lasturl;
      httpWebRequest2.Accept = data != null ? "*/*" : "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
      httpWebRequest2.Method = data != null ? "POST" : "GET";
      if (data != null)
        httpWebRequest2.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
      ((NameValueCollection) httpWebRequest2.Headers)["Accept-Charset"] = "iso-8859-1,*,utf-8";
      ((NameValueCollection) httpWebRequest2.Headers)["Origin"] = "https://steamcommunity.com";
      httpWebRequest2.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.3; en-US; Valve Steam Client/1393366296; ) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Safari/535.19";
      ((NameValueCollection) httpWebRequest2.Headers)["Accept-Language"] = "en-us,en";
      ((NameValueCollection) httpWebRequest2.Headers)["Accept-Encoding"] = "gzip,deflate";
      httpWebRequest2.CookieContainer = cookie;
      httpWebRequest2.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
      if (data != null)
      {
        using (StreamWriter streamWriter = new StreamWriter(((WebRequest) httpWebRequest2).GetRequestStream()))
          streamWriter.Write(data);
      }
      httpWebResponse = (HttpWebResponse) httpWebRequest2.GetResponse();
      if (httpWebResponse.StatusCode == HttpStatusCode.OK)
      {
        streamReader = new StreamReader(httpWebResponse.GetResponseStream());
        return streamReader.ReadToEnd();
      }
    }
    catch (WebException ex)
    {
      if (ex.Response != null)
        return new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
    }
    finally
    {
      if (streamReader != null)
        streamReader.Close();
      if (httpWebResponse != null)
        httpWebResponse.Close();
    }
    return (string) null;
  }
}
