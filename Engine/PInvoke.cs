#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32.SafeHandles;

#endregion

namespace DownTube.Engine;

/// <summary>
/// PInvoke methods. (<see href="https://www.pinvoke.net/index.aspx/"/>)
/// </summary>
public static class PInvoke {
    /// <summary>
    /// Gets the final path name by a given handle. (<see href="https://www.pinvoke.net/default.aspx/shell32/GetFinalPathNameByHandle.html"/>)
    /// </summary>
    /// <param name="HFile">The h file.</param>
    /// <param name="LpszFilePath">The LPSZ file path.</param>
    /// <param name="CchFilePath">The CCH file path.</param>
    /// <param name="DWFlags">The DW flags.</param>
    /// <returns>The path name pointer.</returns>
    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern uint GetFinalPathNameByHandle( SafeFileHandle HFile,
        //[MarshalAs(UnmanagedType.LPTStr)]
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder LpszFilePath,
        uint CchFilePath, uint DWFlags );

    /// <summary>
    /// The 'file name normalised' property.
    /// </summary>
    const uint File_Name_Normalized = 0x0;

    /// <summary>
    /// Gets the final path name by a given handle.
    /// </summary>
    /// <param name="FileHandle">The file handle.</param>
    /// <returns>The final (resolved) path name.</returns>
    static string GetFinalPathNameByHandle( SafeFileHandle FileHandle ) {
        StringBuilder OutPath = new StringBuilder(1024);

        uint Size = GetFinalPathNameByHandle(FileHandle, OutPath, (uint)OutPath.Capacity, File_Name_Normalized);
        if ( Size == 0 || Size > OutPath.Capacity ) {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        // may be prefixed with \\?\, which we don't want
        if ( OutPath[0] == '\\' && OutPath[1] == '\\' && OutPath[2] == '?' && OutPath[3] == '\\' ) {
            return OutPath.ToString(4, OutPath.Length - 4);
        }

        return OutPath.ToString();
    }

    /// <summary>
    /// <see href="https://www.pinvoke.net/default.aspx/kernel32.createfile"/>
    /// </summary>
    /// <param name="Filename">The filename.</param>
    /// <param name="Access">The access.</param>
    /// <param name="Share">The share.</param>
    /// <param name="SecurityAttributes">The security attributes.</param>
    /// <param name="CreationDisposition">The creation disposition.</param>
    /// <param name="FlagsAndAttributes">The flags and attributes.</param>
    /// <param name="TemplateFile">The template file.</param>
    /// <returns>The <see cref="SafeFileHandle"/>.</returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern SafeFileHandle CreateFile(
        //[MarshalAs(UnmanagedType.LPTStr)]
        [MarshalAs(UnmanagedType.LPWStr)] string Filename,
        [MarshalAs(UnmanagedType.U4)] FileAccess Access,
        [MarshalAs(UnmanagedType.U4)] FileShare Share,
        IntPtr SecurityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
        [MarshalAs(UnmanagedType.U4)] FileMode CreationDisposition,
        [MarshalAs(UnmanagedType.U4)] FileAttributes FlagsAndAttributes,
        IntPtr TemplateFile );

    /// <summary>
    /// The relevant flag ('BackupSemantics')
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    const FileAttributes FILE_FLAG_BACKUP_SEMANTICS = (FileAttributes)0x02000000u;

    /// <summary>
    /// Gets the final name of the path, resolving capitalisation and symbolic links.
    /// </summary>
    /// <param name="DirtyPath">The dirty path.</param>
    /// <returns>The final, fully-resolved path.</returns>
    /// <exception cref="Win32Exception">The last exception as found via <see cref="Marshal.GetLastWin32Error()"/>.</exception>
    public static string GetFinalPathName( string DirtyPath ) {
        // use 0 for access so we can avoid error on our metadata-only query (see dwDesiredAccess docs on CreateFile)
        // use FILE_FLAG_BACKUP_SEMANTICS for attributes so we can operate on directories (see Directories in remarks section for CreateFile docs)

        using ( SafeFileHandle DirectoryHandle = CreateFile(
                   DirtyPath,
                   0,
                   FileShare.ReadWrite | FileShare.Delete,
                   IntPtr.Zero,
                   FileMode.Open,
                   FILE_FLAG_BACKUP_SEMANTICS,
                   IntPtr.Zero) ) {
            if ( DirectoryHandle.IsInvalid ) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return GetFinalPathNameByHandle(DirectoryHandle);
        }
    }
}