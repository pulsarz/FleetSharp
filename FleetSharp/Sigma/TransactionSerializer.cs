using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Sigma
{
    public class MinimalUnsignedTransaction
    {
        public List<UnsignedInput> inputs { get; set; }
        public List<DataInput> dataInputs { get; set; }
        public List<BoxCandidate<long>> outputs { get; set; }
    }
    internal static class TransactionSerializer
    {
        public static SigmaWriter serializeTransaction(MinimalUnsignedTransaction transaction)
        {
            var writer = new SigmaWriter(100000);

            //write inputs
            writer.writeVlq((uint)transaction.inputs.Count);
            transaction.inputs.ForEach(input => {
                writeInput(writer, input);
            });

            //write data inputs
            writer.writeVlq((uint)transaction.dataInputs.Count);
            transaction.dataInputs.ForEach(dataInput => {
                writer.writeBytes(Tools.HexToBytes(dataInput.boxId));
            });

            //write distinct token IDs
            var distinctTokenIds = GetDistinctTokenIds(transaction.outputs);
            writer.writeVlq((uint)distinctTokenIds.Count);
            distinctTokenIds.ForEach(tokenId =>
            {
                writer.writeBytes(Tools.HexToBytes(tokenId));
            });

            //write outputs
            writer.writeVlq((uint)transaction.outputs.Count);
            transaction.outputs.ForEach(output => {
                BoxSerializer.serializeBox(output, writer, distinctTokenIds);
            });

            return writer;
        }

        public static void writeInput(SigmaWriter writer, UnsignedInput input)
        {
            writer.writeBytes(Tools.HexToBytes(input.boxId));
            writer.write(0);
            writeExtension(writer, input.extension);
        }

        public static void writeExtension(SigmaWriter writer, ContextExtension extension)
        {
            writer.writeVlq((uint)extension.extensions.Count);
            if (extension.extensions.Count == 0) return;

            foreach (var ext in extension.extensions)
            {
                writer.writeVlq((uint)ext.Key).writeBytes(Tools.HexToBytes(ext.Value));
            }
        }

        public static List<string> GetDistinctTokenIds(List<BoxCandidate<long>> outputs)
        {
            var tokenIds = new HashSet<string>();
            foreach (var output in outputs)
            {
                foreach (var asset in output.assets)
                {
                    tokenIds.Add(asset.tokenId);
                }
            }

            return tokenIds.ToList();
        }
    }
}
