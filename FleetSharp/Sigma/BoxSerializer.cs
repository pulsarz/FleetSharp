using FleetSharp.Builder;
using FleetSharp.Models;
using FleetSharp.Types;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Sigma
{
    internal class BoxSerializer
    {
        public static int MAX_UINT16_VALUE = 65535;
        private static int BLAKE_256_HASH_LENGTH = 32;

        public static SigmaWriter serializeBox(Box<string> box)
            => serializeBox(box);

        public static SigmaWriter serializeBox(Box<string> box, SigmaWriter writer)
            => serializeBox(box, writer);

        public static SigmaWriter serializeBox(BoxCandidate<string> box, SigmaWriter writer, string[] distinctTokenIds)
            => serializeBox(box, writer, distinctTokenIds);

        public static SigmaWriter serializeBox(dynamic box, SigmaWriter? writer = null, List<string>? distinctTokenIds = null)
        {
            if (writer == null) writer = new SigmaWriter(50000);

            writer.writeVlqInt64((ulong)box.value);
            writer.writeBytes(Tools.HexToBytes(box.ergoTree));
            writer.writeVlq((uint)box.creationHeight);
            writeTokens(writer, box.assets, distinctTokenIds);
            writeRegisters(writer, box.additionalRegisters);

            if (distinctTokenIds != null)
            {
                return writer;
            }
            else
            {
                if (!isBox(box))
                {
                    throw new ArgumentException("Invalid box type.");
                }
            }

            return writer.writeBytes(Tools.HexToBytes(box.transactionId)).writeVlq((uint)box.index);
        }

        private static bool isBox(dynamic box)
        {
            if (box is Box<string> || box is ErgoBox || box is BoxCandidate<long> || box is OutputBuilder)
            {
                return box.transactionId != null && box.index != null;
            }

            return false;
        }

        private static void writeTokens(SigmaWriter writer, List<TokenAmount<long>> tokens, List<string>? tokenIds = null)
        {
            if (tokens.Count == 0)
            {
                writer.write(0);
                return;
            }

            writer.writeVlq((uint)tokens.Count);
            if (tokenIds?.Count > 0)
            {
                tokens.ForEach(token =>
                {
                    writer.writeVlq((uint)tokenIds.IndexOf(token.tokenId)).writeVlqInt64((ulong)token.amount);
                });
            }
            else
            {
                tokens.ForEach(token =>
                {
                    writer.writeBytes(Tools.HexToBytes(token.tokenId)).writeVlqInt64((ulong)token.amount);
                });
            }
        }

        private static uint getRegistersLength(NonMandatoryRegisters registers)
        {
            uint length = 0;
            if (registers.R4 != null) length++;
            if (registers.R5 != null) length++;
            if (registers.R6 != null) length++;
            if (registers.R7 != null) length++;
            if (registers.R8 != null) length++;
            if (registers.R9 != null) length++;

            return length;
        }

        private static void writeRegisters(SigmaWriter writer, NonMandatoryRegisters registers)
        {
            uint length = getRegistersLength(registers);

            writer.writeVlq(length);
            if (length == 0) return;

            if (registers.R4 != null) writer.writeBytes(Tools.HexToBytes(registers.R4));
            if (registers.R5 != null) writer.writeBytes(Tools.HexToBytes(registers.R5));
            if (registers.R6 != null) writer.writeBytes(Tools.HexToBytes(registers.R6));
            if (registers.R7 != null) writer.writeBytes(Tools.HexToBytes(registers.R7));
            if (registers.R8 != null) writer.writeBytes(Tools.HexToBytes(registers.R8));
            if (registers.R9 != null) writer.writeBytes(Tools.HexToBytes(registers.R9));
        }

        public static uint estimateBoxSize(dynamic box, long? withValue = null)
        {
            if (box.creationHeight == null || box.creationHeight <= 0) throw new Exception("\"Box size estimation error: creation height is undefined.");

            uint size = 0;

            size += VLQ.EstimateVlqSize(withValue ?? box.value);
            size += (uint)Tools.HexByteSize(box.ergoTree);
            size += VLQ.EstimateVlqSize(box.creationHeight);
            size += VLQ.EstimateVlqSize(box.assets.Count);

            for (var i = 0; i < box.assets.Count; i++)
            {
                size += (uint)Tools.HexByteSize(box.assets[i].tokenId) + VLQ.EstimateVlqSize(box.assets[i].amount);
            }
            
            if (box.additionalRegisters.R4 != null) size += (uint)Tools.HexByteSize(box.additionalRegisters.R4);
            if (box.additionalRegisters.R5 != null) size += (uint)Tools.HexByteSize(box.additionalRegisters.R5);
            if (box.additionalRegisters.R6 != null) size += (uint)Tools.HexByteSize(box.additionalRegisters.R6);
            if (box.additionalRegisters.R7 != null) size += (uint)Tools.HexByteSize(box.additionalRegisters.R7);
            if (box.additionalRegisters.R8 != null) size += (uint)Tools.HexByteSize(box.additionalRegisters.R8);
            if (box.additionalRegisters.R9 != null) size += (uint)Tools.HexByteSize(box.additionalRegisters.R9);

            uint registersLength = getRegistersLength(box.additionalRegisters);
            size += VLQ.EstimateVlqSize(registersLength);
            size += (uint)BLAKE_256_HASH_LENGTH;
            size += VLQ.EstimateVlqSize(isBox(box) ? box.index : MAX_UINT16_VALUE);

            return size;
        }
    }
}
