// Decompiled with JetBrains decompiler
// Type: WinApis
// Assembly: img_012, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 502588B3-DCFF-462A-9C8C-46E9EB112CFD
// Assembly location: C:\Users\Scars\Desktop\img_012.exe

using System;
using System.Runtime.InteropServices;

public class WinApis
{
  [DllImport("kernel32.dll")]
  public static extern int VirtualQueryEx(IntPtr handle, IntPtr adress, out WinApis.PROCESS_QUERY_INFORMATION processQuery, uint length);

  [DllImport("kernel32.dll")]
  public static extern void GetSystemInfo(out WinApis.SYSTEM_INFO input);

  [DllImport("kernel32.dll")]
  public static extern IntPtr OpenProcess(uint ProcessAcces, bool bInheritHandle, int processId);

  [DllImport("kernel32.dll")]
  public static extern bool ReadProcessMemory(IntPtr handle, IntPtr adress, [Out] byte[] buffer, uint size, out IntPtr numberofbytesread);

  public struct PROCESS_QUERY_INFORMATION
  {
    public IntPtr BaseAdress;
    public IntPtr AllocationBase;
    public uint AllocationProtect;
    public uint RegionSize;
    public uint State;
    public uint Protect;
    public uint Type;
  }

  public struct SYSTEM_INFO
  {
    public ushort processorArchitecture;
    private ushort reserved;
    public uint pageSize;
    public IntPtr minimumApplicationAddress;
    public IntPtr maximumApplicationAddress;
    public IntPtr activeProcessorMask;
    public uint numberOfProcessors;
    public uint processorType;
    public uint allocationGranularity;
    public ushort processorLevel;
    public ushort processorRevision;
  }
}
