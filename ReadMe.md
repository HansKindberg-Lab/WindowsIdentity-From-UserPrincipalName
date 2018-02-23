# WindowsIdentity-From-UserPrincipalName

A Visual Studio 2017 project to test impersonation-level on a WindowsIdentity created from a user-principal-name.

This solution is written/tested for:
- Windows 10
- Visual Studio 2017

When you run these tests in Visual Studio, the Visual Studio process runs with your account, the account you used to login to your computer. To check who:

    System.Security.Principal.WindowsIdentity.GetCurrent().Name

## Act as part of the operating system

 - https://docs.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/act-as-part-of-the-operating-system/

Constant: **SeTcbPrivilege**

To be able to create an "impersonatable" WindowsIdentity from a user-principal-name, by calling:

    new System.Security.Principal.WindowsIdentity("firstname.lastname@company.com");

you have to set your account to **act as part of the operating system**.

1. Open **secpol.msc**
2. Security Settings (Säkerhetsinställningar) -> Local Policies (Lokala principer) -> User Rights Assignment (Tilldening av användarrättigheter)
3. Add your account to **Act as part of the operating system** (**Agera som del av operativsystemet**).

## Claims to Windows Token Service
To be able to create an "impersonatable" WindowsIdentity from a user-principal-name, by calling:

    Microsoft.IdentityModel.WindowsTokenService.S4UClient.UpnLogon("firstname.lastname@company.com");

you need to configure the **Claims to Windows Token Service** (windows-service). First you need to install the windows-feature **Windows Identity Foundation 3.5** on the computer. Then you have to allow yourself access to run it by adding your account to **C:\Program Files\Windows Identity Foundation\v3.5\c2wtshost.exe.config**:

    <?xml version="1.0"?>
        <configuration>
            <configSections>
                <section name = "windowsTokenService" type="Microsoft.IdentityModel.WindowsTokenService.Configuration.WindowsTokenServiceSection, Microsoft.IdentityModel.WindowsTokenService, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" />
            </configSections>
            <startup>
                <supportedRuntime version="v2.0.50727" />
                <supportedRuntime version="v4.0" />
            </startup>
            <windowsTokenService>
                <!-- By default no callers are allowed to use the Windows Identity Foundation Claims To NT Token Service.Add the identities you wish to allow below. -->
                <allowedCallers>
                    <clear />
                    <!-- <add value="NT AUTHORITY\Network Service" /> -->
                    <!-- <add value="NT AUTHORITY\Local Service" /> -->
                    <!-- <add value="NT AUTHORITY\System" /> -->
                    <!-- <add value="NT AUTHORITY\Authenticated Users" /> -->
                    <add value="YOURDOMAIN\your-account-name" />
                </allowedCallers>
            </windowsTokenService>
        </configuration>

Then restart the windows-service **Claims to Windows Token Service**.