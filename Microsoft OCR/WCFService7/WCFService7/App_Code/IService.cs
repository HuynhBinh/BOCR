using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
[ServiceContract]
public interface IService
{
    [OperationContract]
    [WebGet]
    string Hello();

    //[OperationContract]
    //[WebGet(UriTemplate = "Data/{id}", ResponseFormat = WebMessageFormat.Json)]
    //string GetData(string id);

    //[OperationContract]
    //CompositeType GetDataUsingDataContract(CompositeType composite);

    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "BOCR/Extract", ResponseFormat = WebMessageFormat.Json)]
    string ExtractText(Stream file);

    // TODO: Add your service operations here
}

// Use a data contract as illustrated in the sample below to add composite types to service operations.
//[DataContract]
//public class CompositeType
//{
//    bool boolValue = true;
//    string stringValue = "Hello ";

//    [DataMember]
//    public bool BoolValue
//    {
//        get { return boolValue; }
//        set { boolValue = value; }
//    }

//    [DataMember]
//    public string StringValue
//    {
//        get { return stringValue; }
//        set { stringValue = value; }
//    }
//}
