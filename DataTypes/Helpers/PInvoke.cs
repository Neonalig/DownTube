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

namespace DownTube.DataTypes;

/// <summary>
/// PInvoke methods. (<see href="https://www.pinvoke.net/index.aspx/"/>)
/// </summary>
public static class PInvoke {


    /// <summary>
    /// Unlocks a region in the specified file. This function can operate either synchronously or asynchronously.
    /// </summary>
    /// <remarks>Unlocking a region of a file releases a previously acquired lock on the file. The region to unlock must correspond exactly to an existing locked region. Two adjacent regions of a file cannot be locked separately and then unlocked using a single region that spans both locked regions.
    /// <para/>
    /// Locks are released before the <see href="https://docs.microsoft.com/en-us/windows/desktop/api/handleapi/nf-handleapi-closehandle">CloseHandle</see> function is finished processing.
    /// <para/>
    /// In Windows 8 and Windows Server 2012, this function is supported by the following technologies.</remarks>
    /// <param name="HFile"> A handle to the file that contains a region locked with LockFile. The file handle must have been created with either the GENERIC_READ or GENERIC_WRITE access right. For more information, see <see href="https://docs.microsoft.com/en-us/windows/desktop/FileIO/file-security-and-access-rights">File Security and Access Rights</see>.</param>
    /// <param name="DWReserved">Reserved parameter; must be zero.</param>
    /// <param name="NNumberOfBytesToUnlockLow">The low-order word of the length of the byte range to be unlocked.</param>
    /// <param name="NNumberOfBytesToUnlockHigh">The high-order word of the length of the byte range to be unlocked.</param>
    /// <param name="LPOverlapped">A pointer to an <see href="https://docs.microsoft.com/en-us/windows/desktop/api/minwinbase/ns-minwinbase-overlapped">OVERLAPPED</see> structure that the function uses with the unlocking request. This structure contains the file offset of the beginning of the unlock range. You must initialize the hEvent member to a valid handle or zero. For more information, see <see href="https://docs.microsoft.com/en-us/windows/desktop/FileIO/synchronous-and-asynchronous-i-o">Synchronous and Asynchronous I/O</see>.</param>
    /// <returns>If the function succeeds, the return value is nonzero.
    /// <para/>If the function fails, the return value is zero or <see langword="null"/>. To get extended error information, call <see cref="Marshal.GetLastPInvokeError"/>.</returns>
    [DllImport("kernel32.dll")]
    static extern bool UnlockFileEx( IntPtr HFile, uint DWReserved,
        uint NNumberOfBytesToUnlockLow, uint NNumberOfBytesToUnlockHigh,
        [In] ref NativeOverlapped LPOverlapped );

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

    /// <summary>
    /// Gets the current path to the specified known folder as currently configured. This does not require the folder to be existent.
    /// </summary>
    /// <param name="KnownFolder">The known folder which current path will be returned.</param>
    /// <returns>The default path of the known folder.</returns>
    /// <exception cref="ExternalException">Thrown if the path could not be retrieved.</exception>
    public static string? GetPath( KnownFolder KnownFolder ) => GetPath(KnownFolder, false);

    /// <summary>
    /// Gets the current path to the specified known folder as currently configured. This does not require the folder to be existent.
    /// </summary>
    /// <param name="KnownFolder">The known folder which current path will be returned.</param>
    /// <param name="DefaultUser">Specifies if the paths of the default user (user profile template) will be used. This requires administrative rights.</param>
    /// <returns>The default path of the known folder.</returns>
    /// <exception cref="ExternalException">Thrown if the path could not be retrieved.</exception>
    public static string? GetPath( KnownFolder KnownFolder, bool DefaultUser ) => GetPath(KnownFolder, (uint)KnownFolderFlags.DontVerify, DefaultUser);

    static string? GetPath( KnownFolder KnownFolder, uint Flags, bool DefaultUser ) {
        int Result = SHGetKnownFolderPath(KnownFolder.Guid, Flags, new IntPtr(DefaultUser ? -1 : 0), out IntPtr OutPath);
        switch ( Result ) {
            case >= 0:
                string? PATH = Marshal.PtrToStringUni(OutPath);
                Marshal.FreeCoTaskMem(OutPath);
                return PATH;
            default:
                throw new ExternalException("Unable to retrieve the known folder path. It may not be available on this system.", Result);
        }
    }

    [DllImport("Shell32.dll")]
    static extern int SHGetKnownFolderPath(
        [MarshalAs(UnmanagedType.LPStruct)] Guid Rfid, uint DWFlags, IntPtr HToken,
        out IntPtr PpszPath );

    [Flags]
    enum KnownFolderFlags : uint {
        SimpleIDList              = 0x00000100,
        NotParentRelative         = 0x00000200,
        DefaultPath               = 0x00000400,
        Init                      = 0x00000800,
        NoAlias                   = 0x00001000,
        DontUnexpand              = 0x00002000,
        DontVerify                = 0x00004000,
        Create                    = 0x00008000,
        NoAppcontainerRedirection = 0x00010000,
        AliasOnly                 = 0x80000000
    }
}

public readonly struct KnownFolder {
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    /// <value>
    /// The unique identifier.
    /// </value>
    public Guid Guid { get; }

    /// <summary>
    /// Gets the path.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    public string? Path { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="KnownFolder"/> struct.
    /// </summary>
    /// <param name="Guid">The unique identifier.</param>
    public KnownFolder( string Guid ) : this (new Guid(Guid)) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="KnownFolder"/> struct.
    /// </summary>
    /// <param name="Guid">The unique identifier.</param>
    public KnownFolder( Guid Guid ) {
        this.Guid = Guid;
        Path = null;

        Path = PInvoke.GetPath(this);
    }

    /// <summary> The contacts library. </summary>
    public static readonly KnownFolder Contacts = new KnownFolder("{56784854-C6CB-462B-8169-88E350ACB882}");

    /// <summary> The desktop directory. </summary>
    public static readonly KnownFolder Desktop = new KnownFolder("{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}");

    /// <summary> The documents library. </summary>
    public static readonly KnownFolder Documents = new KnownFolder("{FDD39AD0-238F-46AF-ADB4-6C85480369C7}");

    /// <summary> The downloads library. </summary>
    public static readonly KnownFolder Downloads = new KnownFolder("{374DE290-123F-4565-9164-39C4925E467B}");

    /// <summary> The favourites library. </summary>
    public static readonly KnownFolder Favourites = new KnownFolder("{1777F761-68AD-4D8A-87BD-30B759FA33DD}");

    /// <summary> The links library. </summary>
    public static readonly KnownFolder Links = new KnownFolder("{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}");

    /// <summary> The music library. </summary>
    public static readonly KnownFolder Music = new KnownFolder("{4BD8D571-6D19-48D3-BE97-422220080E43}");

    /// <summary> The pictures library. </summary>
    public static readonly KnownFolder Pictures = new KnownFolder("{33E28130-4E1E-4676-835A-98395C3BC3BB}");

    /// <summary> The saved games library. </summary>
    public static readonly KnownFolder SavedGames = new KnownFolder("{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}");

    /// <summary> The saved searches library. </summary>
    public static readonly KnownFolder SavedSearches = new KnownFolder("{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}");

    /// <summary> The videos library. </summary>
    public static readonly KnownFolder Videos = new KnownFolder("{18989B1D-99B5-455B-841C-AB7C74E4DDFC}");
}