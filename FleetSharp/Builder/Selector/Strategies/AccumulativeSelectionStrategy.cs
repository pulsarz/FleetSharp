using FleetSharp.Types;
using FleetSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Builder.Selector.Strategies
{
    public class AccumulativeSelectionStrategy<T> : ISelectionStrategy<long>
    {
        private List<Box<long>> _inputs = new List<Box<long>>();

        public List<Box<long>> Select(List<Box<long>> inputs, SelectionTarget<long>? target = null)
        {
            _inputs = inputs;

            var selection = new List<Box<long>>();
            if (target?.tokens?.Any() == true)
            {
                selection.AddRange(_selectTokens(target.tokens));
            }

            long selectedNanoErgs = 0;
            selection.ForEach(x => selectedNanoErgs += x.value);

            if ((target == null && target?.tokens?.Count == 0) ||
                (target != null && selectedNanoErgs < target.nanoErgs))
            {
                long targetAmount = target.nanoErgs != 0
                    ? target.nanoErgs - selectedNanoErgs
                    : 0;

                selection.AddRange(_select(targetAmount));
            }

            return selection;
        }

        private List<Box<long>> _selectTokens(IEnumerable<TokenTargetAmount<long>> targets)
        {
            var selection = new List<Box<long>>();

            foreach (var target in targets)
            {
                var targetAmount = target.amount != 0
                    ? target.amount - BoxUtils.UtxoSum(selection.Select(x => new MinimalBoxAmounts { value = x.value, assets = x.assets.Select(y => new TokenAmount<long> { tokenId = y.tokenId, amount = y.amount }).ToList() }), target.tokenId)
                    : 0;

                if (targetAmount != null && targetAmount <= 0)
                {
                    continue;
                }

                selection.AddRange(_select(targetAmount, target.tokenId));
            }

            return selection;
        }

        private List<Box<long>> _select(long target, string? tokenId = null)
        {
            var inputs = _inputs;
            var acc = default(long);
            var selection = new List<Box<long>>();

            if (target == 0)
            {
                if (tokenId != null)
                {
                    selection.AddRange(inputs.Where(x => x.assets.Any(asset => asset.tokenId == tokenId)));
                }
                else
                {
                    selection.AddRange(inputs);
                }
            }
            else
            {
                for (var i = 0; i < inputs.Count && acc.CompareTo(target) < 0; i++)
                {
                    if (tokenId != null)
                    {
                        foreach (var token in inputs[i].assets)
                        {
                            if (token.tokenId != tokenId)
                            {
                                continue;
                            }

                            acc += token.amount;
                            selection.Add(inputs[i]);
                        }
                    }
                    else
                    {
                        acc += inputs[i].value;
                        selection.Add(inputs[i]);
                    }
                }
            }

            if (selection.Any())
            {
                _inputs = _inputs.Except(selection).ToList();
            }

            return selection;
        }
    }
}
