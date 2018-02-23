using System;
using System.Configuration;
using System.Security.Principal;
using Microsoft.IdentityModel.WindowsTokenService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Project
{
	[TestClass]
	public class Test
	{
		#region Fields

		private static bool? _actAsPartOfTheOperatingSystem;
		private static bool? _claimsToWindowsTokenService;
		private static Lazy<string> _userPrincipalName;

		#endregion

		#region Properties

		protected internal virtual bool ActAsPartOfTheOperatingSystem
		{
			get
			{
				if(_actAsPartOfTheOperatingSystem == null)
					_actAsPartOfTheOperatingSystem = bool.Parse(ConfigurationManager.AppSettings["ActAsPartOfTheOperatingSystem"]);

				return _actAsPartOfTheOperatingSystem.Value;
			}
		}

		protected internal virtual bool ClaimsToWindowsTokenService
		{
			get
			{
				if(_claimsToWindowsTokenService == null)
					_claimsToWindowsTokenService = bool.Parse(ConfigurationManager.AppSettings["ClaimsToWindowsTokenService"]);

				return _claimsToWindowsTokenService.Value;
			}
		}

		protected internal virtual string UserPrincipalName
		{
			get
			{
				if(_userPrincipalName == null)
					_userPrincipalName = new Lazy<string>(() => ConfigurationManager.AppSettings["UserPrincipalName"]);

				return _userPrincipalName.Value;
			}
		}

		#endregion

		#region Methods

		[TestMethod]
		public void ImpersonationLevel_IfActAsPartOfTheOperatingSystem_ShouldReturnImpersonation_OtherwiseIdentification()
		{
			var expectedImpersonationLevel = this.ActAsPartOfTheOperatingSystem ? TokenImpersonationLevel.Impersonation : TokenImpersonationLevel.Identification;
			var windowsIdentity = new WindowsIdentity(this.UserPrincipalName);

			this.ImpersonationLevelTest(expectedImpersonationLevel, windowsIdentity);
		}

		[TestMethod]
		public void ImpersonationLevel_IfClaimsToWindowsTokenService_ShouldReturnImpersonation_OtherwiseIdentification()
		{
			var expectedImpersonationLevel = this.ClaimsToWindowsTokenService ? TokenImpersonationLevel.Impersonation : TokenImpersonationLevel.Identification;
			var windowsIdentity = S4UClient.UpnLogon(this.UserPrincipalName);

			this.ImpersonationLevelTest(expectedImpersonationLevel, windowsIdentity);
		}

		protected internal virtual void ImpersonationLevelTest(TokenImpersonationLevel expectedImpersonationLevel, WindowsIdentity windowsIdentity)
		{
			Assert.AreEqual(expectedImpersonationLevel, windowsIdentity.ImpersonationLevel);

			using(windowsIdentity.Impersonate())
			{
				Assert.AreEqual(expectedImpersonationLevel, WindowsIdentity.GetCurrent().ImpersonationLevel);
			}
		}

		#endregion
	}
}