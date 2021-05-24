using System;
using System.Collections.Generic;
using System.Linq;

namespace Semigroups
{
    internal class CoreLogic
    {
        public IEnumerable<Matrix> MakeSemigroups(List<Matrix> baseSet, Func<Matrix, Matrix, Matrix> operation)
        {
            var result = new List<Matrix>();
            result.AddRange(baseSet);

            for (; ; )
            {
                var newMatrices = new HashSet<Matrix>();

                for (int i = 0; i < result.Count; i++)
                {
                    for (int j = 0; j < result.Count; j++)
                    {
                        var resultMatrix = operation(result[i], result[j]);

                        if (result.Contains(resultMatrix))
                        {
                            continue;
                        }

                        newMatrices.Add(resultMatrix);
                    }
                }

                if (!newMatrices.Any())
                {
                    return result;
                }

                result.AddRange(newMatrices);
            }
        }
    }
}
