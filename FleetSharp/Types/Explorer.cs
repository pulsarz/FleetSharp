using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Types
{
	public class ExplorerAsset
	{
		public string? tokenId { get; set; }
		public int? index { get; set; }
		public long? amount { get; set; }
		public string? name { get; set; }
		public int? decimals { get; set; }
		public string? type { get; set; }
	}
	public class ExplorerRegister
	{
		public string? serializedValue { get; set; }
		public string? sigmaType { get; set; }
		public string? renderedValue { get; set; }
	}
	public class ExplorerAdditionalRegisters
	{
		public ExplorerRegister? R4 { get; set; }
		public ExplorerRegister? R5 { get; set; }
		public ExplorerRegister? R6 { get; set; }
		public ExplorerRegister? R7 { get; set; }
		public ExplorerRegister? R8 { get; set; }
		public ExplorerRegister? R9 { get; set; }
	}
	public class ExplorerBox
	{
		public string? boxId { get; set; }
		public string? transactionId { get; set; }
		public string? blockId { get; set; }
		public long? value { get; set; }
		public int? index { get; set; }
		public long? globalIndex { get; set; }
		public int? creationHeight { get; set; }
		public int? settlementHeight { get; set; }
		public string? ergoTree { get; set; }
		public string? address { get; set; }
		public List<ExplorerAsset>? assets { get; set; }
		public ExplorerAdditionalRegisters? additionalRegisters { get; set; }
		public string? spentTransactionId { get; set; }
		public bool? mainChain { get; set; }
	}

	public class ExplorerBoxexWrapper
	{
		public List<ExplorerBox> items { get; set; }
		public int total { get; set; }
	}
}