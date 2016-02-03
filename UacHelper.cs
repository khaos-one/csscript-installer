using System;
using System.Security.Principal;

namespace CSScriptInstaller {
    public static class UacHelper {
        public static bool IsInRole(WindowsBuiltInRole role) {
            try {
                var user = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(user);
                return principal.IsInRole(role);
            }
            catch (Exception) {
                return false;
            }
        }
    }
}
