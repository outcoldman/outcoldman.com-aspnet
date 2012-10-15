//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;

namespace Microsoft.Samples.XmlRpc
{
	internal class XmlRpcFormatException : Exception
	{
		public XmlRpcFormatException()
		{
		}

		public XmlRpcFormatException(string message)
			: base(message)
		{
		}
	}
}