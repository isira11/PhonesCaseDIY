#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("EfwKecbUaX/8EgCX2m9E+KaCedGic8v69OV2TXkuG5NsHC7Rvgmz0L8Njq2/gomGpQnHCXiCjo6Oio+MDY6Aj78NjoWNDY6Oj0fbZR82bP9be/GCdPp6PXzFFnwQUD7FnWSI9LhLoqK12rPbZpKkQWb1SnMegx12A5qjvOFXD5ZtGRhEF5e/8DWE+YE/3jgE1kUWjG4LZ74MIaOEs3DDyZC6UP6+mRV8HYpamScP+jpSHhaOxDAYU129m1/SrIS+NOkoYbd1eihRrxtFYvTRbQCgtzi6aeZLp3+CxQq0Aoc7eQa701xJPSYQL7NAMIIKsrEy+dvs3y104sTcEsDmKldZEEZtpSFtJeYL6xaD6kJl9rAyOF2lUdhG3F193S91po2Mjo+O");
        private static int[] order = new int[] { 6,2,6,6,4,12,11,9,13,9,11,13,12,13,14 };
        private static int key = 143;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
