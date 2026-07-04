using Lumina.XML;
using System;
using System.Runtime.InteropServices;

public static class XMLFileDialog
{
    [ComImport]
    [Guid("d57c7288-d4ad-4768-be02-9d969532d960")] // Correct IID_IFileOpenDialog
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileOpenDialog
    {
        [PreserveSig] int Show(IntPtr parent);

        void SetFileTypes(uint cFileTypes,
            [MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);

        void SetFileTypeIndex(uint iFileType);
        void GetFileTypeIndex(out uint piFileType);

        void Advise(IntPtr pfde, out uint pdwCookie);
        void Unadvise(uint dwCookie);

        void SetOptions(uint fos);
        void GetOptions(out uint pfos);

        void SetDefaultFolder(IShellItem psi);
        void SetFolder(IShellItem psi);
        void GetFolder(out IShellItem ppsi);
        void GetCurrentSelection(out IShellItem ppsi);

        void SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
        void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
        void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        void GetResult(out IShellItem ppsi);

        void AddPlace(IShellItem psi, uint fdap);

        void SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        void Close(int hr);

        void SetClientGuid(ref Guid guid);
        void ClearClientData();

        void SetFilter(IntPtr pFilter);
    }

    [ComImport]
    [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")] // Correct IShellItem GUID
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItem
    {
        void BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv);
        void GetParent(out IShellItem ppsi);

        void GetDisplayName(
            SIGDN sigdnName,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
        void Compare(IShellItem psi, uint hint, out int piOrder);
    }

    [DllImport("ole32.dll")]
    private static extern int CoCreateInstance(
        [In] ref Guid rclsid,
        IntPtr pUnkOuter,
        uint dwClsContext,
        [In] ref Guid riid,
        out IntPtr ppv);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
    private static extern void SHCreateItemFromParsingName(
        [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
        IntPtr pbc,
        [In] ref Guid riid,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

    private enum SIGDN : uint
    {
        SIGDN_FILESYSPATH = 0x80058000,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct COMDLG_FILTERSPEC
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszSpec;
    }

    private const uint CLSCTX_INPROC_SERVER = 1;

    public static string ShowPresetDialog(
        string title = "Select a Lumina Preset File",
        string filterName = "Lumina Preset Files",
        string filterSpec = "*.xml")
    {
        // Correct CLSID/IID
        Guid clsid = new Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7");
        Guid iid = new Guid("d57c7288-d4ad-4768-be02-9d969532d960");

        IntPtr ppv;
        int hr = CoCreateInstance(ref clsid, IntPtr.Zero, CLSCTX_INPROC_SERVER, ref iid, out ppv);

        if (hr != 0)
            Marshal.ThrowExceptionForHR(hr);

        var dialog = (IFileOpenDialog)Marshal.GetObjectForIUnknown(ppv);

        // Give this dialog its own persistent state
        Guid clientGuid = new Guid("A8C4D1E2-5A5A-4D1F-9C60-8E6D8C2A9B11");
        dialog.SetClientGuid(ref clientGuid);

        var filters = new[]
        {
            new COMDLG_FILTERSPEC
            {
                pszName = filterName,
                pszSpec = filterSpec
            }
        };

        dialog.SetFileTypes((uint)filters.Length, filters);
        dialog.SetFileTypeIndex(1);
        dialog.SetTitle(title);
        dialog.SetDefaultExtension("xml");

        try
        {
            string defaultPath = GlobalPaths.LuminaPresetsDirectory;

            Guid shellItemGuid = typeof(IShellItem).GUID;

            SHCreateItemFromParsingName(
                defaultPath,
                IntPtr.Zero,
                ref shellItemGuid,
                out IShellItem folder);

            // Force this folder every time
            dialog.SetFolder(folder);
            dialog.SetDefaultFolder(folder);

            Marshal.ReleaseComObject(folder);
        }
        catch
        {
            // Ignore if the folder doesn't exist
        }

        hr = dialog.Show(IntPtr.Zero);

        if (hr != 0)
        {
            Marshal.ReleaseComObject(dialog);
            return null;
        }

        dialog.GetResult(out IShellItem item);
        item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out string filePath);

        Marshal.ReleaseComObject(item);
        Marshal.ReleaseComObject(dialog);

        return filePath;
    }
}