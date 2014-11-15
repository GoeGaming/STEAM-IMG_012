// Decompiled with JetBrains decompiler
// Type: inventoryjson
// Assembly: img_012, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 502588B3-DCFF-462A-9C8C-46E9EB112CFD
// Assembly location: C:\Users\Scars\Desktop\img_012.exe

using System;
using System.Collections.Generic;

internal class inventoryjson
{
  private List<string> list1 = new List<string>();
  private List<string> list2 = new List<string>();
  private string _contextID;

  public List<string[]> Parse(string input, string contextid)
  {
    this._contextID = contextid;
    string[] input1 = input.Split(new char[2]
    {
      '{',
      '}'
    });
    if (input1.Length <= 2 || input1[1].IndexOf("true") == -1)
      return new List<string[]>();
    this.parserginventory(input1);
    this.parsergdescrptions(input1);
    return this.getItems();
  }

  private List<string[]> getItems()
  {
    List<string[]> list = new List<string[]>();
    for (int index1 = 0; index1 < this.list1.Count; ++index1)
    {
      string input1 = this.list1[index1];
      string str1 = this.getValue(input1, "id");
      string str2 = this.getValue(input1, "classid");
      string input2 = string.Empty;
      for (int index2 = 0; index2 < this.list2.Count; ++index2)
      {
        if (this.getValue(this.list2[index2], "classid") == str2)
        {
          input2 = this.list2[index2];
          break;
        }
      }
      if (input2 != string.Empty)
      {
        string[] strArray = new string[6]
        {
          this.getValue(input2, "appid"),
          this.getValue(input1, "amount"),
          str1,
          this.getValue(input2, "market_name"),
          this.getValue(input2, "type").ToLower(),
          this._contextID
        };
        if (!list.Contains(strArray) && this.getValue(input2, "tradable") == "1")
          list.Add(strArray);
      }
    }
    return list;
  }

  private string getValue(string input, string name)
  {
    string[] strArray = input.Split(new char[2]
    {
      '"',
      ':'
    }, StringSplitOptions.RemoveEmptyEntries);
    for (int index = 0; index < strArray.Length; ++index)
    {
      if (strArray[index] == name)
        return strArray[index + 1].Replace(",", "");
    }
    return (string) null;
  }

  private void parserginventory(string[] input)
  {
    int num1 = -1;
    int num2 = -1;
    for (int index = 0; index < input.Length; ++index)
    {
      if (input[index].IndexOf("rgInventory") != -1)
        num1 = index + 1;
      if (num1 != -1 && input[index] == "")
      {
        num2 = index;
        break;
      }
    }
    for (int index = num1; index < num2; ++index)
    {
      string[] strArray = input[index].Split('"');
      if (strArray.Length > 2 && strArray[1] == "id")
        this.list1.Add(input[index]);
    }
  }

  private void parsergdescrptions(string[] input)
  {
    int num = -1;
    for (int index = 0; index < input.Length; ++index)
    {
      if (input[index].IndexOf("rgDescriptions") != -1)
      {
        num = index + 1;
        break;
      }
    }
    for (int index = num; index < input.Length; ++index)
    {
      string[] strArray = input[index].Split('"');
      if (strArray.Length > 2 && strArray[1] == "appid")
        this.list2.Add(input[index]);
    }
  }
}
