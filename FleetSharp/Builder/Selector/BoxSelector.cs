using FleetSharp.Builder.Selector.Strategies;
using FleetSharp.Exceptions;
using FleetSharp.Models;
using FleetSharp.Types;
using FleetSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FleetSharp.Builder.Selector
{
    public class SelectionTarget<T>
    {
        public T? nanoErgs { get; set; }
        public List<TokenTargetAmount<T>>? tokens { get; set; }
    }
    
    public class BoxSelector<T> where T : ErgoUnsignedInput

    {
        private List<ErgoUnsignedInput> _inputs { get; set; }
        private ISelectionStrategy<long>? _strategy;
        private FilterPredicate<ErgoUnsignedInput>? _ensureFilterPredicate;
        private SortingSelector<ErgoUnsignedInput>? _inputsSortSelector;
        private SortingDirection? _inputsSortDir;
        private HashSet<string>? _ensureInclusionBoxIds;

        public BoxSelector(List<ErgoUnsignedInput> inputs)
        {
            _inputs = inputs;
        }

        //todo custom selector

        public List<ErgoUnsignedInput> Select(SelectionTarget<long> target)
        {
            if (_strategy == null)
            {
                _strategy = new AccumulativeSelectionStrategy<ErgoUnsignedInput>();
            }

            var remaining = DeepCloneTarget(target);
            var unselected = new List<ErgoUnsignedInput>(_inputs);
            var selected = new List<ErgoUnsignedInput>();

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

            //if (HasDuplicatesBy(selected, item => item.BoxId))
            if (selected.Select(x => x.boxId).ContainsDuplicates())
            {
                //throw new Exception("DuplicateInputSelection");
                throw new DuplicateInputException();
            }

            var unreached = _getUnreachedTargets(selected, target);
            if (unreached.nanoErgs > 0 || unreached.tokens?.Any() == true)
            {
                throw new InsufficientInputsException(unreached);
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

        private SelectionTarget<long> _getUnreachedTargets(List<ErgoUnsignedInput> inputs, SelectionTarget<long> target)
        {
            SelectionTarget<long> unreached = new SelectionTarget<long> {  };
            long selectedNanoergs = inputs.Sum(input => input.value);

            if (target.nanoErgs != null && target.nanoErgs > selectedNanoergs)
            {
                unreached.nanoErgs = target.nanoErgs - selectedNanoergs;
            }

            if (target.tokens == null || target.tokens.Count == 0)
            {
                return unreached;
            }

            foreach (var tokenTarget in target.tokens)
            {
                long totalSelected = BoxUtils.UtxoSum(inputs.Select(x => new MinimalBoxAmounts { value = x.value, assets = x.assets } ), tokenTarget.tokenId);
                if (tokenTarget.amount != null && tokenTarget.amount > totalSelected)
                {
                    if (tokenTarget.tokenId == inputs.First().boxId)
                    {
                        continue;
                    }

                    if (unreached.tokens == null)
                    {
                        unreached.tokens = new List<TokenTargetAmount<long>>();
                    }

                    unreached.tokens.Add(new TokenTargetAmount<long>
                    {
                        tokenId = tokenTarget.tokenId,
                        amount = tokenTarget.amount - totalSelected
                    });
                }
            }

            return unreached;
        }

        public BoxSelector<T> ensureInclusion(FilterPredicate<ErgoUnsignedInput> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            this._ensureFilterPredicate = predicate;
            return this;
        }

        public BoxSelector<T> ensureInclusion(List<string> boxIds)
        {
            if (boxIds == null)
            {
                throw new ArgumentNullException(nameof(boxIds));
            }

            if (this._ensureInclusionBoxIds == null)
            {
                this._ensureInclusionBoxIds = new HashSet<string>();
            }

            foreach (var boxId in boxIds)
            {
                this._ensureInclusionBoxIds.Add(boxId);
            }

            return this;
        }
        /*
        public BoxSelector<T> ensureInclusion(FilterPredicate<ErgoUnsignedInput> predicateOrBoxIds)
        {
            if (predicateOrBoxIds == null)
            {
                throw new ArgumentNullException(nameof(predicateOrBoxIds));
            }

            return predicateOrBoxIds switch
            {
                FilterPredicate<ErgoUnsignedInput> predicate => ensureInclusion(predicate),
                List<string> boxIds => ensureInclusion(boxIds),
                _ => throw new ArgumentException("Invalid argument type", nameof(predicateOrBoxIds)),
            };
        }
        */
        public BoxSelector<T> orderBy(SortingSelector<ErgoUnsignedInput> selector, SortingDirection? direction = null)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            this._inputsSortSelector = selector;
            this._inputsSortDir = direction;
            return this;
        }

        private bool IsISelectionStrategyImplementation(object obj)
        {
            return (obj is ISelectionStrategy<long> myObj);
        }

        public static SelectionTarget<long> BuildTargetFrom(IEnumerable<ErgoUnsignedInput> boxes)
        {
            if (boxes == null)
            {
                throw new ArgumentNullException(nameof(boxes));
            }

            var tokens = new Dictionary<string, long>();
            long nanoErgs = 0;

            foreach (var box in boxes)
            {
                nanoErgs += box.value;

                foreach (var token in box.assets)
                {
                    var tokenId = token.tokenId;

                    if (tokens.TryGetValue(tokenId, out var amount))
                    {
                        amount += token.amount;
                        tokens[tokenId] = amount;
                    }
                    else
                    {
                        tokens[tokenId] = token.amount;
                    }
                }
            }

            var target = new SelectionTarget<long>
            {
                nanoErgs = nanoErgs,
                tokens = tokens.Select(kv => new TokenTargetAmount<long> { tokenId = kv.Key, amount = kv.Value }).ToList(),
            };

            return target;
        }


    }
}
