using System;
using System.ServiceModel;

namespace ParagoServices
{
	[ServiceContract(Namespace = ParagoServiceNamespaces.Contract)]
	public interface IParagoServiceApplication
	{
		[OperationContract]
		[FaultContract(typeof(ParagoServiceFault), Name = "ParagoServiceFault")]
		string ReadSetting(string key);

		[OperationContract]
		[FaultContract(typeof(ParagoServiceFault), Name = "ParagoServiceFault")]
		void UpdateSetting(string key, string value);

		[OperationContract]
		[FaultContract(typeof(ParagoServiceFault), Name = "ParagoServiceFault")]
		string GetDataSize(DataCollection<String> data);
	}
}