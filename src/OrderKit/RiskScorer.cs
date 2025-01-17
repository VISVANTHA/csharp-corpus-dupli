using System;
using System.Collections.Generic;

namespace OrderKit
{
    /// <summary>
    /// Score is the planted cognitive-complexity peak: the nesting is the point,
    /// not the branch count. A cyclomatic tool and a cognitive tool should rank
    /// this and PricingService.PriceCents differently, which is the cross-check.
    /// </summary>
    public sealed class RiskScorer
    {
        private readonly ISet<string> _watchedRegions;

        public RiskScorer()
        {
            _watchedRegions = new HashSet<string>();
            _watchedRegions.Add("sanctioned");
            _watchedRegions.Add("embargoed");
        }

        public int Score(Order order, string region, IList<string> flags)
        {
            int score = 0;
            if (order != null)
            {
                if (!order.IsEmpty())
                {
                    foreach (OrderLine line in order.Lines)
                    {
                        if (line.Category == "hazmat")
                        {
                            if (line.Quantity > 10)
                            {
                                if (region != null)
                                {
                                    if (_watchedRegions.Contains(region))
                                    {
                                        score += 40;
                                    }
                                    else
                                    {
                                        score += 15;
                                    }
                                }
                                else
                                {
                                    score += 25;
                                }
                            }
                            else
                            {
                                score += 5;
                            }
                        }
                        else if (line.Category == "fragile")
                        {
                            if (line.Quantity > 50)
                            {
                                score += 8;
                            }
                        }
                    }
                }
            }

            if (flags != null)
            {
                foreach (string flag in flags)
                {
                    if (flag != null)
                    {
                        if (flag.StartsWith("hard-"))
                        {
                            score += 20;
                        }
                        else if (flag.StartsWith("soft-"))
                        {
                            score += 3;
                        }
                    }
                }
            }

            if (score > 100) score = 100;
            return score;
        }
    }
}
