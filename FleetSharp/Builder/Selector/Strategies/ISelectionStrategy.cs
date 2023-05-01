using FleetSharp.Models;
using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Builder.Selector.Strategies
{
    public interface ISelectionStrategy<T>
    {
        List<ErgoUnsignedInput> Select(List<ErgoUnsignedInput> inputs, SelectionTarget<T>? target = null);
    }
}
