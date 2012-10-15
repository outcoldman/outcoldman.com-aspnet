//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel.Configuration;

namespace Microsoft.Samples.XmlRpc
{
	internal class XmlRpcEndpointBehaviorSection : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get { return typeof (XmlRpcEndpointBehavior); }
		}

		protected override object CreateBehavior()
		{
			return new XmlRpcEndpointBehavior();
		}
	}
}