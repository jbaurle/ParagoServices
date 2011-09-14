//
// Parago Media GmbH & Co. KG, Jürgen Bäurle (http://www.parago.de)
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace ParagoServices.Features.Feature
{
	[Guid("0f2c5d47-1a51-4ace-a1db-46a0766172ed")]
	public class FeatureEventReceiver : SPFeatureReceiver
	{
		public override void FeatureActivated(SPFeatureReceiverProperties properties)
		{
			//System.Diagnostics.Debugger.Break();

			SPWeb web = properties.Feature.Parent as SPWeb;

			// NOTE: After an Parago Service has been created for the first time, SharePoint’s service 
			// application management will become aware of the new service application and allow 
			// administrators to create or manage instances of the Parago Service Application from 
			// Central Administration. If more than one entry of 'Parago Services' is appearing in 
			// the New popup menu, than different service instance are created and can be removed by 
			// calling the SPFarm.Local.Services.Remove method.
			ServiceHelper.CreateParagoServiceWithServiceInstance();
		}

		public override void FeatureInstalled(SPFeatureReceiverProperties properties)
		{
			//System.Diagnostics.Debugger.Break();

			// NOTE: The names must be lower case in order to work, see class SPClientServiceHost, 
			// ClientServiceHost, MultipleBaseAddressBasicHttpBindingServiceHost as well as
			// BasicHttpBindingEndpointCreator to understand underlying SharePoint logic.
			ConfigureWebServiceQuotas("paragoservice.svc");
			ConfigureWebServiceQuotas("paragoservicerest.svc");
		}

		public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
		{
			SPWeb web = properties.Feature.Parent as SPWeb;

			//ServiceHelper.RemoveParagoServiceAndAllDependentObjects();
		}

		#region Helper Methods

		static void ConfigureWebServiceQuotas(string webServiceName)
		{
			SPWcfServiceSettings settings = new SPWcfServiceSettings {
				MaxReceivedMessageSize = 2147483647,
				MaxBufferSize = 2147483647,
				OpenTimeout = TimeSpan.FromMinutes(5),
				CloseTimeout = TimeSpan.FromMinutes(5),
				ReceiveTimeout = TimeSpan.FromMinutes(5),
				ReaderQuotasMaxDepth = 2147483647,
				ReaderQuotasMaxStringContentLength = 2147483647,
				ReaderQuotasMaxArrayLength = 2147483647,
				ReaderQuotasMaxBytesPerRead = 2147483647,
				ReaderQuotasMaxNameTableCharCount = 2147483647,
			};

			SPWebService contentService = SPWebService.ContentService;
			contentService.WcfServiceSettings[webServiceName] = settings;
			contentService.Update();
		}

		#endregion
	}
}
