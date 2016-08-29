using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ResotelApp.Utils
{
    class SecureStringUtil
    {
        public static string Read(SecureString secureString)
        {
            if(secureString == null)
            {
                return null;
            }
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }


    }
}
