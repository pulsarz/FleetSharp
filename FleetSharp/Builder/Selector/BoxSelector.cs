using FleetSharp.Builder.Selector.Strategies;
using FleetSharp.Types;
using FleetSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Builder.Selector
{
    public class SelectionTarget<T>
    {
        public T? nanoErgs { get; set; }
        public List<TokenTargetAmount<T>>? tokens { get; set; }
    }
    /*
    public class BoxSelector<T> where T : Box<long>

    {
        private List<Box<long>> _inputs { get; set; }
        private ISelectionStrategy<long>? _strategy;
        private FilterPredicate<Box<long>>? _ensureFilterPredicate;
        private SortingSelector<Box<long>>? _inputsSortSelector;
        private SortingDirection? _inputsSortDir;
        private HashSet<string>? _ensureInclusionBoxIds;

        public BoxSelector(List<Box<long>> inputs)
        {
            _inputs = inputs;
        }

        //todo custom selector

        public List<Box<long>> Select(SelectionTarget<long> target)
        {
            if (_strategy == null)
            {
                _strategy = new AccumulativeSelectionStrategy<Box<long>>();
            }

            var remaining = DeepCloneTarget(target);
            var unselected = new List<Box<long>>(_inputs);
            var selected = new List<Box<long>>();

            var predicate = _ensureFilterPredicate;
            var inclusion = _ensureInclusionBoxIds;

            if (predicate != null)
            {
                if (inclusion != null)
                {
                    selected = unselected
                        .Where(box => predicate(box) || inclusion.Contains(box.boxId))
                        .ToList();
                }
                else
                {
                    selected = unselected
                        .Where(box => predicate(box))
                        .ToList();
                }
            }
            else if (inclusion != null)
            {
                selected = unselected
                    .Where(box => inclusion.Contains(box.boxId))
                    .ToList();
            }

            if (selected.Any())
            {
                unselected = unselected
                    .Where(box => !selected.Any(sel => sel.boxId == box.boxId))
                    .ToList();

                if (remaining.nanoErgs > 0)
                {
                    long temp = 0;
                    selected.ForEach(x => temp += x.value);
                    remaining.nanoErgs -= temp;
                }

                if (remaining.tokens != null && remaining.tokens.Any() && selected.Any(input => input.assets.Count > 0))
                {
                    foreach (var t in remaining.tokens)
                    {
                        if (t.amount > 0)
                        {
                            t.amount -= BoxUtils.UtxoSum(selected.Select(x => new MinimalBoxAmounts { value = x.value, assets = x.assets.Select(y => new TokenAmount<long> { tokenId = y.tokenId, amount = y.amount }).ToList() }), t.tokenId);
                        }
                    }
                }
            }

            if (_inputsSortSelector != null)
            {
                if (_inputsSortDir == SortingDirection.Descending) unselected = unselected.OrderByDescending(x => _inputsSortSelector(x)).ToList();
                else unselected = unselected.OrderBy(x => _inputsSortSelector(x)).ToList();
            }

            selected.AddRange(_strategy.Select(unselected, remaining));

            if (HasDuplicatesBy(selected, item => item.BoxId))
            {
                throw new Exception("DuplicateInputSelection");
            }

            var unreached = GetUnreachedTargets(selected, target);
            if (unreached.NanoErgs > 0 || unreached.Tokens?.Any() == true)
            {
                throw new Exception("InsufficientInputs");
            }

            return selected;
        }

        private SelectionTarget<long> DeepCloneTarget(SelectionTarget<long> target)
        {
            return new SelectionTarget<long>
            {
                nanoErgs = target.nanoErgs,
                tokens = target.tokens?
                    .Select(t => new TokenTargetAmount<long>
                    {
                        tokenId = t.tokenId,
                        amount = t.amount
                    })
                    .ToList()
            };
        }
    }*/
}
